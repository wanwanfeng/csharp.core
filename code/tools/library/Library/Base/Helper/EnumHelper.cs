using System;
using System.Collections.Generic;

namespace Library.Helper
{
    public partial class EnumHelper
    {
        private static void FunValue(Type type, Action<object> action)
        {
            if (type.IsEnum)
            {
                var list = Enum.GetValues(type);
                foreach (var array in list)
                {
                    action.Invoke(array);
                }
            }
            else
            {
                throw new Exception(string.Format("{0} type is not Enum !", type.Name));
            }
        }

        private static void FunValue<T>(Action<object> action)
        {
            FunValue(typeof (T), action);
        }

        public static Dictionary<object, string> ToStringDic(Type type)
        {
            var dic = new Dictionary<object, string>();
            FunValue(type, (v) => { dic[v] = v.ToString(); });
            return dic;
        }

        public static Dictionary<object, int> ToIntDic(Type type)
        {
            var dic = new Dictionary<object, int>();
            FunValue(type, (v) => { dic[v] = (int) v; });
            return dic;
        }

        public static Dictionary<string, int> ToDictionary(Type type)
        {
            var dic = new Dictionary<string, int>();
            FunValue(type, (v) => { dic[v.ToString()] = (int) v; });
            return dic;
        }

        public static Dictionary<T, string> ToStringDic<T>()
        {
            var dic = new Dictionary<T, string>();
            FunValue<T>((v) => { dic[(T) v] = v.ToString(); });
            return dic;
        }

        public static Dictionary<T, int> ToIntDic<T>()
        {
            var dic = new Dictionary<T, int>();
            FunValue<T>((v) => { dic[(T) v] = (int) v; });
            return dic;
        }

        public static Dictionary<string, int> ToDictionary<T>()
        {
            var dic = new Dictionary<string, int>();
            FunValue<T>((v) => { dic[v.ToString()] = (int) v; });
            return dic;
        }
    }
}