using System;
using System.Collections.Generic;
using System.ComponentModel;
using findText.Script;
using Library;
using Library.Extensions;

namespace findText
{
    internal static class Program
    {
        public enum ConvertType
        {
            [TypeValue(typeof (ActionForCss))] css,
            [TypeValue(typeof (ActionForCpp))] cpp,
            [TypeValue(typeof (ActionForCSharp))] csharp,
            [TypeValue(typeof (ActionForPhp))] php,
            [TypeValue(typeof (ActionForJava))] java,
            [TypeValue(typeof (ActionForJavaScript))] javascript,
            [TypeValue(typeof (ActionForHtml))] html,
            [TypeValue(typeof (ActionForHtml2))] [Description("html 插件")] html2,
            [TypeValue(typeof (GetIPList))] [Description("获取代理IP")] GetIPList,
        }

        private static void Main(string[] args)
        {
            Action<object> callFunc = obj =>
            {
                BaseActionFor baseActionFor = (BaseActionFor) obj;
                SystemConsole.Run(config: new Dictionary<string, Action>()
                {
                    {"搜索", () => { baseActionFor.Open(); }},
                    {"还原", () => { baseActionFor.Revert(); }},
                });
            };
            SystemConsole.Run<ConvertType>(callFunc);
        }
    }
}
