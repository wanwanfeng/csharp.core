using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Library.Extensions;

namespace Library.Helper
{
    public static class AttributeHelper
    {
        #region enum

        public static IDictionary<object, TA> GetCache<T, TA>() where TA : Attribute
        {
            return EnumHelper.GetValue<T>().ToDictionary(p => p, q => typeof (T).GetField(q.ToString()).GetFirstCustomAttribute<TA>());
        }

        public static IDictionary<T, Type> GetCacheTypeValue<T>() where T : struct
        {
            return GetCache<T, TypeValueAttribute>().ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.value : null);
        }

        public static IDictionary<T, TK> GetCacheDefaultValue<T, TK>() where T : struct
        {
            return GetCache<T, DefaultValueAttribute>().ToDictionary(k => (T) k.Key, v => v.Value != null ? (TK) v.Value.Value : default(TK));
        }

        public static IDictionary<T, string> GetCacheDescription<T>() where T : struct
        {
            return GetCache<T, DescriptionAttribute>().ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.Description : "");
        }

        public static IDictionary<T, string> GetCacheStringValue<T>() where T : struct
        {
            return GetCache<T, StringValueAttribute>().ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.value : "");
        }

        public static IDictionary<T, string> GetCacheHookValue<T>() where T : struct
        {
            return GetCache<T, HookValueAttribute>().ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.value : "");
        }

        public static IDictionary<T, int> GetCacheIntValue<T>() where T : struct
        {
            return GetCache<T, IntValueAttribute>().ToDictionary(k => (T)k.Key, v => v.Value != null ? v.Value.value : 0);
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
        private static IDictionary<T, TA> GetCache<T, TA>(IEnumerable<T> list) where TA : Attribute
        {
            return list.ToDictionary(p => p, q => typeof (T).GetField(q.ToString()).GetFirstCustomAttribute<TA>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">FieldInfo</typeparam>
        /// <typeparam name="TK">Attribute</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IDictionary<T, TK> GetCacheDefaultValue<T, TK>(IEnumerable<T> list) where T : struct
        {
            return GetCache<T, DefaultValueAttribute>(list).ToDictionary(k => k.Key, v => v.Value != null ? (TK) v.Value.Value : default(TK));
        }

        #endregion
    }
}