﻿using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Library.Helper
{
    public class StringHelper
    {
        /// <summary>
        /// 字符串转Unicode
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>Unicode编码后的字符串</returns>
        public static string String2Unicode(string source)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(source);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'),
                    bytes[i].ToString("x").PadLeft(2, '0'));
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Unicode转字符串
        /// </summary>
        /// <param name="source">经过Unicode编码的字符串</param>
        /// <returns>正常字符串</returns>
        public static string Unicode2String(string source)
        {
            var regex = new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return regex.Replace(source, x => Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)).ToString());
        }
        /// <summary>
        /// 使用litjson的方法进行uncode进行转换
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string StringToUnicode(string source)
        {
            StringBuilder sb = new StringBuilder(1024);
            foreach (var _char in source.ToCharArray())
            {
                if ((int)_char >= 32 && (int)_char <= 126)
                {
                    sb.Append(_char);
                    continue;
                }
                sb.Append("\\u");
                sb.Append(IntToHex(_char));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 来自于litjson
        /// </summary>
        /// <param name="_char"></param>
        /// <returns></returns>
        private static char[] IntToHex(int _char)
        {
            int n = _char;

            int num;
            char[] hex = new char[4];
            for (int i = 0; i < 4; i++)
            {
                num = n % 16;

                if (num < 10)
                    hex[3 - i] = (char)('0' + num);
                else
                    hex[3 - i] = (char)('A' + (num - 10));

                n >>= 4;
            }
            return hex;
        }
    }
}