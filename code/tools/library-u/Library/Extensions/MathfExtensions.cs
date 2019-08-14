using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Library
{

    //Int.ToString(format):
    //格式字符串采用以下形式：Axx，其中 A 为格式说明符，指定格式化类型，xx 为精度说明符，控制格式化输出的有效位数或小数位数，具体如下：

    //格式说明符
    //说明    示例	输出
    //C 货币	2.5.ToString("C")	￥2.50
    //D	十进制数	25.ToString("D5")	00025
    //E	科学型	25000.ToString("E")	2.500000E+005
    //F	固定点	25.ToString("F2")	25.00
    //G	常规	2.5.ToString("G")	2.5
    //N	数字	2500000.ToString("N")	2,500,000.00
    //X	十六进制	255.ToString("X")	FF

    /*
    //十进制转二进制
    Console.WriteLine(Convert.ToString(69, 2));
    //十进制转八进制
    Console.WriteLine(Convert.ToString(69, 8));
    //十进制转十六进制
    Console.WriteLine(Convert.ToString(69, 16));

    //二进制转十进制
    Console.WriteLine(Convert.ToInt32("100111101", 2));
    //八进制转十进制
    Console.WriteLine(Convert.ToInt32("76", 8));
    //十六进制转十进制
    Console.WriteLine(Convert.ToInt32("FF", 16));
     * */

    /// <summary>
    /// Math扩展
    /// </summary>
    public static class MathfExtensions
    {
        /// <summary>
        /// 默认保留小数点后两位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        public static string ToSignificantDigit(this int value, int digit = 2)
        {
            return Convert.ToDouble(value).ToString("F" + digit);
        }

        /// <summary>
        /// 默认保留小数点后两位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        public static string ToSignificantDigit(this float value, int digit = 2)
        {
            return Convert.ToDouble(value).ToString("F" + digit);
        }

        /// <summary>
        /// 默认保留小数点后两位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        public static string ToSignificantDigitd(this double value, int digit = 2)
        {
            return Convert.ToDouble(value).ToString("F" + digit);
        }

        /// <summary>
        /// 得到小数点后2位的百分比,自动 加上%号;
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPercentage(this int value)
        {
            return Convert.ToDouble(value).ToString("P");
        }

        /// <summary>
        /// 得到小数点后2位的百分比,自动 加上%号;
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPercentage(this float value)
        {
            return Convert.ToDouble(value).ToString("P");
        }

        /// <summary>
        /// 得到小数点后2位的百分比,自动 加上%号;
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPercentage(this double value)
        {
            return Convert.ToDouble(value).ToString("P");
        }
    }
}
