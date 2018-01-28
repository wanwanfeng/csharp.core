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
                GetValue(args);
                return;
            }

            var list = new[,]
            {
                {"md5,image"},
                {"md5,ff277dcb965c4e0b1bc16d38999e417a.jpg"}
            };
            foreach (string s in list)
            {
                GetValue(s.Split(','));
            }
            Console.ReadKey();
        }

        private static void GetValue(string[] args)
        {
            Queue<string> queue = new Queue<string>(args);
            string type = queue.Count == 0 ? "" : queue.Dequeue();
            if (string.IsNullOrEmpty(type))
                return;

            string res = "";
            switch (type)
            {
                case "md5":
                {
                    string input = queue.Count == 0 ? "" : queue.Dequeue();
                    res = Library.Encrypt.MD5.Encrypt(input);
                }
                    break;
                case "aes":
                {
                    string input = queue.Count == 0 ? "" : queue.Dequeue();
                    string key = queue.Count == 0 ? "" : queue.Dequeue();
                    string third = queue.Count == 0 ? "-e" : queue.Dequeue();
                    var bytes = File.ReadAllBytes(input);
                    var resBytes = third == "-d"
                        ? Library.Encrypt.AES.Decrypt(bytes, key)
                        : Library.Encrypt.AES.Encrypt(bytes, key);
                    File.WriteAllBytes(input, resBytes);
                }
                    break;
                case "help":
                {
                    res = @"
输入格式（类型，源串，密钥，其他）
MD5加密（string:md5,string:input,string:[key],string[-h|-e]）
AES加密（string:aes,string:input,string:[key],string[-e]）
AES解密（string:aes,string:input,string:[key],string[-d]）
";
                }
                    break;
            }
            Console.WriteLine(res);
        }
    }
}
