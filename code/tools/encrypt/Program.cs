using System;
using System.Collections.Generic;
using System.IO;
using Library.Extensions;

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

            do
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine("1;加密");
                Console.WriteLine("2;解密");
                Console.WriteLine("---------------------------");

                var cmd = SystemExtensions.GetInputStr("输入指令：");
                do
                {
                    switch (cmd)
                    {
                        case "1":
                            new YueGeEncrypt();
                            break;
                        case "2":
                            new YueGeDencrypt();
                            break;
                    }
                } while (SystemExtensions.Continue());
            } while (SystemExtensions.Continue());

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
