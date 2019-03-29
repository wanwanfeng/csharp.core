using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;

namespace encrypt
{
    static class Program
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
                SystemConsole.Run(config: new Dictionary<string, Action>()
                {
                    {"加密", () => { new YueGeEncrypt(); }},
                    {"解密", () => { new YueGeDencrypt(); }},
                    {"解密2", () => { new YueGeDencrypt2(); }}
                });
            } while (SystemConsole.ContinueY());

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

        public static void ForEachPaths(this IEnumerable<string> paths, Action<string> callAction)
        {
            paths.Select(p => p.Replace("\\", "/")).ToList().ForEach((p, i, target) =>
            {
                Console.WriteLine("is now : " + (((float) i) / target.Count).ToString("p") + "\t" + p);
                if (File.Exists(p)) callAction(p);
            });
        }
    }
}
