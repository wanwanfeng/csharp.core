using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

namespace Library.Extensions
{
    public static class StringExtensions
    {
        #region 裁剪字符串并追加

        /// <summary>
        /// 裁剪字符串并追加(纯英文)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public static string Cut(this string str, int length, bool add = true)
        {
            return str.Length < length ? str : str.Substring(0, length) + (add ? "……" : "");
        }

        /// <summary>
        /// 裁剪字符串并追加(中英混合)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public static string Cut2(this string str, int length, bool add = true)
        {
            byte[] bs = System.Text.Encoding.Default.GetBytes(str);
            if (bs.Length > length)
            {
                byte[] bs2 = new byte[length];
                //Buffer.BlockCopy(bs, 0, bs2, 0, length * 4);
                Array.ConstrainedCopy(bs, 0, bs2, 0, length);
                str = System.Text.Encoding.Default.GetString(bs2) + (add ? "……" : "");
            }
            else
            {
                str = str + (add ? "……" : "");
            }
            return str;
        }



        #endregion

        #region 字符串添加字符

        /// <summary>
        /// 字符串添加字符
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="lineCount"></param>
        /// <returns></returns>
        public static string CopyChar(this char cr, int lineCount)
        {
            StringBuilder sb = new StringBuilder(lineCount);
            for (int i = 0; i < lineCount; i++)
            {
                sb.Append(cr);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 字符串添加字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lineNumber"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Insert(this string str, int lineNumber, string value)
        {
            if (str.Length < lineNumber)
                return str;
            for (int i = str.Length - 1; i > 0; i--)
            {
                if (i%lineNumber == 0)
                {
                    str = str.Insert(i, value);
                }
            }
            return str;
        }

        /// <summary>
        /// 字符串添加换行符
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lineNumber"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Insert(this string value, int lineNumber)
        {
            byte[] bytes = Encoding.Default.GetBytes(value);

            if (bytes.Length < lineNumber)
                return value;
            for (int i = value.Length - 1; i > 0; i--)
            {
                if (i%lineNumber == 0)
                {
                    value = value.Insert(i, "\n");
                }
            }

            if (value.Length < lineNumber)
                return value;
            for (int i = value.Length - 1; i > 0; i--)
            {
                if (i%lineNumber == 0)
                {
                    value = value.Insert(i, "\n");
                }
            }
            return value;
        }


        #endregion

        #region 将阿拉伯数字转为罗马数字

        /// <summary>
        /// 将阿拉伯数字转为罗马数字
        /// </summary>
        /// <param name="value">阿拉伯数字</param>
        public static string ToRoman(this int value)
        {
            int[] arabic = new int[13] {1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1};
            string[] roman = new string[13] {"M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I"};
            int i = 0;
            string result = "";

            while (value > 0)
            {
                while (value >= arabic[i])
                {
                    value = value - arabic[i];
                    result = result + roman[i];
                }
                i++;
            }
            return result;
        }

        #endregion
    }
}