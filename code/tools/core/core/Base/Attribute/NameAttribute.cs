using System;
using System.Linq;
using Library.Extensions;

namespace Library
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NameAttribute : Attribute
    {
        public string mainName;
        public string[] anotherNames;

        public NameAttribute(params string[] anotherName)
        {
            this.anotherNames = anotherName;
        }

        public static string[] GetAnotherNames<T>(string mainName)
        {
            var att = typeof (T).GetField(mainName).GetFirstCustomAttribute<NameAttribute>();
            return att == null ? new string[0] : att.anotherNames;
        }

        public static string GetAnotherName<T>(string mainName)
        {
            return GetAnotherNames<T>(mainName).FirstOrDefault();
        }

        public static string GetMainName<T>(string anotherName)
        {
            foreach (var field in typeof(T).GetFields())
            {
                var anothersNames = GetAnotherNames<T>(field.Name);
                if (anothersNames.Contains(anotherName))
                    return field.Name;
            }
            return null;
        }
    }
}