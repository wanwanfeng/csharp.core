using System;
using System.Collections.Generic;
using System.IO;

namespace encrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                test.GetValue(args);
                return;
            }

            Console.WriteLine("---------------------------");
            Console.WriteLine("1;加密");
            Console.WriteLine("2;解密");
            Console.WriteLine("---------------------------");

            Console.Write("输入指令：");
            var cmd = Console.ReadLine() ?? "e";
            switch (cmd)
            {
                case "1":
                    new YueGeEncrypt();
                    break;
                case "2":
                    new YueGeDencrypt();
                    break;
            }
            Environment.Exit(0);
            return;

            var list = new[,]
            {
                {"md5,image"},
                {"md5,ff277dcb965c4e0b1bc16d38999e417a.jpg"}
            };
            foreach (string s in list)
            {
                test.GetValue(s.Split(','));
            }
            Console.ReadKey();
        }
    }
}
