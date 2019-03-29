using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using findText.Script;
using Library;
using Library.Extensions;

namespace findText
{
    internal static class Program
    {
        public enum ConvertType
        {
            [TypeValue(typeof(ActionForCss))] css,
            [TypeValue(typeof(ActionForCpp))] cpp,
            [TypeValue(typeof(ActionForCSharp))] csharp,
            [TypeValue(typeof(ActionForPhp))] php,
            [TypeValue(typeof(ActionForJava))] java,

            [TypeValue(typeof(ActionForJavaScript))]
            javascript,
            [TypeValue(typeof(ActionForHtml))] html,

            [TypeValue(typeof(ActionForHtml2))] [Description("html 插件")]
            html2
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

        public static void ForEachPaths(this IEnumerable<string> paths, Action<string> callAction)
        {
            paths.Select(p => p.Replace("\\", "/")).ToList().ForEach((p, i, target) =>
            {
                Console.WriteLine("is now : " + (((float)i) / target.Count).ToString("p") + "\t" + p);
                if (File.Exists(p)) callAction(p);
            });
        }
    }
}
