using System;
using System.IO;

namespace fontConvert.Script
{
    public class ActionByVB : BaseActionBy
    {
        protected override string OpenRunLine(string value)
        {
            //return Microsoft.VisualBasic.Strings.StrConv(value, Microsoft.VisualBasic.VbStrConv.TraditionalChinese, 0);
            //return Microsoft.VisualBasic.Strings.StrConv(value, Microsoft.VisualBasic.VbStrConv.TraditionalChinese, 0);
            //return Microsoft.VisualBasic.Strings.StrConv(value, Microsoft.VisualBasic.VbStrConv.SimplifiedChinese, 0);
            return value;
        }


        public class ToTraditional : ActionByDLL
        {
            protected override string OpenRunLine(string value)
            {
                return Base.ToTraditional(value); //字符转换为繁体中文
            }
        }

        public class ToSimplified : ActionByDLL
        {
            protected override string OpenRunLine(string value)
            {
                return Base.ToSimplified(value); //字符转换成简体中文
            }
        }

    }
}