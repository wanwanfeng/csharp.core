using System;
using System.Collections.Generic;
using System.IO;

namespace encrypt
{
    public class test
    {
        public static void GetValue(string[] args)
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
                    res = Library.Encrypt.MD5(input);
                }
                    break;
                case "aes":
                {
                    string input = queue.Count == 0 ? "" : queue.Dequeue();
                    string key = queue.Count == 0 ? "" : queue.Dequeue();
                    string third = queue.Count == 0 ? "-e" : queue.Dequeue();
                    Library.AES.Key = key;
                    var resBytes = third == "-d"
                        ? Library.Dencrypt.AES(File.ReadAllText(input))
                        : Library.Encrypt.AES(File.ReadAllText(input));
                    File.WriteAllText(input, resBytes);
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