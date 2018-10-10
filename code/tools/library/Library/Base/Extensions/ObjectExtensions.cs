
namespace Library.Extensions
{
    /// <summary>
    /// Object扩展
    /// </summary>
    public static class ObjectExtensions
    {
        public static object GetPropertyValue(this object value, string name)
        {
            var info = value.GetType().GetProperty(name);
            return info != null ? info.GetValue(value, null) : null;
        }

        public static object GetFieldValue(this object value, string name)
        {
            var info = value.GetType().GetField(name);
            return info != null ? info.GetValue(value) : null;
        }
    }
}