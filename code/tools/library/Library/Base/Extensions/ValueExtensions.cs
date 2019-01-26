using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Extensions
{
    /// <summary>
    /// 值类型扩展
    /// </summary>
    public static class ValueExtensions
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
            return bool.TryParse(value.ToLower(), out v) ? v : def;
        }

        /// <summary>
        /// 转为int[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static IEnumerable<int> AsInt(this IEnumerable<string> value, int def = 0)
        {
            return value.Select(p => p.AsInt(def));
        }

        /// <summary>
        /// 转为float[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static IEnumerable<float> AsFloat(this IEnumerable<string> value, float def = 0)
        {
            return value.Select(p => p.AsFloat(def));
        }

        /// <summary>
        /// 转为double[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static IEnumerable<double> AsDouble(this IEnumerable<string> value, double def = 0)
        {
            return value.Select(p => p.AsDouble(def));
        }

        /// <summary>
        /// 转为long[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static IEnumerable<long> AsLong(this IEnumerable<string> value, long def = 0)
        {
            return value.Select(p => p.AsLong(def));
        }

        /// <summary>
        /// 转为decimal[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static IEnumerable<decimal>AsDecimal(this IEnumerable<string> value, decimal def = 0)
        {
            return value.Select(p => p.AsDecimal(def));
        }

        /// <summary>
        /// 转为decimal[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static IEnumerable<byte> AsByte(this IEnumerable<string> value, byte def = 0)
        {
            return value.Select(p => p.AsByte(def));
        }

        /// <summary>
        /// 转为decimal[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static IEnumerable<short> AsShort(this IEnumerable<string> value, short def = 0)
        {
            return value.Select(p => p.AsShort(def));
        }

        /// <summary>
        /// 转为bool[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static IEnumerable<bool> AsBool(this IEnumerable<string> value, bool def = false)
        {
            return value.Select(p => p.AsBool(def));
        }

        /// <summary>
        /// 转为int[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<int> SplitInt(this string value, params char[] separator)
        {
            return value.SplitString(separator).AsInt();
        }

        /// <summary>
        /// 转为float[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<float> SplitFloat(this string value, params char[] separator)
        {
            return value.SplitString(separator).AsFloat();
        }

        /// <summary>
        /// 转为double[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<double> SplitDouble(this string value, params char[] separator)
        {
            return value.SplitString(separator).AsDouble();
        }

        /// <summary>
        /// 转为long[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<long> SplitLong(this string value, params char[] separator)
        {
            return value.SplitString(separator).AsLong();
        }

        /// <summary>
        /// 转为decimal[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<decimal> SplitDecimal(this string value, params char[] separator)
        {
            return value.SplitString(separator).AsDecimal();
        }

        /// <summary>
        /// 转为decimal[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<byte> SplitByte(this string value, params char[] separator)
        {
            return value.SplitString(separator).AsByte();
        }

        /// <summary>
        /// 转为decimal[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<short> SplitShort(this string value, params char[] separator)
        {
            return value.SplitString(separator).AsShort();
        }

        /// <summary>
        /// 转为bool[]型数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<bool> SplitBool(this string value, params char[] separator)
        {
            return value.SplitString(separator).AsBool();
        }

        /// <summary>
        /// 转为string[]型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] SplitString(this string value, params char[] separator)
        {
            try
            {
                return value.Split(separator.Length == 0 ? new[] {','} : separator);
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
                return content + string.Join(content, value.ToCharArray().Select(p => p.ToString()).ToArray());

                //char[] source = value.ToChar();
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
        public static string Join<T>(this IEnumerable<T> value, string separator = null)
        {
            return string.Join(separator ?? Environment.NewLine, value.Select(p => p.ToString()).ToArray());
        }
    }
}