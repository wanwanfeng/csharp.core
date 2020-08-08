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
            return type.GetEnumerator().ToDictionary(p => p, q => (int) q);
        }

        public static Dictionary<string, int> ToDictionary(this Enum type)
        {
            return type.GetEnumerator().ToDictionary(p => p.ToString(), q => (int) q);
        }

        #region enum

        public static T Get<T>(this Enum type) where T : Attribute
        {
            return type.GetType().GetField(type.ToString()).GetFirstCustomAttribute<T>();
        }

        public static Type GetTypeValue(this Enum type)
        {
            var v = type.Get<TypeValueAttribute>();
            return v != null ? v.value : null;
        }

        public static object GetDefaultValue(this Enum type)
        {
            var v = type.Get<DefaultValueAttribute>();
            return v != null ? v.Value : null;
        }

        public static string GetDescription(this Enum type)
        {
            var v = type.Get<DescriptionAttribute>();
            return v != null ? v.Description : "";
        }

        public static string GetStringValue(this Enum type)
        {
            var v = type.Get<StringValueAttribute>();
            return v != null ? v.value : "";
        }

        public static string GetHookValue(this Enum type)
        {
            var v = type.Get<HookValueAttribute>();
            return v != null ? v.value : "";
        }

        public static int GetIntValue(this Enum type)
        {
            var v = type.Get<IntValueAttribute>();
            return v != null ? v.value : 0;
        }

        #endregion
    }
}