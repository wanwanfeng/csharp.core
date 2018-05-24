using Library.Extensions;
using LitJson;

namespace Library.LitJson
{
    public static class LitJsonExtensions
    {
        /// <summary>
        /// 转为Int型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static int AsInt(this JsonData value, int def = 0)
        {
            return value.IsInt ? value.ToString().AsInt(def) : def;
        }

        /// <summary>
        /// 转为Long型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static long AsLong(this JsonData value, long def = 0)
        {
            return value.IsLong ? value.ToString().AsLong(def) : def;
        }

        /// <summary>
        /// 转为Float型（可能有精度损失）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static float AsFloat(this JsonData value, float def = 0)
        {
            return value.IsDouble ? value.ToString().AsFloat(def) : def;
        }

        /// <summary>
        /// 转为Double型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static double AsDouble(this JsonData value, double def = 0)
        {
            return value.IsDouble ? value.ToString().AsDouble(def) : def;
        }

        /// <summary>
        /// 转为Bool型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static bool AsBool(this JsonData value, bool def = false)
        {
            return value.IsBoolean ? value.ToString().AsBool(def) : def;
        }
    }
}