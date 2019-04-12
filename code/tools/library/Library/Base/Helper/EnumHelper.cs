using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Helper
{
    public partial class EnumHelper
    {
        public static IEnumerable<object> GetValue(Type type)
        {
            if (type.IsEnum)
            {
                var list = Enum.GetValues(type);
                foreach (var it in list)
                {
                    yield return it;
                }
                yield break;
            }
            throw new Exception(string.Format("{0} type is not Enum !", type.Name));
        }

        public static IEnumerable<object> GetValue<T>()
        {
            return GetValue(typeof (T));
        }

        public static Dictionary<object, string> ToStringDic(Type type)
        {
            return GetValue(type).ToDictionary(p => p, q => q.ToString());
        }

        public static Dictionary<T, string> ToStringDic<T>()
        {
            return GetValue<T>().ToDictionary(p => (T) p, q => q.ToString());
        }

        public static Dictionary<object, int> ToIntDic(Type type)
        {
            return GetValue(type).ToDictionary(p => p, q => (int)q);
        }

        public static Dictionary<T, int> ToIntDic<T>()
        {
            return GetValue<T>().ToDictionary(p => (T) p, q => (int) q);
        }

        public static Dictionary<string, int> ToDictionary(Type type)
        {
            return GetValue(type).ToDictionary(p => p.ToString(), q => (int)q);
        }

        public static Dictionary<string, int> ToDictionary<T>()
        {
            return GetValue<T>().ToDictionary(p => p.ToString(), q => (int) q);
        }
    }
}