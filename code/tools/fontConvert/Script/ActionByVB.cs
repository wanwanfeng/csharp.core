using System;
using System.IO;
using Microsoft.VisualBasic;

namespace fontConvert.Script
{
    public class ActionByVB : BaseActionBy
    {
        /// <summary>
        /// 字符转换为繁体中文
        /// </summary>
        public class ToTraditional : ActionByVB
        {
            protected override string OpenRunLine(string value)
            {
                //return Microsoft.VisualBasic.Strings.StrConv(value, Microsoft.VisualBasic.VbStrConv.TraditionalChinese, 0);
                return value;
            }
        }

        /// <summary>
        /// 字符转换成简体中文
        /// </summary>
        public class ToSimplified : ActionByVB
        {
            protected override string OpenRunLine(string value)
            {
                //return Microsoft.VisualBasic.Strings.StrConv(value, Microsoft.VisualBasic.VbStrConv.SimplifiedChinese, 0);
                return value;
            }
        }

    }
}