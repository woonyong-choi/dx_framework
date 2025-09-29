using J2y.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JReplication
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static class JReflection
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] ReadMethods, WriteMethods
        public static Dictionary<Type, MethodInfo> s_read_methods;
        public static Dictionary<Type, MethodInfo> s_write_methods;
        #endregion

        #region [변수] Type -> Methods
        private static Dictionary<Type, Dictionary<string, MethodInfo>> s_method_infos = new Dictionary<Type, Dictionary<string, MethodInfo>>();
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Properties
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] ReadMethods, WriteMethods
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Dictionary<Type, MethodInfo> ReadMethods { get { return s_read_methods; } }
        public static Dictionary<Type, MethodInfo> WriteMethods { get { return s_write_methods; } }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //  [JReflection]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Static] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        static JReflection()
        {
            load_readwrite_methods();
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Reflection] Classes
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Class] Find All Classes
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type[] FindAllClasses()
        {
            var mscorlib = typeof(JActor).Assembly;
            var types = mscorlib.GetTypes();
            return types;
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //  [RPC] [Method]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RPC] Method 찾기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MethodInfo FindMethod(Type tp, string fun_name)
        {
            // 1. 클래스 타입 체크
            if (!s_method_infos.ContainsKey(tp))
                s_method_infos[tp] = new Dictionary<string, MethodInfo>();



            // 2. 함수 목록 체크
            var this_methods = s_method_infos[tp];
            if (!this_methods.ContainsKey(fun_name))
            {
                var method = tp.GetMethod(fun_name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (null == method)
                    return null;
                this_methods[fun_name] = method;
                return method;
            }

            return this_methods[fun_name];
        }
        #endregion

        #region [RPC] 현재 Method 이름
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            var st = new StackTrace(new StackFrame(2));
            var m1 = st.GetFrame(0).GetMethod();

            return st.GetFrame(0).GetMethod().Name;
        }
        #endregion

        #region [RPC] Method 호출
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object InvokeMethod(object obj, string methodName, params object[] argus)
        {
            var method = FindMethod(obj.GetType(), methodName);
            if (method != null)
                return method.Invoke(obj, argus);
            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool HasMethod(object obj, string methodName)
        {
            return FindMethod(obj.GetType(), methodName) != null;
        }
        #endregion

        #region [Reflection] Static Method
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MethodInfo FindStaticMethod(Type type, string method_name)
        {
            return type.GetMethod(method_name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool HasStaticMethod(Type type, string method_name)
        {
            return FindStaticMethod(type, method_name) != null;
        }
        #endregion

        #region [Reflection] Method Parameter Types
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type[] GetMethodParameterTypes(Type tp, string fun_name)
        {
            var mi = FindMethod(tp, fun_name);
            if (null == mi)
                return null;

            return GetMethodParameterTypes(mi);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type[] GetMethodParameterTypes(MethodInfo mi)
        {
            if (null == mi)
                return null;
            var param_infos = mi.GetParameters();
            return param_infos.Select(pi => pi.ParameterType).ToArray();
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Reflection] Fields/Properties
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Fields] FindAllFields
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static FieldInfo[] FindAllFields(object target)
        {
            var tp = target.GetType();
            var fields = tp.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return fields;
        }
        #endregion

        #region [Properties] FindAllProperties
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static PropertyInfo[] FindAllProperties(object target)
        {
            var tp = target.GetType();
            var properties = tp.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return properties;
        }
        #endregion

        #region [Fields] FindAllMembers
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MemberInfo[] FindAllMembers(object target)
        {
            var tp = target.GetType();
            var members = tp.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return members;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MemberInfo[] FindAllStaticMembers(object target)
        {
            var tp = target.GetType();
            var members = tp.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            return members;
        }
        #endregion

        #region [Fields] All Fields/Properties
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static IEnumerable<MemberInfo> FindAllMembers(object target, bool fields, bool properties, bool methods)
        {
            var members = FindAllMembers(target)
                .Where(m =>
                    (fields && (m is FieldInfo)) ||
                    (properties && (m is PropertyInfo)) ||
                    (methods && (m is MethodInfo)));
            return members;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static IEnumerable<MemberInfo> FindAllStaticMembers(object target, bool fields, bool properties, bool methods)
        {
            var members = FindAllStaticMembers(target)
                .Where(m =>
                    (fields && (m is FieldInfo)) ||
                    (properties && (m is PropertyInfo)) ||
                    (methods && (m is MethodInfo)));
            return members;
        }
        #endregion


        #region [Properties/Field] Get/Set
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object GetPropertyValue2(object target, string prop_name, object default_value = default(object))
        {
            if (null == target)
                return default_value;
            var tp = target.GetType();
            var property = tp.GetProperty(prop_name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (property != null)
                return property.GetValue(target);
            return default_value;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T GetPropertyValue<T>(object target, string prop_name, T default_value = default(T))
        {
            return (T)GetPropertyValue2(target, prop_name, default_value);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SetPropertyValue(object target, string prop_name, object value)
        {
            var tp = target.GetType();
            var property = tp.GetProperty(prop_name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (property != null)
                property.SetValue(target, value);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object GetFieldValue2(object target, string fieldName, object default_value = default(object))
        {
            if (null == target)
                return default_value;
            var tp = target.GetType();
            var field = tp.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field != null)
                return field.GetValue(target);
            return default_value;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T GetFieldValue<T>(object target, string fieldName, T default_value = default(T))
        {
            return (T)GetFieldValue2(target, fieldName, default_value);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SetFieldValue(object target, string fieldName, object value)
        {
            var tp = target.GetType();
            var field = tp.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field != null)
                field.SetValue(target, value);
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Reflection] Read/Write Functions
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Reflection] Read/Write 함수들 추출
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static void load_readwrite_methods()
        {
            s_read_methods = new Dictionary<Type, MethodInfo>();
            var methods = typeof(BinaryReader).GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var mi in methods)
            {
                if (mi.GetParameters().Length == 0 && mi.Name.StartsWith("Read", StringComparison.InvariantCulture) && mi.Name.Substring(4) == mi.ReturnType.Name)
                {
                    s_read_methods[mi.ReturnType] = mi;
                }
            }

            s_write_methods = new Dictionary<Type, MethodInfo>();
            methods = typeof(BinaryWriter).GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var mi in methods)
            {
                if (mi.Name.Equals("Write", StringComparison.InvariantCulture))
                {
                    var pis = mi.GetParameters();
                    if (pis.Length == 1)
                        s_write_methods[pis[0].ParameterType] = mi;
                }
            }
        }
        #endregion

        #region [Reflection] [Stream] Read Value
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool HasReadMethod(Type tp)
        {
            return s_read_methods.ContainsKey(tp);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object Read(BinaryReader reader, Type tp)
        {
            if (s_read_methods.TryGetValue(tp, out MethodInfo readMethod))
            {
                return readMethod.Invoke(reader, null);
            }
            //value = default(object);
            return default(object);
        }
        #endregion

        #region [Reflection] [Stream] Write Value
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool HasWriteMethod(Type tp)
        {
            return s_write_methods.ContainsKey(tp);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool Write(BinaryWriter writer, Type tp, object value)
        {
            if (s_write_methods.TryGetValue(tp, out MethodInfo writeMethod))
            {
                writeMethod.Invoke(writer, new object[] { value });
                return true;
            }
            return false;
        }
        #endregion


        #region [Reflection] [Read] [Fields] BinaryReader -> All Fields
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ReadAllFields(BinaryReader reader, object target)
        {
            ReadAllFields(reader, target, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ReadAllFields(BinaryReader reader, object target, BindingFlags flags)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            var tp = target.GetType();

            var fields = tp.GetFields(flags);
            SortMembersList(fields);

            foreach (var fi in fields)
            {
                object value;

                // find read method
                if (s_read_methods.TryGetValue(fi.FieldType, out MethodInfo readMethod))
                {
                    // read value
                    value = readMethod.Invoke(reader, null);

                    // set the value
                    fi.SetValue(target, value);
                }
            }
        }
        #endregion

        #region [Reflection] [Read] [Properties] BinaryReader -> All Properties
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ReadAllProperties(BinaryReader reader, object target)
        {
            ReadAllProperties(reader, target, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ReadAllProperties(BinaryReader reader, object target, BindingFlags flags)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            var tp = target.GetType();

            var fields = tp.GetProperties(flags);
            SortMembersList(fields);
            foreach (var fi in fields)
            {
                object value;

                // find read method
                if (s_read_methods.TryGetValue(fi.PropertyType, out MethodInfo readMethod))
                {
                    // read value
                    value = readMethod.Invoke(reader, null);

                    // set the value
                    var setMethod = fi.GetSetMethod();
                    if (setMethod != null)
                        setMethod.Invoke(target, new object[] { value });
                }
            }
        }
        #endregion

        #region [Reflection] [Write] [Fields] All Fields -> BinaryWriter
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void WriteAllFields(BinaryWriter writer, object ob)
        {
            WriteAllFields(writer, ob, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void WriteAllFields(BinaryWriter writer, object ob, BindingFlags flags)
        {
            if (ob == null)
                return;
            var tp = ob.GetType();

            var fields = tp.GetFields(flags);
            SortMembersList(fields);

            foreach (var fi in fields)
            {
                object value = fi.GetValue(ob);

                // find the appropriate Write method
                if (s_write_methods.TryGetValue(fi.FieldType, out MethodInfo writeMethod))
                    writeMethod.Invoke(writer, new object[] { value });
                else
                    throw new JNetException("Failed to find write method for type " + fi.FieldType);
            }
        }
        #endregion

        #region [Reflection] [Write] [Properties] All Properties -> BinaryWriter
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void WriteAllProperties(BinaryWriter writer, object ob)
        {
            WriteAllProperties(writer, ob, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void WriteAllProperties(BinaryWriter writer, object ob, BindingFlags flags)
        {
            if (ob == null)
                return;
            var tp = ob.GetType();

            var fields = tp.GetProperties(flags);
            SortMembersList(fields);

            foreach (var fi in fields)
            {
                MethodInfo getMethod = fi.GetGetMethod();
                if (getMethod != null)
                {
                    object value = getMethod.Invoke(ob, null);

                    // find the appropriate Write method
                    if (s_write_methods.TryGetValue(fi.PropertyType, out MethodInfo writeMethod))
                        writeMethod.Invoke(writer, new object[] { value });
                }
            }
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [유틸] 
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유틸] SortMembersList
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // shell sort
        internal static void SortMembersList(System.Reflection.MemberInfo[] list)
        {
            int h;
            int j;
            System.Reflection.MemberInfo tmp;

            h = 1;
            while (h * 3 + 1 <= list.Length)
                h = 3 * h + 1;

            while (h > 0)
            {
                for (int i = h - 1; i < list.Length; i++)
                {
                    tmp = list[i];
                    j = i;
                    while (true)
                    {
                        if (j >= h)
                        {
                            if (string.Compare(list[j - h].Name, tmp.Name, StringComparison.InvariantCulture) > 0)
                            {
                                list[j] = list[j - h];
                                j -= h;
                            }
                            else
                                break;
                        }
                        else
                            break;
                    }

                    list[j] = tmp;
                }
                h /= 3;
            }
        }
        #endregion
    }

}


