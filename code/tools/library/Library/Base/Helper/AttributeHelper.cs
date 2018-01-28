using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Library.Helper
{
    public class TypeValueAttribute : Attribute
    {
        public Type value { get; protected set; }

        public TypeValueAttribute(Type value = null)
        {
            this.value = value;
        }
    }

    public class StringValueAttribute : Attribute
    {
        public string value { get; protected set; }

        public StringValueAttribute(string value = "")
        {
            this.value = value;
        }
    }

    public class HookValueAttribute : StringValueAttribute
    {
        public HookValueAttribute(string value = "")
        {
            this.value = value;
        }
    }

    public class IntValueAttribute : Attribute
    {
        public int value { get; protected set; }

        public IntValueAttribute(int value = 0)
        {
            this.value = value;
        }
    }

    public class BoolValueAttribute : Attribute
    {
        public bool value { get; protected set; }

        public BoolValueAttribute(bool value = false)
        {
            this.value = value;
        }
    }

    public static class AttributeHelper
    {
        #region enum

        public static IDictionary<T, TA> GetCache<T, TA>() where TA : Attribute
        {
            IDictionary<T, TA> cache = new Dictionary<T, TA>();
            foreach (var value in Enum.GetValues(typeof (T)))
            {
                cache[(T) value] = typeof (T).GetField(value.ToString())
                    .GetCustomAttributes(false)
                    .OfType<TA>()
                    .FirstOrDefault();
            }
            return cache;
        }

        public static IDictionary<T, Type> GetCacheTypeValue<T>() where T : struct
        {
            return GetCache<T, TypeValueAttribute>()
                .ToDictionary(k => k.Key, v => v.Value != null ? v.Value.value : null);
        }

        public static IDictionary<T, TK> GetCacheDefaultValue<T, TK>() where T : struct
        {
            return GetCache<T, DefaultValueAttribute>()
                .ToDictionary(k => k.Key, v => v.Value != null ? (TK) v.Value.Value : default(TK));
        }

        public static IDictionary<T, string> GetCacheDescription<T>() where T : struct
        {
            return GetCache<T, DescriptionAttribute>()
                .ToDictionary(k => k.Key, v => v.Value != null ? v.Value.Description : "");
        }

        public static IDictionary<T, string> GetCacheStringValue<T>() where T : struct
        {
            return GetCache<T, StringValueAttribute>()
                .ToDictionary(k => k.Key, v => v.Value != null ? v.Value.value : "");
        }

        public static IDictionary<T, string> GetCacheHookValue<T>() where T : struct
        {
            return GetCache<T, HookValueAttribute>()
                .ToDictionary(k => k.Key, v => v.Value != null ? v.Value.value : "");
        }

        public static IDictionary<T, int> GetCacheIntValue<T>() where T : struct
        {
            return GetCache<T, IntValueAttribute>()
                .ToDictionary(k => k.Key, v => v.Value != null ? v.Value.value : 0);
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
                cache[(T) t] = typeof (T).GetField(t.ToString())
                    .GetCustomAttributes(false)
                    .OfType<TA>()
                    .FirstOrDefault();
            }
            return cache;
        }

        public static IDictionary<T, TK> GetCacheDefaultValue<T, TK>(List<T> list) where T : struct
        {
            return GetCache<T, DefaultValueAttribute>(list)
                .ToDictionary(k => k.Key, v => v.Value != null ? (TK) v.Value.Value : default(TK));
        }

        #endregion
    }
}