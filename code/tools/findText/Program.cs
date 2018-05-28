using System;
using Library.Extensions;
using Library.Helper;

namespace findText
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("-------语言列表-------");
            foreach (var value in Enum.GetValues(typeof (ConvertType)))
            {
                Console.WriteLine(" " + (int) value + ":" + value);
            }
            Console.WriteLine("\n e:退出");
            Console.WriteLine("----------------------");

            var cache = AttributeHelper.GetCacheTypeValue<ConvertType>();
            ConvertType convertType = (ConvertType) SystemExtensions.GetInputStr().AsInt();

            BaseActionFor baseActionFor = (BaseActionFor) Activator.CreateInstance(cache[convertType]);
            Console.WriteLine("-------操作列表-------");
            Console.WriteLine("1:搜索");
            Console.WriteLine("2:还原");
            Console.WriteLine("e:退出");
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
            } while (SystemExtensions.Continue());
        }
    }
}
