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
        public static int ToInt(this JsonData value, int def = 0)
        {
            return value.IsInt ? (int) value : def;
        }

        /// <summary>
        /// 转为Long型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static long ToLong(this JsonData value, long def = 0)
        {
            return value.IsLong ? (long) value : def;
        }

        /// <summary>
        /// 转为Float型（可能有精度损失）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static float ToFloat(this JsonData value, float def = 0)
        {
            return value.IsDouble ? (float) (double) value : def;
        }

        /// <summary>
        /// 转为Double型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static double ToDouble(this JsonData value, double def = 0)
        {
            return value.IsDouble ? (double) value : def;
        }

        /// <summary>
        /// 转为Bool型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static bool ToBool(this JsonData value, bool def = false)
        {
            return value.IsBoolean ? (bool) value : def;
        }
    }
}