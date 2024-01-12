using FastMember;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Imato.Reflection
{
    public static class Objects
    {
        private static ConcurrentDictionary<string, Accessor> _accessors
            = new ConcurrentDictionary<string, Accessor>();

        private static Accessor GetAccessor(Type type,
            string[]? skipFields = null)
        {
            return _accessors.GetOrAdd(type.Name, (_) =>
            {
                var accessor = new Accessor
                {
                    Name = type.Name,
                    TypeAccessor = TypeAccessor.Create(type)
                };
                accessor.Members = accessor.TypeAccessor
                    .GetMembers()
                    .Where(m => m.CanRead
                        && (skipFields == null || !skipFields.Contains(m.Name)))
                    .ToArray();
                return accessor;
            });
        }

        /// <summary>
        /// Get object fields dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object</param>
        /// <param name="skipFields">List on fields </param>
        /// <param name="skipChildren">Skip children object</param>
        /// <returns></returns>
        public static IDictionary<string, object?> GetFields<T>(this T obj,
            string[]? skipFields = null,
            bool skipChildren = false,
            string[]? fields = null)
        {
            return GetFieldsList(obj, skipFields, fields, skipChildren);
        }

        private static IDictionary<string, object?> GetFieldsList(object? obj,
            Type type,
            string[]? skipFields,
            string[]? fields,
            bool skipChildren,
            int level = 0,
            string fieldName = "")
        {
            var dic = new Dictionary<string, object?>();
            var done = false;

            if (obj == null)
            {
                return dic;
            }

            if (type.GetInterfaces().Any(x => x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var id = 1;
                foreach (var item in (IEnumerable)obj)
                {
                    var name = $"{fieldName}[{id}]";
                    foreach (var d in GetFieldsList(item, item.GetType(), skipFields, fields, skipChildren, level, name))
                    {
                        AddToDictionary(dic, d.Key, d.Value, skipFields, fields);
                    }
                    id++;
                }
                return dic;
            }

            var accessor = GetAccessor(type, skipFields);
            foreach (var m in accessor.Members)
            {
                done = false;

                var name = fieldName == "" ? m.Name : $"{fieldName}.{m.Name}";
                var value = accessor.TypeAccessor[obj, m.Name];
                if (m.Type.IsValueType || m.Type == typeof(string) || value is null)
                {
                    AddToDictionary(dic, name, value, skipFields, fields);
                    done = true;
                }

                if (!done && level < 10 && !skipChildren)
                {
                    foreach (var n in GetFieldsList(value, m.Type, skipFields, fields, false, level++, name))
                    {
                        AddToDictionary(dic, n.Key, n.Value, skipFields, fields);
                    }
                }
            }
            return dic;
        }

        public static object? GetField<T>(T obj, string fieldName)
        {
            var accessor = GetAccessor(typeof(T));
            var m = accessor.Members
                .Where(x => x.Name.ToUpper() == fieldName.ToUpper())
                .FirstOrDefault();
            if (m != null)
            {
                return accessor.TypeAccessor[obj, m.Name];
            }

            return null;
        }

        /// <summary>
        /// Return public fields of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="skipFields"></param>
        /// <param name="skipChildren"></param>
        /// <returns></returns>

        public static IEnumerable<string> GetFieldNames<T>(
            string[]? skipFields = null,
            bool skipChildren = false)
        {
            return GetFieldNames(typeof(T), skipFields, skipChildren);
        }

        private static IEnumerable<string> GetFieldNames(Type type,
            string[]? skipFields = null,
            bool skipChildren = false,
            int level = 0,
            string fieldName = "")
        {
            var list = new List<string>();
            var done = false;

            var accessor = GetAccessor(type, skipFields);
            foreach (var m in accessor.Members)
            {
                done = false;

                var name = fieldName == "" ? m.Name : $"{fieldName}.{m.Name}";

                if (m.Type.IsValueType || m.Type == typeof(string))
                {
                    AddToList(list, name, skipFields);
                    done = true;
                }

                if (!done && level < 10 && !skipChildren)
                {
                    foreach (var n in GetFieldNames(m.Type, skipFields, false, level++, name))
                    {
                        AddToList(list, n, skipFields);
                    }
                }
            }
            return list;
        }

        private static void AddToDictionary(Dictionary<string, object?> dic,
            string key,
            object? value = null,
            string[]? skipFields = null,
            string[]? fields = null)
        {
            if ((skipFields == null || !skipFields.Any(x => x.Equals(key, StringComparison.InvariantCultureIgnoreCase)))
                && (fields == null || fields.Any(x => x.Equals(key, StringComparison.InvariantCultureIgnoreCase)))
                && !dic.ContainsKey(key))
            {
                dic.Add(key, value);
            }
        }

        private static void AddToList(IList<string> list,
            string value,
            string[]? skipFields = null)
        {
            if ((skipFields == null || !skipFields.Contains(value)) && !list.Contains(value))
            {
                list.Add(value);
            }
        }

        private static IDictionary<string, object?> GetFieldsList<T>(T obj, string[]? skipFields, string[]? fields, bool skipChildren)
        {
            var type = typeof(T);
            return GetFieldsList(obj, type, skipFields, fields, skipChildren);
        }

        /// <summary>
        /// Get object differences as dictionary of fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="skipFields"></param>
        /// <returns></returns>
        public static IDictionary<string, object?> GetDiff<T>(this T obj1, T obj2, string[]? skipFields = null, bool skipChildren = false)
        {
            var dic1 = obj1.GetFields(skipFields, skipChildren);
            var dic2 = obj2.GetFields(skipFields, skipChildren);
            var dif = new Dictionary<string, object?>();
            foreach (var d in dic1)
            {
                if (d.Value != null
                    && dic2.ContainsKey(d.Key)
                    && dic2[d.Key] != null
                    && !d.Value.Equals(dic2[d.Key]))
                {
                    dif.Add(d.Key, d.Value);
                }
                if (d.Value is null && dic2.ContainsKey(d.Key) && dic2[d.Key] != null)
                {
                    dif.Add(d.Key, null);
                }
                if (d.Value != null && (!dic2.ContainsKey(d.Key) || dic2[d.Key] is null))
                {
                    dif.Add(d.Key, d.Value);
                }
            }

            foreach (var d in dic2)
            {
                if (d.Value != null
                    && !dic1.ContainsKey(d.Key))
                {
                    dif.Add(d.Key, null);
                }
            }

            return dif;
        }

        public static bool IsEqual<T>(this T obj1, T obj2, string[]? skipFields = null, bool skipChildren = false)
        {
            var diff = GetDiff(obj1, obj2, skipFields, skipChildren);
            return diff.Count == 0;
        }
    }
}