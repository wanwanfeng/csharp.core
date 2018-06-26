using System;
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
            [TypeValue(typeof (ActionForCpp))] cpp,
            [TypeValue(typeof (ActionForCSharp))] csharp,
            [TypeValue(typeof (ActionForPhp))] php,
            [TypeValue(typeof (ActionForJava))] java,
            [TypeValue(typeof (ActionForJavaScript))] javascript,
            [TypeValue(typeof (ActionForHtml))] html,
            [TypeValue(typeof (ActionForHtml2))][Description("html 插件")] html2,
        }

        private static void Main(string[] args)
        {
            Action<object> callFunc = obj =>
            {
                BaseActionFor baseActionFor = (BaseActionFor) obj;

                Console.WriteLine("-------操作列表-------");
                Console.WriteLine("1:搜索");
                Console.WriteLine("2:还原");
                Console.WriteLine("----------------------");

                var cmd = SystemConsole.GetInputStr();
                do
                {
                    switch (cmd)
                    {
                        case "1":
                            baseActionFor.Open(SystemConsole.GetInputStr("input dir path:"));
                            break;
                        case "2":
                            baseActionFor.Revert(SystemConsole.GetInputStr("iinput file path (*.xlsx):"));
                            break;
                    }
                    Console.ReadKey();
                } while (SystemConsole.ContinueY());
            };
            SystemConsole.Run<ConvertType>(callFunc);
        }
    }
}
