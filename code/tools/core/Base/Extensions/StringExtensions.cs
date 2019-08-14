using System;
using System.Text;

namespace Library.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string str)
        {
            return string.Empty == str;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string Join(this string str, string separator, string[] value)
        {
            return string.Join(separator, value);
        }

        public static string Join(this string str, string separator, string[] value, int startIndex, int count)
        {
            return string.Join(separator, value, startIndex, count);
        }

        public static string IsInterned(this string str)
        {
            return string.IsInterned(str);
        }

        public static string Intern(this string str)
        {
            return string.Intern(str);
        }

        #region 裁剪字符串并追加

        /// <summary>
        /// 裁剪字符串并追加end
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string Substring(this string str, int length, string end = "")
        {
            return str.Length < length ? str : str.Substring(0, length) + end;
        }

        /// <summary>
        /// 裁剪字符串并追加(中英混合)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string SubstringReal(this string str, int length, string end = "")
        {
            byte[] bs = System.Text.Encoding.Default.GetBytes(str);
            if (bs.Length > length)
            {
                byte[] bs2 = new byte[length];
                //Buffer.BlockCopy(bs, 0, bs2, 0, length * 4);
                Array.ConstrainedCopy(bs, 0, bs2, 0, length);
                str = System.Text.Encoding.Default.GetString(bs2) + end;
            }
            else
            {
                str = str + end;
            }
            return str;
        }



        #endregion

        #region 字符串添加字符

        /// <summary>
        /// 字符添加字符
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="lineCount"></param>
        /// <returns></returns>
        public static string PadRight(this char cr, int lineCount)
        {
            return cr.ToString().PadRight(lineCount, cr);
        }

        /// <summary>
        /// 字符添加字符向两边扩充
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lineCount"></param>
        /// <param name="paddingChar"></param>
        /// <returns></returns>
        public static string Pad(this string value, int lineCount, char paddingChar = ' ')
        {
            //lineCount = 9;

            lineCount += lineCount%2 == 0 ? 0 : 1;
            return value.PadLeft(lineCount/2 + value.Length/2, paddingChar).PadRight(lineCount, paddingChar);
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