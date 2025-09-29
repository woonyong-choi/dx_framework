using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JSerialization
    //      새로운 Serialization을 만드는 이유는 다음과 같다.
    //          1. 범용 Serialization은 타입과 값을 모두 저장하기 때문에 용량이 크고 속도가 느리다.
    //          2. 클라이언트와 서버의 객체들은 비슷한 클래스를 사용하지만 정확히 같은 타입은 아니며(ex-LsfUser, LcUser) 어셈블리가 다르기 때문에 
    //             Serialization이 불가능하다.
    //          -> 1번의 경우는 많은 오픈소스가 존재하지만 2번의 이유로 직접 제작하도록 한다.
    //
    //
    // @JSerialization
    //      가장 기본적인 데이터 직렬화 방식(BinaryFormatter 기반)
    //
    // @FstSerialization
    //      Reflection을 이용하여 기본 데이터만을 Serialize & Deserialize하는 방식으로, null이나 추상타입에 대한 지원을 하지 않고, 최소한의 데이터만을 변환하는 방식
    //
    // @JNewSerialization : TypeHashing
    //      Fast와 유사한 알고리즘으로 동작하나 데이터의 형식(Type FullName)과 함께 데이터를 변환하여 null이나 추상 타입에 대한 제약이 없는 방식.단, 데이터의 형식이 차지하는 데이터 크기를 최소화하기 위한 Hashing과정이 추가됨
    //      (※ Hashing 결과물을 해석하기 위한 데이터는 별도의 파일로 저장 되며, 이에 대한 경로 지정 필요)
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JNewSerialization
    //
    //      1. Fast Serialization
    //      2. TypeHashing Serialization
    //      3. Network Serialization
    //      4. Variable Replication(변수 리플리케이션)을 위한 객체 변환 감지
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static class JNewSerialization
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] BindingFlags
        static readonly BindingFlags _flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        #endregion

        // todo : fast & rpc - ValueTuple

        #region [변수] [Serializer Optimize]
        public static bool _type_dict_dirty = true;
        public static Dictionary<int, Type> _type_dict = new Dictionary<int, Type>();
        public static List<string> _type_name_list = new List<string>();
        public static string _type_name_list_load_path = "type_name_list.txt";
        public static string _type_name_list_save_path = "type_name_list.txt";
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Serialization] [TypeHashing]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Utility] [TypeHashing] GetHashType
        public static Type GetHashType(int hash_key)
        {
            if (_type_dict.ContainsKey(hash_key))
                return _type_dict[hash_key];

            JLogger.WriteFormat("[JNewSerialization] GetHashType - {0} is Not Menegement Hash Key", hash_key);

            return null;
        }

#if !RELEASE
        public static Dictionary<string, Type> _str_type_dict = new Dictionary<string, Type>();
        public static Type GetSerializeType(string type_name)
        {
            if (_str_type_dict.TryGetValue(type_name, out Type type))
                return type;

            type = JUtil.OldGetType(type_name);
            if ((type == null) && (type_name != "NULL"))
            {
                JLogger.WriteFormat("[JNewSerialization] GetType - {0} is Not Menegement Type", type_name);
                return null;
            }

            _str_type_dict.Add(type_name, type);
            return type;
        }
