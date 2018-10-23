
using System.Reflection;

namespace Library.Extensions
{
    /// <summary>
    /// Object扩展
    /// </summary>
    public static class ObjectExtensions
    {
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static bool IsDefault<T>(this T obj) where T : class
        {
            return obj == default(T);
        }


        public static object GetPropertyValue(this object value, string name)
        {
            var info = value.GetType().GetProperty(name);
            return info != null ? info.GetValue(value, null) : null;
        }

        public static object GetFieldValue(this object value, string name)
        {
            var type = value.GetType();
            var info = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return info != null ? info.GetValue(value) : null;
        }
    }
}