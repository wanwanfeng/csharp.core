using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Library.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<object> GetEnumerator(this Enum type)
        {
            var list = Enum.GetValues(type.GetType());
            foreach (var it in list)
            {
                yield return it;
            }
        }

        public static Dictionary<object, string> ToStringDic(this Enum type)
        {
            return type.GetEnumerator().ToDictionary(p => p, q => q.ToString());
        }

        public static Dictionary<object, int> ToIntDic(this Enum type)
        {
            return type.GetEnumerator().ToDictionary(p => p, q => (int)q);
        }

        public static Dictionary<string, int> ToDictionary(this Enum type)
        {
            return type.GetEnumerator().ToDictionary(p => p.ToString(), q => (int)q);
        }

        #region enum

        public static IDictionary<object, T> GetCache<T>(this Enum type) where T : Attribute
        {
            return type.GetEnumerator().ToDictionary(p => p, q => type.GetType().GetField(q.ToString()).GetFirstCustomAttribute<T>());
        }

        public static IDictionary<T, Type> GetCacheTypeValue<T>(this Enum type) where T : struct
        {
            return type.GetCache<TypeValueAttribute>().ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.value : null);
        }

        public static IDictionary<T, TK> GetCacheDefaultValue<T, TK>(this Enum type) where T : struct
        {
            return type.GetCache<DefaultValueAttribute>().ToDictionary(k => (T)k.Key, v => v.Value != null ? (TK)v.Value.Value : default(TK));
        }

        public static IDictionary<T, string> GetCacheDescription<T>(this Enum type) where T : struct
        {
            return type.GetCache<DescriptionAttribute>().ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.Description : "");
        }

        public static IDictionary<T, string> GetCacheStringValue<T>(this Enum type) where T : struct
        {
            return type.GetCache<StringValueAttribute>().ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.value : "");
        }

        public static IDictionary<T, string> GetCacheHookValue<T>(this Enum type) where T : struct
        {
            return type.GetCache<HookValueAttribute>().ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.value : "");
        }

        public static IDictionary<T, int> GetCacheIntValue<T>(this Enum type) where T : struct
        {
            return type.GetCache<IntValueAttribute>().ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.value : 0);
        }

        #endregion

    }
}