#endif
        #endregion

        #region [Serialize] [TypeHashing] object (+Type)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Serialize(BinaryWriter writer, object value, Type type = null)
        {
            if (value == null)
            {
                SerializeHash(writer, "NULL");
                return;
            }

            if (type == null)
                type = value.GetType();

            if (type.IsEnum)
            {
                SerializeHash(writer, type.FullName, type);
                writer.Write((int)value);
            }
            else if (JReflection.s_write_methods.TryGetValue(type, out MethodInfo writeMethod))
            {
                SerializeHash(writer, type.FullName, type);
                writeMethod.Invoke(writer, new object[] { value });
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                Type genericArgType;
                if (type.IsArray)
                    genericArgType = type.GetElementType();
                else
                    genericArgType = type.GetGenericArguments()[0];

                type = JUtil.MakeGenericTypeName(type);
                SerializeHash(writer, type.FullName, type);
                SerializeHash(writer, genericArgType.FullName, genericArgType);

                var list = value as IList;

                Serialize(writer, list.Count);
                foreach (var item in list)
                {
                    Serialize(writer, item);
                }
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var genericArgsType = type.GetGenericArguments();

                type = JUtil.MakeGenericTypeName(type);
                SerializeHash(writer, type.FullName, type);
                SerializeHash(writer, genericArgsType[0].FullName, genericArgsType[0]);
                SerializeHash(writer, genericArgsType[1].FullName, genericArgsType[1]);

                var dict = value as IDictionary;

                Serialize(writer, dict.Count);
                foreach (var key in dict.Keys)
                {
                    Serialize(writer, key);
                    Serialize(writer, dict[key]);
                }
            }
            //else if (typeof(ITuple).IsAssignableFrom(type))
            //{
            //    var genericArgsType = type.GetGenericArguments();

            //    type = JUtil.MakeGenericTypeName(type);
            //    SerializeHash(writer, type.FullName, type);

            //    var tuple = value as ITuple;

            //    int length = tuple.Length;
            //    Serialize(writer, length);
            //    for (int i = 0; i < length; ++i)
            //    {
            //        SerializeHash(writer, genericArgsType[i].FullName, genericArgsType[i]);
            //        Serialize(writer, tuple[i]);
            //    }
            //}
            //else if (typeof(JObject).IsAssignableFrom(type))
            //{
            //    var obj = value as JObject;
            //    SerializeHash(writer, type.FullName, type);
            //    obj.FastSerialize(writer);
            //    //JObjectManager.Serialize(writer, value as JObject);
            //}
            //else if (type.IsClass || type.IsValueType)
            //{
            //    SerializeHash(writer, type.FullName, type);
            //    FieldInfo[] fields = type.GetFields(_flags);
            //    foreach (FieldInfo field in fields)
            //    {
            //        Serialize(writer, field.GetValue(value));
            //    }
            //}
        }

        #endregion

        #region [Serialize] [TypeHashing] Function Overloading
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Serialize(Stream stream, object value, Type type = null) { Serialize(new BinaryWriter(stream), value, type); }
        public static void Serialize(BinaryWriter writer, params object[] args) { Serialize(writer, (object)args); }
        public static void Serialize(Stream stream, params object[] args) { Serialize(new BinaryWriter(stream), (object)args); }
        #endregion

        #region [Deserialize] [TypeHashing] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object Deserialize(BinaryReader reader)
        {
            Type type = DeserializeHash(reader);

            if (type == null)
                return null;

            if (type.IsEnum)
            {
                var enum_inst = Activator.CreateInstance(type);
                enum_inst = type.GetEnumValues().GetValue(reader.ReadInt32());
                return enum_inst;
            }
            else if (JReflection.s_read_methods.TryGetValue(type, out MethodInfo readMethod))
            {
                return readMethod.Invoke(reader, null);
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                Type item_type = DeserializeHash(reader);

                IList list;

                int list_count = (int)Deserialize(reader);
                if (type.IsArray)
                {
                    list = Array.CreateInstance(item_type, list_count);
                    for (int i = 0; i < list_count; ++i)
                        list[i] = Deserialize(reader);
                }
                else
                {
                    list = Activator.CreateInstance(type.MakeGenericType(item_type)) as IList;
                    for (int i = 0; i < list_count; ++i)
                        list.Add(Deserialize(reader));
                }

                return list;
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                Type key_type = DeserializeHash(reader);
                Type value_type = DeserializeHash(reader);

                var dict = Activator.CreateInstance(type.MakeGenericType(key_type, value_type)) as IDictionary;

                int dict_count = (int)Deserialize(reader);
                for (int i = 0; i < dict_count; ++i)
                {
                    var key = Deserialize(reader);
                    var item = Deserialize(reader);

                    dict.Add(key, item);
                }

                return dict;
            }
            //else if (typeof(ITuple).IsAssignableFrom(type))
            //{
            //    int list_count = (int)Deserialize(reader);
            //    var types = new Type[list_count];
            //    var args = new object[list_count];

            //    for (int i = 0; i < list_count; ++i)
            //    {
            //        types[i] = DeserializeHash(reader);
            //        args[i] = Deserialize(reader);
            //    }

            //    return Activator.CreateInstance(type.MakeGenericType(types), args);
            //}

            //else if (typeof(JObject).IsAssignableFrom(type))
            //{
            //    var inst = Activator.CreateInstance(type) as JObject;
            //    inst.FastDeserialize(reader);
            //    //return JObjectManager.Deserialize(reader);
            //    return inst;
            //}
            //else if (type.IsClass || type.IsValueType)
            //{
            //    object inst = Activator.CreateInstance(type);
            //    FieldInfo[] fields = type.GetFields(_flags);
            //    foreach (FieldInfo field in fields)
            //    {
            //        field.SetValue(inst, Deserialize(reader));
            //    }
            //    return inst;
            //}

            return null;
        }
        #endregion

        #region [Deserialize] [TypeHashing] Function Overloading
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object Deserialize(Stream stream) { return Deserialize(new BinaryReader(stream)); }
        public static T Deserialize<T>(BinaryReader reader) { return (T)Deserialize(reader); }
        public static T Deserialize<T>(Stream stream) { return (T)Deserialize(stream); }
        #endregion

        #region [Deserialize] [TypeHashing] RPC (object[])
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object[] Deserialize(BinaryReader reader, MethodInfo mi)
        {
            var arguments = mi.GetGenericArguments();
            var targets = new object[arguments.Length];

            for (int i = 0; i < arguments.Length; ++i)
            {
                targets[i] = Deserialize(reader);
            }

            return targets;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object[] Deserialize(Stream stream, MethodInfo mi)
        {
            return Deserialize(new BinaryReader(stream), mi);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Serialization] [Fast] (추상 타입 사용 불가, null 불가)
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Serialize] [Fast] object (+Type)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void FastSerialize(BinaryWriter writer, object value, Type type = null)
        {
            if (type == null)
                type = value.GetType();

            #region Native Data Serialize
            if (type.IsEnum)
            {
                FastSerialize(writer, value, typeof(int));
                return;
            }

            if (JReflection.s_write_methods.TryGetValue(type, out MethodInfo writeMethod))
            {
                writeMethod.Invoke(writer, new object[] { value });
                return;
            }
            #endregion

            #region List Data Serialize
            if (typeof(IList).IsAssignableFrom(type))
            {
                var list = value as IList;

                int count = list.Count;
                FastSerialize(writer, count);

                for (int i = 0; i < count; ++i)
                    FastSerialize(writer, list[i]);

                return;
            }
            #endregion

            #region Dictionary Data Serialize
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var dict = value as IDictionary;

                int count = dict.Count;
                FastSerialize(writer, count);

                var arg_type = dict.GetType().GetGenericArguments();

                foreach (var key in dict.Keys)
                {
                    FastSerialize(writer, key, arg_type[0]);
                    FastSerialize(writer, dict[key], arg_type[1]);
                }

                return;
            }
            #endregion

            #region Tuple Data Serialize
            //if (typeof(ITuple).IsAssignableFrom(type))
            //{
            //    var tuple = value as ITuple;
            //    int length = tuple.Length;
            //    FastSerialize(writer, length);

            //    for (int i = 0; i < length; ++i)
            //        FastSerialize(writer, tuple[i]);

            //    return;
            //}
            #endregion

            #region Class & Struct Data Serialize
            var fields = type.GetFields(_flags);
            foreach (var fi in fields)
            {
                var target = fi.GetValue(value);
                if (target == null)
                    throw new Exception("JNewSerialization::FastSerialize - target is null");

                FastSerialize(writer, target);
            }
            #endregion
        }
        #endregion

        #region [Serialize] [Fast] Function Overloading
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void FastSerialize(Stream stream, object value, Type type = null) { FastSerialize(new BinaryWriter(stream), value, type); }
        public static void FastSerialize(Stream stream, params object[] args) { FastSerialize(new BinaryWriter(stream), args); }
        public static void FastSerialize(BinaryWriter writer, params object[] args)
        {
            for (int i = 0; i < args.Length; ++i)
                FastSerialize(writer, args[i]);
        }
        #endregion

        #region [Deserialize] [Fast]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object FastDeserialize(BinaryReader reader, Type type)
        {
            #region Native Data Deserialize
            if (type.IsEnum)
            {
                return FastDeserialize(reader, typeof(int));
            }

            if (JReflection.s_read_methods.TryGetValue(type, out MethodInfo readMethod))
            {
                return readMethod.Invoke(reader, null);
            }
            #endregion

            #region Array Data Deserialize
            if (type.IsArray == true)
            {

                int count = (int)FastDeserialize(reader, typeof(int));
                var array = Activator.CreateInstance(type, new object[] { count }) as IList;
                var array_itemType = type.GetElementType();

                for (int i = 0; i < count; ++i)
                {
                    array[i] = FastDeserialize(reader, array_itemType);
                }

                return array;
            }
            #endregion

            #region Tuple Data Deserialize
            //if (typeof(ITuple).IsAssignableFrom(type))
            //{
            //    int list_count = (int)FastDeserialize(reader, typeof(int));
            //    var args = new object[list_count];
            //    var types = type.GetGenericArguments();

            //    for (int i = 0; i < list_count; ++i)
            //        args[i] = FastDeserialize(reader, types[i]);

            //    return Activator.CreateInstance(type, args);
            //}
            #endregion

            var target = Activator.CreateInstance(type); // create instance

            #region List Data Deserialize
            if (typeof(IList).IsAssignableFrom(type))
            {
                var list = target as IList;
                var list_itemType = type.GetGenericArguments()[0];

                int count = (int)FastDeserialize(reader, typeof(int));
                for (int i = 0; i < count; ++i)
                {
                    list.Add(FastDeserialize(reader, list_itemType));
                }

                return list;
            }
            #endregion

            #region Dictionary Data Deserialize
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var dict = target as IDictionary;

                var dict_keyType = type.GetGenericArguments()[0];
                var dict_itemType = type.GetGenericArguments()[1];

                int count = (int)FastDeserialize(reader, typeof(int));
                for (int i = 0; i < count; ++i)
                {
                    dict.Add(FastDeserialize(reader, dict_keyType), FastDeserialize(reader, dict_itemType));
                }

                return dict;
            }
            #endregion

            #region Class & Struct Deserialize
            var fields = type.GetFields(_flags);

            foreach (var fi in fields)
            {
                fi.SetValue(target, FastDeserialize(reader, fi.FieldType));
            }
            #endregion

            return target;
        }
        #endregion

        #region [Deserialize] [Fast] Function Overloading
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object FastDeserialize(Stream stream, Type type) { return FastDeserialize(new BinaryReader(stream), type); }
        public static T FastDeserialize<T>(BinaryReader reader) { return (T)FastDeserialize(reader, typeof(T)); }
        public static T FastDeserialize<T>(Stream stream) { return (T)FastDeserialize(new BinaryReader(stream), typeof(T)); }
        #endregion

        #region [Deserialize] [Fast] RPC (object[], fast, 추상 타입 사용 불가, null 불가)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object[] FastDeserialize(BinaryReader reader, MethodInfo mi)
        {
            var arguments = mi.GetGenericArguments();
            var targets = new object[arguments.Length];

            for (int i = 0; i < arguments.Length; ++i)
            {
                targets[i] = FastDeserialize(reader, arguments[i].GetType());
            }

            return targets;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object[] FastDeserialize(Stream stream, MethodInfo mi)
        {
            return FastDeserialize(new BinaryReader(stream), mi);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Serializer Optimize] 
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Serializer Optimize] [TypeNameList] Save / Load
        public static void SaveTypeNameList()
        {
            try
            {
                var writer = new StringWriter();

                writer.Write(_type_name_list.Count);
                foreach (string type_name in _type_name_list)
                {
                    writer.Write("\r\n");
                    writer.Write(type_name);
                }

                File.WriteAllText(_type_name_list_save_path, writer.ToString());
                writer.Close();
            }
            catch (DirectoryNotFoundException)
            {
                //JUtil.CreateFolder(_type_name_list_path);
                //Debug.Log("JNewSerialization::SaveTypeNameList() - [ DirectoryNotFoundException ] Create Directory, {0}", _type_name_list_path);
                //
                //SaveTypeNameList();
            }
        }

        public static bool LoadTypeNameList()
        {
            try
            {
                //#if NET_SERVER
                //                var full_text = File.ReadAllText(_type_name_list_load_path);
                //                var reader = new StringReader(full_text);
                //#else
                //                var txtFile = Resources.Load(_type_name_list_load_path, typeof(TextAsset)) as TextAsset;
                //                var reader = new StringReader(txtFile.text);
                //#endif
                //                var cnt_string = reader.ReadLine();
                //                var cnt = int.Parse(cnt_string);
                //                for (int i = 0; i < cnt; ++i)
                //                    _type_name_list.Add(reader.ReadLine());

                //                reader.Close();

                return true;
            }
            catch (FileNotFoundException)
            {
                // nothing.
            }
            catch (DirectoryNotFoundException)
            {
                //JUtil.CreateFolder(_type_name_list_path);
                //Debug.Log("JNewSerialization::LoadTypeNameList() - [ DirectoryNotFoundException ] Create Directory, {0}", _type_name_list_path);
            }
            //catch (EndOfStreamException)
            //{
            //    // nothing.
            //}


            return false;
        }
        #endregion

        #region [Serializer Optimize] [갱신] Update TypeHash
        static void UpdateTypeHash()
        {
            if (_type_dict_dirty == false)
                return;
            _type_dict_dirty = false;

            _type_name_list.Clear();

            LoadTypeNameList();

            foreach (string type_name in _type_name_list)
            {
#if RELEASE
                int hash_idx = JUtil.StringHash(type_name);
                if (_type_dict.ContainsKey(hash_idx))
                    continue;

                var type = JUtil.TryGetType(type_name);
                if (type == null && type_name != "NULL")
                    continue;

                _type_dict.Add(hash_idx, type);
#else
                if (_str_type_dict.ContainsKey(type_name))
                    continue;

                var type = JUtil.OldGetType(type_name);
                if (type == null && type_name != "NULL")
                    continue;

                _str_type_dict.Add(type_name, type);
#endif
            }
        }

        public static int UpdateTypeHash(string str, Type type)
        {
            UpdateTypeHash();

            int hash_idx = JUtil.StringHash(str);
            if (_type_dict.ContainsKey(hash_idx) == false)
            {
                if (_type_name_list.Contains(str) == false)
                {
                    _type_name_list.Add(str);
                    SaveTypeNameList();
                }

                _type_dict.Add(hash_idx, type);
            }

            return hash_idx;
        }
        #endregion

        #region [Serializer Optimize] [Serialize] String To Hash & Save Type
        static void SerializeHash(BinaryWriter writer, string str, Type type = null)
        {
            UpdateTypeHash();
#if RELEASE
            int hash_idx = JUtil.StringHash(str);
            if (_type_dict.ContainsKey(hash_idx) == false)
            {
                _type_name_list.Add(str);
                _type_dict.Add(hash_idx, type);
                SaveTypeNameList();
                _on_add_new_type_name?.Invoke(str);
            }

            writer.Write(hash_idx);
#else
            if (_type_name_list.Contains(str) == false)
            {
                _type_name_list.Add(str);
                SaveTypeNameList();
            }

            writer.Write(str);
#endif
        }
        #endregion

        #region [Serializer Optimize] [Deserialize] Hash To Type
        static Type DeserializeHash(BinaryReader reader)
        {
#if RELEASE
            UpdateTypeHash();

            return GetHashType(reader.ReadInt32());
#else
            return GetSerializeType(reader.ReadString());
#endif
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 4. Variable Replication
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [VariableReplication] DiffSerialize
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int DiffSerialize(ref List<object> obj_list, object current_value, object prev_value, int parnet_field_index = -1)
        {
            if (current_value == null && prev_value == null)
                return 0;

            if (parnet_field_index != -1)
                obj_list.Add(parnet_field_index);

            if (current_value == null || prev_value == null)
            {
                obj_list.Add(-1);

                if (current_value == null)
                    obj_list.Add(UpdateTypeHash("NULL", null));
                else
                {
                    var curr_type = current_value.GetType();
                    obj_list.Add(UpdateTypeHash(curr_type.FullName, curr_type));
                    obj_list.Add(current_value);
                }
                return -1;
            }

            Type type = current_value.GetType();

            int diff_count_index = obj_list.Count;
            int diff_count = 0;
            obj_list.Add(0);
            obj_list.Add(UpdateTypeHash(type.FullName, type));

            #region Native Data Serialize
            if (type.IsEnum)
            {
                if (current_value.Equals(prev_value) == false)
                {
                    obj_list.Add((int)current_value);
                    diff_count++;
                }
            }
            else if (JReflection.HasWriteMethod(type))
            {
                if (current_value.Equals(prev_value) == false)
                {
                    obj_list.Add(current_value);
                    diff_count++;
                }
            }
            #endregion

            #region List & Array Serialize
            else if (typeof(IList).IsAssignableFrom(type))
            {
                var curr_list = current_value as IList;
                var prev_list = prev_value as IList;

                if (curr_list.Count == prev_list.Count)
                {
                    obj_list.Add(true);

                    int list_diff_count_index = obj_list.Count;
                    int list_diff_count = 0;
                    obj_list.Add(0);

                    int count = curr_list.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        if (DiffSerialize(ref obj_list, curr_list[i], prev_list[i], i) != 0)
                            list_diff_count++;
                    }

                    if (list_diff_count > 0)
                    {
                        obj_list[list_diff_count_index] = list_diff_count;
                        diff_count++;
                    }
                    else
                    {
                        obj_list.RemoveAt(list_diff_count_index);
                        obj_list.RemoveAt(list_diff_count_index - 1);
                    }
                }
                else
                {
                    obj_list.Add(false);
                    obj_list.Add(curr_list);
                    diff_count++;
                }
            }
            #endregion

            #region Dictionary Serialize
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var curr_dict = current_value as IDictionary;
                var prev_dict = prev_value as IDictionary;

                int dict_diff_count_index = obj_list.Count;
                int dict_diff_count = 0;
                obj_list.Add(0);

                foreach (var key in curr_dict.Keys)
                {
                    if (prev_dict.Contains(key))
                    {
                        int flag_index = obj_list.Count;
                        obj_list.Add(0); // float Flag
                        obj_list.Add(key);
                        if (DiffSerialize(ref obj_list, curr_dict[key], prev_dict[key], -1) != 0)
                        {
                            dict_diff_count++;
                        }
                        else
                        {
                            obj_list.RemoveAt(flag_index + 1);
                            obj_list.RemoveAt(flag_index);
                        }
                        prev_dict.Remove(key);
                    }
                    else
                    {
                        obj_list.Add(1); // Add Flag
                        obj_list.Add(key);
                        obj_list.Add(curr_dict[key]);
                        dict_diff_count++;
                    }
                }

                foreach (var key in prev_dict.Keys)
                {
                    obj_list.Add(2); // Delete Flag
                    obj_list.Add(key);
                    dict_diff_count++;
                }

                if (dict_diff_count > 0)
                {
                    obj_list[dict_diff_count_index] = dict_diff_count;
                    diff_count++;
                }
                else
                {
                    obj_list.RemoveAt(dict_diff_count_index);
                }
            }
            #endregion

            #region Class & Struct Serialize
            else if (type.IsClass || type.IsValueType)
            {
                FieldInfo[] fields = type.GetFields(_flags);
                var fields_length = fields.Length;
                for (int i = 0; i < fields_length; ++i)
                {
                    object curr_field_value = fields[i].GetValue(current_value);
                    object prev_field_value = fields[i].GetValue(prev_value);

                    if (DiffSerialize(ref obj_list, curr_field_value, prev_field_value, i) != 0)
                        diff_count++;
                }
            }
            #endregion

            if (diff_count > 0)
                obj_list[diff_count_index] = diff_count;
            else
            {
                obj_list.RemoveAt(diff_count_index + 1);
                obj_list.RemoveAt(diff_count_index);
                if (parnet_field_index >= 0)
                    obj_list.RemoveAt(diff_count_index - 1);
            }

            return diff_count;
        }

        public static int DiffSerialize<T>(BinaryWriter writer, T current_value, ref T prev_value)
        {
            List<object> obj_list = new List<object>();
            DiffSerialize(ref obj_list, current_value, prev_value);

            foreach (var item in obj_list)
            {
                Serialize(writer, item);
            }

            prev_value = (T)JUtil.CloneProcedure(current_value);

            return obj_list.Count;
        }

        public static int DiffSerialize<T>(Stream stream, T current_value, ref T prev_value)
        {
            return DiffSerialize(new BinaryWriter(stream), current_value, ref prev_value);
        }
        #endregion

        #region [Variable Replication] DiffDeserialize
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object DiffDeserialize(BinaryReader reader, object target)
        {
            int diff_count = (int)Deserialize(reader);

            UpdateTypeHash();
            Type type = GetHashType((int)Deserialize(reader));

            if (diff_count == -1)
            {
                if (type == null)
                    return null;
                else
                    return Deserialize(reader);
            }

            #region Native Data Deserialize
            if (type.IsEnum)
            {
                var enum_inst = Activator.CreateInstance(type);
                enum_inst = type.GetEnumValues().GetValue((int)Deserialize(reader));
                return enum_inst;
            }
            else if (JReflection.HasReadMethod(type))
            {
                return Deserialize(reader);
            }
            #endregion

            #region List & Array Deserialize
            else if (typeof(IList).IsAssignableFrom(type))
            {
                bool isFix = (bool)Deserialize(reader);
                if (isFix)
                {
                    var target_list = target as IList;
                    int list_diff_count = (int)Deserialize(reader);
                    for (int li = 0; li < list_diff_count; ++li)
                    {
                        int list_index = (int)Deserialize(reader);
                        target_list[list_index] = DiffDeserialize(reader, target_list[list_index]);
                    }
                    return target_list;
                }
                else
                {
                    return Deserialize(reader);
                }
            }
            #endregion

            #region Dictionary Deserialize
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var target_dict = target as IDictionary;
                int dict_diff_count = (int)Deserialize(reader);
                for (int count = 0; count < dict_diff_count; ++count)
                {
                    switch ((int)Deserialize(reader))
                    {
                        case 0: // float
                            {
                                var key = Deserialize(reader);
                                target_dict[key] = DiffDeserialize(reader, target_dict[key]);
                            }
                            break;
                        case 1: // Add
                            {
                                var key = Deserialize(reader);
                                var value = Deserialize(reader);
                                if (target_dict.Contains(key) == true)
                                    target_dict[key] = value;
                                else
                                    target_dict.Add(key, value);
                            }
                            break;

                        case 2: // Delete
                            {
                                target_dict.Remove(Deserialize(reader));
                            }
                            break;

                        default:
                            {
                                throw new Exception("JNewSerialization::DiffDeserialize - IDictionary Flag Error");
                            }
                    }
                }
                return target_dict;
            }
            #endregion

            #region Class & Struct Deserialize
            else if (type.IsClass || type.IsValueType)
            {
                FieldInfo[] fields = type.GetFields(_flags);
                for (int i = 0; i < diff_count; ++i)
                {
                    int field_index = (int)Deserialize(reader);
                    var fi = fields[field_index];

                    fi.SetValue(target, DiffDeserialize(reader, fi.GetValue(target)));
                }
                return target;
            }
            #endregion

            return null;
        }

        public static object DiffDeserialize(Stream stream, object target)
        {
            return DiffDeserialize(new BinaryReader(stream), target);
        }

        public static void DiffDeserialize<T>(BinaryReader reader, ref T target)
        {
            target = (T)DiffDeserialize(reader, target);
        }

        public static void DiffDeserialize<T>(Stream stream, ref T target)
        {
            target = (T)DiffDeserialize(new BinaryReader(stream), target);
        }
        #endregion

        #region [Variable Replication] [Fast] DiffSerialize (object, fast, 추상 타입 사용 불가, null 불가)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static int FastDiffSerialize(ref List<object> obj_list, object current_value, object prev_value, Type type, int parnet_field_index = -1)
        {
            var fields = type.GetFields(_flags);
            int fields_length = fields.Length;

            if (current_value == null)
                throw new Exception("JNewSerialization::FastDiffSerialize - current_value is null");

            if (parnet_field_index != -1)
                obj_list.Add(parnet_field_index);

            int diff_count_index = obj_list.Count;
            int diff_count = 0;
            obj_list.Add(0);

            for (int field_index = 0; field_index < fields_length; ++field_index)
            {
                var curr_fieldValue = fields[field_index].GetValue(current_value);
                var prev_fieldValue = fields[field_index].GetValue(prev_value);
                var field_type = fields[field_index].FieldType;

                #region Native Data Serialize
                if (JReflection.HasWriteMethod(field_type) || field_type.IsEnum)
                {
                    if (curr_fieldValue == null)
                        throw new Exception("JNewSerialization::FastDiffSerialize - curr_fieldValue is null");

                    if (curr_fieldValue.Equals(prev_fieldValue) == false)
                    {
                        obj_list.Add(field_index);
                        obj_list.Add(curr_fieldValue);

                        diff_count++;
                    }
                }
                #endregion

                #region List Serialize
                // 추가, 삭제의 구분이 난해하므로, 변조에 대해서만 처리.
                // 추가/삭제 시 모든 데이터 전송.
                else if (typeof(IList).IsAssignableFrom(field_type))
                {
                    int init_field_index = field_index;
                    var list_itemType = (field_type.IsArray == true) ? field_type.GetElementType() : field_type.GetGenericArguments()[0];

                    var curr_list = curr_fieldValue as IList;

                    int count = curr_list.Count;
                    if ((prev_fieldValue is IList prev_list) && (prev_list.Count == count))
                    {
                        obj_list.Add(field_index);

                        int list_diff_count_index = obj_list.Count;
                        int list_diff_count = 0;
                        obj_list.Add(0);

                        if (JReflection.HasWriteMethod(list_itemType))
                        {
                            for (int i = 0; i < count; ++i)
                            {
                                if (curr_list[i] == null)
                                    throw new Exception("JNewSerialization::FastDiffSerialize - curr_list[i] is null");

                                if (curr_list[i].Equals(prev_list[i]) == false)
                                {
                                    obj_list.Add(i);
                                    obj_list.Add(curr_list[i]);

                                    list_diff_count++;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < count; ++i)
                            {
                                if (FastDiffSerialize(ref obj_list, curr_list[i], prev_list[i], list_itemType, i) != 0)
                                    list_diff_count++;
                            }
                        }

                        if (list_diff_count > 0)
                        {
                            obj_list[list_diff_count_index] = list_diff_count;
                            diff_count++;
                        }
                        else
                        {
                            obj_list.RemoveAt(list_diff_count_index);
                            obj_list.RemoveAt(list_diff_count_index - 1);
                        }
                    }
                    else
                    {
                        obj_list.Add(field_index);
                        obj_list.Add(-1);
                        obj_list.Add(count);

                        for (int i = 0; i < count; ++i)
                        {
                            if (curr_list[i] == null)
                                throw new Exception("JNewSerialization::FastDiffSerialize - curr_list[i] is null");

                            obj_list.Add(curr_list[i]);
                        }

                        diff_count++;
                    }
                }
                #endregion

                #region Dictionary Serialize
                else if (typeof(IDictionary).IsAssignableFrom(field_type))
                {
                    var curr_dict = curr_fieldValue as IDictionary;
                    var prev_dict = prev_fieldValue as IDictionary;

                    obj_list.Add(field_index);

                    int dict_diff_count_index = obj_list.Count;
                    int dict_diff_count = 0;
                    obj_list.Add(0);

                    var value_type = field_type.GetGenericArguments()[1];

                    if (JReflection.HasWriteMethod(value_type))
                    {
                        foreach (var key in curr_dict.Keys)
                        {
                            if (prev_dict.Contains(key))
                            {
                                if (curr_dict[key] == null)
                                    throw new Exception("JNewSerialization::FastDiffSerialize - curr_dict[key] is null");

                                int flag_index = obj_list.Count;
                                obj_list.Add(0); // float Flag

                                if (curr_dict[key].Equals(prev_dict[key]) == false)
                                {
                                    obj_list.Add(key);
                                    obj_list.Add(curr_dict[key]);

                                    dict_diff_count++;
                                }
                                else
                                {
                                    obj_list.RemoveAt(flag_index);
                                }
                                prev_dict.Remove(key);
                            }
                            else
                            {
                                obj_list.Add(1); // Add Flag

                                obj_list.Add(key);
                                obj_list.Add(curr_dict[key]);

                                dict_diff_count++;
                            }
                        }

                        foreach (var key in prev_dict.Keys)
                        {
                            obj_list.Add(2); // Delete Flag
                            obj_list.Add(key);
                            dict_diff_count++;
                        }
                    }
                    else
                    {
                        foreach (var key in curr_dict.Keys)
                        {
                            if (prev_dict.Contains(key))
                            {
                                int flag_index = obj_list.Count;
                                obj_list.Add(0); // float Flag
                                obj_list.Add(key);

                                if (FastDiffSerialize(ref obj_list, curr_dict[key], prev_dict[key], value_type, -1) != 0)
                                {
                                    dict_diff_count++;
                                }
                                else
                                {
                                    obj_list.RemoveAt(flag_index + 1);
                                    obj_list.RemoveAt(flag_index);
                                }
                                prev_dict.Remove(key);
                            }
                            else
                            {
                                obj_list.Add(1); // Add Flag

                                obj_list.Add(key);
                                obj_list.Add(curr_dict[key]);

                                dict_diff_count++;
                            }
                        }

                        foreach (var key in prev_dict.Keys)
                        {
                            obj_list.Add(2); // Delete Flag
                            obj_list.Add(key);
                            dict_diff_count++;
                        }
                    }


                    if (dict_diff_count > 0)
                    {
                        obj_list[dict_diff_count_index] = dict_diff_count;
                        diff_count++;
                    }
                    else
                    {
                        obj_list.RemoveAt(dict_diff_count_index);
                        obj_list.RemoveAt(dict_diff_count_index - 1);
                    }
                }
                #endregion

                #region Class & Struct Serialize
                else if (FastDiffSerialize(ref obj_list, curr_fieldValue, prev_fieldValue, field_type, field_index) != 0)
                    diff_count++;

                fields[field_index].SetValue(prev_value, curr_fieldValue);
                #endregion
            }


            if (diff_count > 0)
            {
                obj_list[diff_count_index] = diff_count;
            }
            else
            {
                obj_list.RemoveAt(diff_count_index);
                if (parnet_field_index >= 0)
                    obj_list.RemoveAt(diff_count_index - 1);
            }

            return diff_count;
        }

        public static int FastDiffSerialize<T>(BinaryWriter writer, T current_value, ref T prev_value)
        {
            Type type = typeof(T);
            if (JReflection.HasWriteMethod(type) || type.IsEnum)
            {
                if (current_value.Equals(prev_value) == false)
                {
                    prev_value = current_value;
                    FastSerialize(writer, current_value);
                    return 1;
                }

                return 0;
            }

            List<object> obj_list = new List<object>();
            FastDiffSerialize(ref obj_list, current_value, prev_value, type);

            foreach (var item in obj_list)
                FastSerialize(writer, item);

            return obj_list.Count;
        }
        public static int FastDiffSerialize<T>(Stream stream, T current_value, ref T prev_value)
        {
            return FastDiffSerialize<T>(new BinaryWriter(stream), current_value, ref prev_value);
        }
        #endregion

        #region [Variable Replication] [Fast] DiffDeserialize (object, fast, 추상 타입 사용 불가, null 불가)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void FastDiffDeserialize<T>(BinaryReader reader, Type type, ref T target)
        {
            if (JReflection.HasReadMethod(type))
            {
                target = (T)FastDeserialize(reader, type);
                return;
            }

            var fields = type.GetFields(_flags);
            int diff_count = (int)FastDeserialize(reader, typeof(int));

            for (int i = 0; i < diff_count; ++i)
            {
                int field_index = (int)FastDeserialize(reader, typeof(int));

                var fi = fields[field_index];
                var field_type = fi.FieldType;

                #region Native Data Deserialize
                if (JReflection.HasReadMethod(field_type) || field_type.IsEnum)
                {
                    fi.SetValue(target, FastDeserialize(reader, field_type));
                    continue;
                }
                #endregion

                #region List & Array Deserialize
                if (typeof(IList).IsAssignableFrom(field_type))
                {
                    int list_diff_count = (int)FastDeserialize(reader, typeof(int));

                    var list_ref_target = fi.GetValue(target);
                    var target_list = list_ref_target as IList;
                    var list_itemType = (field_type.IsArray == true) ? field_type.GetElementType() : field_type.GetGenericArguments()[0];

                    if (list_diff_count != -1)
                    {
                        if (JReflection.HasReadMethod(field_type))
                        {
                            for (int li = 0; li < list_diff_count; ++li)
                            {
                                int list_index = (int)FastDeserialize(reader, typeof(int));
                                target_list[list_index] = FastDeserialize(reader, list_itemType);
                            }
                        }
                        else
                        {
                            for (int li = 0; li < list_diff_count; ++li)
                            {
                                int list_index = (int)FastDeserialize(reader, typeof(int));
                                object read_object = target_list[list_index];
                                FastDiffDeserialize(reader, list_itemType, ref read_object);
                                target_list[list_index] = read_object;
                            }
                        }
                    }
                    else
                    {
                        target_list.Clear();

                        int list_count = (int)FastDeserialize(reader, typeof(int));
                        for (int li = 0; li < list_count; ++li)
                        {
                            target_list.Add(FastDeserialize(reader, list_itemType));
                        }
                    }

                    continue;
                }
                #endregion

                #region Dictionary Deserialize
                if (typeof(IDictionary).IsAssignableFrom(field_type))
                {
                    int dict_diff_count = (int)FastDeserialize(reader, typeof(int));

                    var dict_ref_target = fi.GetValue(target);
                    var target_dict = dict_ref_target as IDictionary;

                    var key_type = field_type.GetGenericArguments()[0];
                    var value_type = field_type.GetGenericArguments()[1];

                    if (JReflection.HasWriteMethod(value_type))
                    {
                        for (int count = 0; count < dict_diff_count; ++count)
                        {
                            switch ((int)FastDeserialize(reader, typeof(int)))
                            {
                                case 0: // float
                                    {
                                        target_dict[FastDeserialize(reader, key_type)] = FastDeserialize(reader, value_type);
                                    }
                                    break;
                                case 1: // Add
                                    {
                                        var key = FastDeserialize(reader, key_type);
                                        var value = FastDeserialize(reader, value_type);
                                        if (target_dict.Contains(key) == true)
                                            target_dict[key] = value;
                                        else
                                            target_dict.Add(key, value);
                                    }
                                    break;

                                case 2: // Delete
                                    {
                                        target_dict.Remove(FastDeserialize(reader, key_type));
                                    }
                                    break;

                                default:
                                    {
                                        throw new Exception("JNewSerialization::FastDiffDeserialize - IDictionary Flag Error");
                                    }
                            }
                        }
                    }
                    else
                    {
                        for (int count = 0; count < dict_diff_count; ++count)
                        {
                            switch ((int)FastDeserialize(reader, typeof(int)))
                            {
                                case 0: // float
                                    {
                                        var key = FastDeserialize(reader, key_type);
                                        var value = target_dict[key];
                                        FastDiffDeserialize(reader, value_type, ref value);
                                        target_dict[key] = value;
                                    }
                                    break;
                                case 1: // Add
                                    {
                                        var key = FastDeserialize(reader, key_type);
                                        var value = FastDeserialize(reader, value_type);
                                        if (target_dict.Contains(key) == true)
                                            target_dict[key] = value;
                                        else
                                            target_dict.Add(key, value);
                                    }
                                    break;

                                case 2: // Delete
                                    {
                                        target_dict.Remove(FastDeserialize(reader, key_type));
                                    }
                                    break;

                                default:
                                    {
                                        throw new Exception("JNewSerialization::FastDiffDeserialize - IDictionary Flag Error");
                                    }
                            }
                        }
                    }

                    continue;
                }
                #endregion

                #region Class & Struct Deserialize
                object ref_target = fi.GetValue(target);
                FastDiffDeserialize(reader, field_type, ref ref_target);
                fi.SetValue(target, ref_target);
                #endregion
            }
        }

        public static void FastDiffDeserialize<T>(Stream stream, Type type, ref T target)
        {
            FastDiffDeserialize(new BinaryReader(stream), type, ref target);
        }

        public static void FastDiffDeserialize<T>(BinaryReader reader, ref T target)
        {
            FastDiffDeserialize(reader, typeof(T), ref target);
        }

        public static void FastDiffDeserialize<T>(Stream stream, ref T target)
        {
            FastDiffDeserialize(new BinaryReader(stream), typeof(T), ref target);
        }
        #endregion

    }
}
