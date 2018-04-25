using System;
using Library.Extensions;
using Library.Helper;

namespace findText
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string cmd;
            Console.WriteLine("-------语言列表-------");
            foreach (var value in Enum.GetValues(typeof (ConvertType)))
            {
                Console.WriteLine(" " + (int) value + ":" + value);
            }
            Console.WriteLine("\n e:退出");
            Console.WriteLine("----------------------");
            Console.Write("input:");
            cmd = Console.ReadLine() ?? "e";
            if (cmd == "e") Environment.Exit(0);

            var cache = AttributeHelper.GetCacheTypeValue<ConvertType>();
            ConvertType convertType = (ConvertType)cmd.AsInt();
            BaseActionFor baseActionFor = (BaseActionFor) Activator.CreateInstance(cache[convertType]);
            Console.WriteLine("-------操作列表-------");
            Console.WriteLine("1:搜索");
            Console.WriteLine("2:还原");
            Console.WriteLine("e:退出");
            Console.Write("input:");
            cmd = Console.ReadLine() ?? "e";
            switch (cmd)
            {
                case "1":
                {
                    Console.Write("input dir path;");
                    string path = Console.ReadLine() ?? "";
                    baseActionFor.Open(path);
                }
                    break;
                case "2":
                {
                    Console.Write("input file path (*.xlsx);");
                    string path = Console.ReadLine() ?? "";
                    baseActionFor.Revert(path);
                }
                    break;
                default:
                    Environment.Exit(0);
                    break;
            }
            Console.ReadKey();
        }
    }
}
