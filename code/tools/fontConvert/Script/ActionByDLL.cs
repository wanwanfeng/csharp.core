using System;
using System.IO;
using System.Runtime.InteropServices;

namespace fontConvert.Script
{
    public class ActionByDLL : BaseActionBy
    {
        protected class Base
        {
            /// <summary>
            /// 中文字符工具类
            /// </summary>
            private const int LOCALE_SYSTEM_DEFAULT = 0x0800;

            private const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
            private const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;

            [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern int LCMapString(int Locale, int dwMapFlags, string lpSrcStr, int cchSrc,
                [Out] string lpDestStr, int cchDest);

            public static string ToSimplified(string source)
            {
                String target = new String(' ', source.Length);
                int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_SIMPLIFIED_CHINESE, source, source.Length, target,
                    source.Length);
                return target;
            }

            public static string ToTraditional(string source)
            {
                String target = new String(' ', source.Length);
                int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_TRADITIONAL_CHINESE, source, source.Length, target,
                    source.Length);
                return target;
            }
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