using CLIInterface.Math3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if NET_SERVER
	using Newtonsoft.Json;
#endif


namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JUtil
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static partial class JUtil
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Base
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [로그] Write
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void WriteLog(string log, params object[] args)
        {
            Console.WriteLine(string.Format(log, args));
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Log(string text, Vector3 savePosition)
        {
            Console.WriteLine(string.Format("{0}({1:0.0}, {2:0.0}, {3:0.0})", text, savePosition.x, savePosition.y, savePosition.z));
        }
        #endregion

        #region [유니크 ID]
        private static long s_uniqueIndex = 1000;

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static long CreateUniqueId()
        {
            return ++s_uniqueIndex;
        }
        #endregion

        #region [유틸] CreatePropertyInstance
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        static public object CreatePropertyInstance(Type type, string property_name)
        {
            while (type != null)
            {
                var prop_type_info = type.GetProperty(property_name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (prop_type_info == null)
                {
                    type = type.BaseType;
                    continue;
                }
                var make_type = prop_type_info.PropertyType;
                if (make_type.IsAbstract)
                    return null;

                return Activator.CreateInstance(make_type);
            }
            return null;
        }
        #endregion

        #region [유틸] CloneProcedure
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T CloneProcedure<T>(T obj)
        {
            if (obj == null)
                return default(T);

            var type = obj.GetType();

            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                return obj;
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                IList list = (IList)obj;
                var list_count = list.Count;

                if (type.IsArray)
                {
                    IList copiedArray = Array.CreateInstance(type.GetElementType(), list_count);
                    for (int i = 0; i < list_count; ++i)
                        copiedArray[i] = CloneProcedure(list[i]);

                    return (T)copiedArray;
                }
                else
                {
                    IList copiedList = (IList)Activator.CreateInstance(type);
                    for (int i = 0; i < list_count; ++i)
                        copiedList.Add(CloneProcedure(list[i]));

                    return (T)copiedList;
                }
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                IDictionary dict = (IDictionary)obj;
                IDictionary copiedDict = (IDictionary)Activator.CreateInstance(type);

                foreach (var key in dict.Keys)
                    copiedDict.Add(key, CloneProcedure(dict[key]));

                return (T)copiedDict;
            }
            //else if (typeof(Tuple).IsAssignableFrom(type))
            //{
            //    Tuple tuple = (Tuple)obj;
            //    var tuple_length = tuple.Length;
            //    var args = new object[tuple_length];

            //    for (int i = 0; i < tuple_length; ++i)
            //        args[i] = CloneProcedure(tuple[i]);

            //    return (T)Activator.CreateInstance(type, args);
            //}
            else if (typeof(ICloneable).IsAssignableFrom(type))
            {
                ICloneable cloneObj = (ICloneable)obj;
                return (T)cloneObj.Clone();
            }
            else if (type.IsClass || type.IsValueType)
            {
                object copiedObject = Activator.CreateInstance(type);

                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue != null)
                        field.SetValue(copiedObject, fieldValue); // warning! : shallow copy
                }
                return (T)copiedObject;
            }

            return default(T);
        }
        #endregion

        #region [유틸] DataCopy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void DataCopy<T>(ref T copiedObject, object obj)
        {
            if (obj == null)
                return;

            var type = copiedObject.GetType();
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                var target = copiedObject as object;
                target = obj;
                return;
            }
            else if (type.IsArray)
            {
                Type typeElement = Type.GetType(type.FullName.Replace("[]", string.Empty));
                var array = obj as Array;
                var copiedArray = copiedObject as Array;
                for (int i = 0; i < array.Length; i++)
                {
                    copiedArray.SetValue(CloneProcedure(array.GetValue(i)), i);
                }
            }
            else if (type.IsClass || type.IsValueType)
            {
                var objType = obj.GetType();
                if (objType.IsAssignableFrom(type) == false)
                    return;

                FieldInfo[] fields = objType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue != null)
                    {
                        field.SetValue(copiedObject, CloneProcedure(fieldValue));
                    }
                }
            }
        }
        public static void DataCopy(object copiedObject, object obj) // for class
        {
            if (obj == null)
                return;

            var type = copiedObject.GetType();
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                var target = copiedObject as object;
                target = obj;
                return;
            }
            else if (type.IsArray)
            {
                Type typeElement = Type.GetType(type.FullName.Replace("[]", string.Empty));
                var array = obj as Array;
                var copiedArray = copiedObject as Array;
                for (int i = 0; i < array.Length; i++)
                {
                    copiedArray.SetValue(CloneProcedure(array.GetValue(i)), i);
                }
            }
            else if (type.IsClass || type.IsValueType)
            {
                var objType = obj.GetType();
                if (objType.IsAssignableFrom(type) == false)
                    return;

                FieldInfo[] fields = objType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue != null)
                    {
                        field.SetValue(copiedObject, CloneProcedure(fieldValue));
                    }
                }
            }
        }
        #endregion

        #region [String To int] 문자열 해시
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int StringHash(string str)
        {
            int result = 0;
            int p_pow = 1;

            foreach (char c in str)
            {
                result = (result + (c - 'a' + 1) * p_pow) % 1000000009;
                p_pow = (31 * p_pow) % 1000000009;
            }

            return result;
        }
        #endregion

        #region [메모리] Memset
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Memset(int[] target, int value)
        {
            Memset(target, value, target.Length);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Memset(int[] target, int value, int count)
        {
            int i;
            int blockSize = Math.Min(1024, count);

            for (i = 0; i < blockSize; i++)
                target[i] = value;

            if (i == count)
                return;

            int bytesCount = blockSize * sizeof(int);

            while (i + blockSize < count)
            {
                Buffer.BlockCopy(target, 0, target, i, bytesCount);
                i += blockSize;
            }

            bytesCount = (count - i) * sizeof(int);
            Buffer.BlockCopy(target, 0, target, i, bytesCount);
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Type
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Type] IsSubclassOf
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsSubclassOf(object obj, Type parent_type)
        {
            var type = obj.GetType();
            return type.Equals(parent_type) || type.IsSubclassOf(parent_type);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsSubclassOf<T>(object obj) where T : class
        {
            return IsSubclassOf(obj, typeof(T));
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsSameType(object obj1, object obj2)
        {
            return IsSameType(obj1.GetType(), obj1.GetType());
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsSameType(Type t1, Type t2)
        {
            return t1.IsAssignableFrom(t2) || t2.IsAssignableFrom(t1);
        }
        #endregion

        #region [Type] CollectionType
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type GetCollectionType(Type type)
        {
            if (type.IsGenericType)
            {
                Type[] types = type.GetGenericArguments();
                if (types.Length == 1)
                {
                    return types[0];
                }
                else
                {
                    // Could be null if implements two IEnumerable
                    return type.GetInterfaces().Where(t => t.IsGenericType)
                      .Where(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                      .SingleOrDefault().GetGenericArguments()[0];
                }
            }
            else if (type.IsArray)
            {
                return type.GetElementType();
            }
            // TODO: Who knows, but its probably not suitable to render in a table
            return null;
        }
        #endregion

        #region [Type] <-> string
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type GetType(string type_name)
        {
            if (string.IsNullOrEmpty(type_name))
            {
                JLogger.WriteError("[ERROR] type_name is null.");
                return null;
            }

#if NET_SERVER
			if (type_name.Contains("UnityEngine"))
				type_name = type_name.Replace("UnityEngine", "J2y");
#endif

            var type = Type.GetType(type_name);

            if (type == null)
                type = FindType(type_name);
            return type;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string TypeToString(Type type)
        {
            var type_name = type.ToString();
#if !NET_SERVER
            if (type_name.Contains("J2y.GameObject") || type_name.Contains("J2y.MonoBehaviour") ||
                type_name.Contains("J2y.Component") || type_name.Contains("J2y.Transform"))
            {
                type_name = type_name.Replace("J2y", "UnityEngine");
            }
#endif
            return type_name;
        }
        #endregion

        #region [Type] TryGetType
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type OldGetType(string typeName)
        {
            var getType = Type.GetType(typeName);

            if (getType == null)
                getType = FindType(typeName);

            return getType;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type FindType(string typeName)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var getType = a.GetType(typeName);
                if (getType != null)
                    return getType;
            }
            return null;
        }
        #endregion

        #region [Type] MakeGenericTypeName
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type MakeGenericTypeName(Type type)
        {
            return OldGetType(type.Namespace + '.' + type.Name);
        }
        #endregion

        #region [Type] GlobalType (assemblies 고려)

        private static Dictionary<string, Type> s_TypeLookup = new Dictionary<string, Type>();
        private static List<string> s_LoadedAssemblies = null;

        /// <summary>
        /// Searches through all of the loaded assembies for the specified type.
        /// </summary>
        /// <param name="name">The string value of the type.</param>
        /// <returns>The found Type. Can be null.</returns>
        public static Type GetGlobalType(string name)
        {
            Type type;
            // Cache the results for quick repeated lookup.
            if (s_TypeLookup.TryGetValue(name, out type))
            {
                return type;
            }

            type = Type.GetType(name);
            // Look in the loaded assemblies.
            if (type == null)
            {
                if (s_LoadedAssemblies == null)
                {
#if NETFX_CORE && !UNITY_EDITOR
                    s_LoadedAssemblies = GetStorageFileAssemblies(typeName).Result;
#else
                    s_LoadedAssemblies = new List<string>();
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < assemblies.Length; ++i)
                    {
                        s_LoadedAssemblies.Add(assemblies[i].FullName);
                    }
#endif
                }
                // Continue until the type is found.
                for (int i = 0; i < s_LoadedAssemblies.Count; ++i)
                {
                    type = Type.GetType(name + "," + s_LoadedAssemblies[i]);
                    if (type != null)
                    {
                        break;
                    }
                }
            }
            if (type != null)
            {
                s_TypeLookup.Add(name, type);
            }
            return type;
        }
        #endregion

        #region NameToType
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type NameToType(string typeName)
        {
            var getType = Type.GetType(typeName);

            if (getType == null)
                getType = FindTypeByName(typeName);

            return getType;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type FindTypeByName(string typeName)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = a.GetTypes();
                foreach (var type in types)
                {
                    if (type.Name == typeName)
                        return type;
                }
            }
            return null;
        }
        #endregion

    }



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Helper
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [Helper] Lerp

    public class LerpHelper
    {
        private float _time;
        private float _prevValue;
        public float _target;

        public void SetTarget(float startValue, float target)
        {
            _prevValue = startValue;
            _target = target;
            _time = 0.0f;
        }

        public float Lerp(float dt)
        {
            _time = Mathf.Clamp01(_time + dt);
            Value = Mathf.Lerp(_prevValue, _target, _time);
            return Value;
        }

        public float Value { get; private set; }
    }
    #endregion

    #region [Helper] Timer
    public class TimerHelper
    {
        private float _time;
        private float _interval;

        public TimerHelper(float interval)
        {
            _interval = interval;
        }

        public bool Update(float dt)
        {
            _time += dt;
            if (_time > _interval)
            {
                _time = _time % _interval;
                return true;
            }

            return false;
        }
    }
    #endregion

    #region [유틸] 비트 플래그
    public static class JBitMask
    {
        public static bool Compare<T>(int flags, T flag) where T : struct
        {
            int flagValue = (int)(object)flag;

            return (flags == flagValue) || ((flags & flagValue) > 0);
        }

        public static bool IsSet<T>(int flags, T flag) where T : struct
        {
            int flagValue = 1 << (int)(object)flag;

            return (flags & flagValue) != 0;
        }

        public static void Set<T>(ref int flags, T flag) where T : struct
        {
            int flagValue = 1 << (int)(object)flag;

            flags = (int)(object)(flags | flagValue);
        }

        public static void Unset<T>(ref int flags, T flag) where T : struct
        {
            int flagValue = 1 << (int)(object)flag;

            flags = (int)(object)(flags & (~flagValue));
        }
    }
    #endregion





}
