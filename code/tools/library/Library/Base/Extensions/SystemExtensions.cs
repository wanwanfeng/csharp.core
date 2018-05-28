using System;

namespace Library.Extensions
{
    public class SystemExtensions
    {
        /// <summary>
        /// 用于控制台程序
        /// </summary>
        /// <returns></returns>
        public static string GetInputStr(string beforeTip = "input:", string afterTip = "", string def = "e")
        {
            Console.Write(beforeTip);
            var str = Console.ReadLine() ?? def;
            if (str == "e" || string.IsNullOrEmpty(str))
            {
                Console.WriteLine("按任意键退出！");
                Console.ReadKey();
                Environment.Exit(0);
            }
            str = str.Contains(" ") ? str.Substring(1, str.Length - 2) : str;
            Console.WriteLine(afterTip + str);
            return str;
        }

        public static string WriteLine(string msg)
        {
            var path = Console.ReadLine() ?? "";
            return path.Contains(" ") ? path.Substring(1, path.Length - 2) : path;
        }
    }
}