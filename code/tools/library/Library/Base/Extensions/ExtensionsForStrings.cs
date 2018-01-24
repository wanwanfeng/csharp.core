using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

namespace Library.Extensions
{
    public static class ExtensionsForStrings
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


        private static Dictionary<string, string> _loumaNumberCache = new Dictionary<string, string>()
        {
            {"1000", "M"},
            {"900", "CM"},
            {"500", "D"},
            {"400", "CD"},
            {"100", "C"},
            {"90", "XC"},
            {"50", "L"},
            {"40", "XL"},
            {"10", "X"},
            {"9", "IX"},
            {" 5", "V"},
            {" 4", "IV"},
            {" 1", "I"},
        };

        /// <summary>
        /// 将阿拉伯数字转为罗马数字
        /// </summary>
        /// <param name="value">阿拉伯数字</param>
        public static string ToLoumaNumber(this int value)
        {
            int[] arabic = new int[13];
            string[] roman = new string[13];
            int i = 0;
            string result = "";

            arabic[0] = 1000;
            arabic[1] = 900;
            arabic[2] = 500;
            arabic[3] = 400;
            arabic[4] = 100;
            arabic[5] = 90;
            arabic[6] = 50;
            arabic[7] = 40;
            arabic[8] = 10;
            arabic[9] = 9;
            arabic[10] = 5;
            arabic[11] = 4;
            arabic[12] = 1;

            roman[0] = "M";
            roman[1] = "CM";
            roman[2] = "D";
            roman[3] = "CD";
            roman[4] = "C";
            roman[5] = "XC";
            roman[6] = "L";
            roman[7] = "XL";
            roman[8] = "X";
            roman[9] = "IX";
            roman[10] = "V";
            roman[11] = "IV";
            roman[12] = "I";

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