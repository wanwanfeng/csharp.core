using System;
using System.Security.Cryptography;
using System.Text;

namespace Encrypt
{
    internal class MD5Utils
    {
        public static string Encrypt(string str)
        {
            byte[] result = Encoding.Default.GetBytes(str); //输入文本
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", ""); //输出加密文本
        }

        public static string Encrypt(string str, string key)
        {
            return Encrypt(str + key);
        }
    }
}
