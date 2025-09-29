using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // JUtil. Extensions
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [Extensions] Dictionary
    public static class DictionaryExtensions
    {
        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            Func<TValue, bool> predicate)
        {
            var keys = dic.Keys.Where(k => predicate(dic[k])).ToList();
            foreach (var key in keys)
            {
                dic.Remove(key);
            }
        }
    }
    #endregion

    #region [Extensions] List
    public static class ListExtensions
    {
        //    list: List<T> to resize
        //    size: desired new size
        // element: default value to insert

        public static void Resize<T>(this List<T> list, int size, T element = default(T))
        {
            int count = list.Count;

            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > count)
            {
                if (size > list.Capacity)   // Optimization
                    list.Capacity = size;

                list.AddRange(Enumerable.Repeat(element, size - count));
            }
        }
    }
    #endregion

    #region [Interface] Prototype
    public abstract class Prototype
    {
        public abstract Prototype Clone();
    }
    #endregion

    #region [Extensions] Array
    public static class ArrayExtensions
    {
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }
    }
    #endregion

    #region [Extensions] Type
    public static class TypeExtensions
    {
        public static object GetValue(this MemberInfo member, object value)
        {
            if (member is FieldInfo)
                return (member as FieldInfo).GetValue(value);
            if (member is PropertyInfo)
                return (member as PropertyInfo).GetValue(value);
            return null;
        }
        public static Type GetMemberType(this MemberInfo member)
        {
            if (member is FieldInfo)
                return (member as FieldInfo).FieldType;
            if (member is PropertyInfo)
                return (member as PropertyInfo).PropertyType;
            return null;
        }

        #region [Type] Get/Has
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes().Any(attr => attr is T);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes().FirstOrDefault(attr => attr is T) as T;
        }

        #endregion

    }
    #endregion

    public static class AwaitExtensions
    {
        public static async void WrapErrors(this Task task)
        {
            await task;
        }
    }


}

#region [Extensions] Linq
public static class LinqExtensions
{
    public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
    {
        foreach (var x in @this)
            action(x);
    }

    public static T MinBy<T, TProp>(this IEnumerable<T> source, Func<T, TProp> propSelector)
    {
        return source.OrderBy(propSelector).FirstOrDefault();
    }

    public static T MaxBy<T, TProp>(this IEnumerable<T> source, Func<T, TProp> propSelector)
    {
        return source.OrderBy(propSelector).LastOrDefault();
    }
}
#endregion