using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Library.Extensions;

namespace Library.Helper
{
    public static class AttributeHelper
    {
        #region enum

        public static IDictionary<object, TA> GetCache<T, TA>() where TA : Attribute
        {
            IDictionary<object, TA> cache = new Dictionary<object, TA>();
            if (typeof (T).IsEnum)
            {
                foreach (var value in Enum.GetValues(typeof (T)))
                {
                    cache[value] = typeof (T).GetField(value.ToString()).GetFirstCustomAttribute<TA>();
                }
            }
            else
            {
                throw new Exception(string.Format("{0} type is not Enum !", typeof (T).Name));
            }
            return cache;
        }

        public static IDictionary<T, Type> GetCacheTypeValue<T>() where T : struct
        {
            return GetCache<T, TypeValueAttribute>()
                .ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.value : null);
        }

        public static IDictionary<T, TK> GetCacheDefaultValue<T, TK>() where T : struct
        {
            return GetCache<T, DefaultValueAttribute>()
                .ToDictionary(k => (T)k.Key, v => v.Value != null ? (TK)v.Value.Value : default(TK));
        }

        public static IDictionary<T, string> GetCacheDescription<T>() where T : struct
        {
            return GetCache<T, DescriptionAttribute>()
                .ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.Description : "");
        }

        public static IDictionary<T, string> GetCacheStringValue<T>() where T : struct
        {
            return GetCache<T, StringValueAttribute>()
                .ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.value : "");
        }

        public static IDictionary<T, string> GetCacheHookValue<T>() where T : struct
        {
            return GetCache<T, HookValueAttribute>()
                .ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.value : "");
        }

        public static IDictionary<T, int> GetCacheIntValue<T>() where T : struct
        {
            return GetCache<T, IntValueAttribute>()
                .ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.value : 0);
        }

        #endregion

        #region field

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">FieldInfo</typeparam>
        /// <typeparam name="TA">Attribute</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        private static IDictionary<T, TA> GetCache<T, TA>(List<T> list) where TA : Attribute
        {
            IDictionary<T, TA> cache = new Dictionary<T, TA>();
            foreach (var t in list)
            {
                cache[(T) t] = typeof (T).GetField(t.ToString()).GetFirstCustomAttribute<TA>();
            }
            return cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">FieldInfo</typeparam>
        /// <typeparam name="TK">Attribute</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IDictionary<T, TK> GetCacheDefaultValue<T, TK>(List<T> list) where T : struct
        {
            return GetCache<T, DefaultValueAttribute>(list)
                .ToDictionary(k => k.Key, v => v.Value != null ? (TK) v.Value.Value : default(TK));
        }

        #endregion
    }
}