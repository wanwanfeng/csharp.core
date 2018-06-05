using System;
using findText.Script;
using Library.Extensions;
using Library.Helper;

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

                var cmd = SystemExtensions.GetInputStr();
                do
                {
                    switch (cmd)
                    {
                        case "1":
                            baseActionFor.Open(SystemExtensions.GetInputStr("input dir path:"));
                            break;
                        case "2":
                            baseActionFor.Revert(SystemExtensions.GetInputStr("iinput file path (*.xlsx):"));
                            break;
                    }
                    Console.ReadKey();
                } while (SystemExtensions.ContinueY());
            };
            SystemConsole.Run<ConvertType>(callFunc);
        }
    }
}
