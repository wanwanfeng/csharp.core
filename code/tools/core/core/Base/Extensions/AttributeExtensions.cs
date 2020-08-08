using System;
using System.Linq;
using System.Reflection;

namespace Library.Extensions
{
    public static partial class AttributeExtensions
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

        /// <summary>
        /// 获取第一个自定义的Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(this MemberInfo type, bool inherit = false) where T : Attribute
        {
            return type.GetCustomAttributes(inherit).OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool IsDefined<T>(this MemberInfo type, bool inherit = false) where T : Attribute
        {
            return type.IsDefined(typeof (T), inherit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool IsDefined(this MemberInfo type, Type target, bool inherit = false)
        {
            return type.IsDefined(target, inherit);
        }
    }
}