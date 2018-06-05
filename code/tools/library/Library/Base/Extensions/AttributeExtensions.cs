using System;
using System.Linq;
using System.Reflection;

namespace Library.Extensions
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// 获取第一个自定义的Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetFirstCustomAttribute<T>(this MemberInfo type, bool inherit = false) where T : Attribute
        {
            return type.GetCustomAttributes(inherit).OfType<T>().FirstOrDefault();
        }
    }
}