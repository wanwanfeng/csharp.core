using System;
using System.Text;

namespace Library.Extensions
{
    /// <summary>
    /// 值类型扩展
    /// </summary>
    public static class ExtensionsForValue
    {
        /// <summary>
        /// 转为byte型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static byte AsByte(this string value, byte def = 0)
        {
            byte v = 0;
            return byte.TryParse(value, out v) ? v : def;
        }

        /// <summary>
        /// 转为short型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static short AsShort(this string value, short def = 0)
        {
            short v = 0;
            return short.TryParse(value, out v) ? v : def;
        }

        /// <summary>
        /// 转为ushort型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static ushort AsuShort(this string value, ushort def = 0)
        {
            ushort v = 0;
            return ushort.TryParse(value, out v) ? v : def;
        }

        /// <summary>
        /// 转为Int型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static int AsInt(this string value, int def = 0)
        {
            int v = 0;
            return int.TryParse(value, out v) ? v : def;
        }

        /// <summary>
        /// 转为uInt型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static uint AsuInt(this string value, uint def = 0)
        {
            uint v = 0;
            return uint.TryParse(value, out v) ? v : def;
        }

        /// <summary>
        /// 转为Long型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static long AsLong(this string value, long def = 0)
        {
            long v = 0;
            return long.TryParse(value, out v) ? v : def;
        }

        /// <summary>
        /// 转为uLong型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static ulong AsuLong(this string value, ulong def = 0)
        {
            ulong v = 0;
            return ulong.TryParse(value, out v) ? v : def;
        }

        /// <summary>
        /// 转为Float型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static float AsFloat(this string value, float def = 0)
        {
            float v = 0;
            return float.TryParse(value, out v) ? v : def;
        }

        /// <summary>
        /// 转为Double型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static double AsDouble(this string value, double def = 0)
        {
            double v = 0.0;
            return double.TryParse(value, out v) ? v : def;
        }

        /// <summary>
        /// 转为Decimal型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static decimal AsDecimal(this string value, decimal def = 0)
        {
            decimal v = 0;
            return decimal.TryParse(value, out v) ? v : def;
        }

        /// <summary>
        /// 转为Bool型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static bool AsBool(this string value, bool def = false)
        {
            var v = false;
            return bool.TryParse(value, out v) ? v : def;
        }

        /// <summary>
        /// 转为int[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int[] AsIntArray(this string[] value)
        {
            return Array.ConvertAll(value, p => p.AsInt());
        }

        /// <summary>
        /// 转为float[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float[] AsFloatArray(this string[] value)
        {
            return Array.ConvertAll(value, p => p.AsFloat());
        }

        /// <summary>
        /// 转为double[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double[] AsDoubleArray(this string[] value)
        {
            return Array.ConvertAll(value, p => p.AsDouble());
        }

        /// <summary>
        /// 转为long[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long[] AsLongArray(this string[] value)
        {
            return Array.ConvertAll(value, p => p.AsLong());
        }

        /// <summary>
        /// 转为decimal[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal[] AsDecimalArray(this string[] value)
        {
            return Array.ConvertAll(value, p => p.AsDecimal());
        }

        /// <summary>
        /// 转为decimal[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] AsByteArray(this string[] value)
        {
            return Array.ConvertAll(value, p => p.AsByte());
        }

        /// <summary>
        /// 转为decimal[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static short[] AsShortArray(this string[] value)
        {
            return Array.ConvertAll(value, p => p.AsShort());
        }

        /// <summary>
        /// 转为bool[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool[] AsBoolArray(this string[] value)
        {
            return Array.ConvertAll(value, p => p.AsBool());
        }

        /// <summary>
        /// 转为int[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static int[] AsIntArray(this string value, params char[] separator)
        {
            string[] str = value.AsStringArray(separator);
            return Array.ConvertAll(str, p => p.AsInt());
        }

        /// <summary>
        /// 转为float[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static float[] AsFloatArray(this string value, params char[] separator)
        {
            string[] str = value.AsStringArray(separator);
            return Array.ConvertAll(str, p => p.AsFloat());
        }

        /// <summary>
        /// 转为double[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static double[] AsDoubleArray(this string value, params char[] separator)
        {
            string[] str = value.AsStringArray(separator);
            return Array.ConvertAll(str, p => p.AsDouble());
        }

        /// <summary>
        /// 转为bool[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static bool[] AsBoolArray(this string value, params char[] separator)
        {
            string[] str = value.AsStringArray(separator);
            return Array.ConvertAll(str, p => p.AsBool());
        }

        /// <summary>
        /// 转为string[]型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] AsStringArray(this string value, params char[] separator)
        {
            try
            {
                var result = value.Split(separator.Length == 0 ? separator = new char[] {','} : separator);
                return result;
            }
            catch (Exception)
            {
                return new string[0];
            }
        }

        /// <summary>
        /// string型每个char均匀插入同一字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string InsertString(this string value, string content)
        {
            try
            {
                return content + string.Join(content, value.ToCharArray().AsStringArray());

                //char[] source = value.ToCharArray();
                //StringBuilder sbBuilder = new StringBuilder(value.Length + (content.Length)*value.Length);
                //foreach (var item in source)
                //{
                //    sbBuilder.Append(content);
                //    sbBuilder.Append(item);
                //}
                //return sbBuilder.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 值类型数组连接为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinToString<T>(this T[] value, string separator = "\r\n")
        {
            return string.Join(separator, value.AsStringArray());
        }

        /// <summary>
        /// 值类型数组转为string[]型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string[] AsStringArray<T>(this T[] value)
        {
            return Array.ConvertAll(value, p => p.ToString());
        }
    }
}