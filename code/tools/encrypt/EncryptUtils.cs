using System;
using System.Security.Cryptography;
using System.Text;

namespace encrypt
{
    class EncryptUtils
    {
        //MD5加密  
        public static string GetMd5Hash(string input, string key = "", bool isHead = true)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (isHead)
                    input = key + input;
                else
                    input = input + key;
            }
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            foreach (var bt in data)
            {
                sBuilder.Append(bt.ToString("x2"));
            }
            return sBuilder.ToString();
        }

        // AES-256 加密  
        public static string Encrypt(string toEncrypt, string key = "")
        {
            var resultArray = Encrypt(Encoding.UTF8.GetBytes(toEncrypt), key);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        // AES-256 加密  
        public static byte[] Encrypt(byte[] toDecrypt, string key = "")
        {
            if (string.IsNullOrEmpty(key))
                key = "12345678901234567890123456789012";
            // 256-AES key      
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = toDecrypt;

            RijndaelManaged rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            return cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        }

        //AES-256 解密  
        public static string Decrypt(string toDecrypt, string key)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(toDecrypt), key));
        }

        //AES-256 解密  
        public static byte[] Decrypt(byte[] toDecrypt, string key)
        {
            if (string.IsNullOrEmpty(key))
                key = "12345678901234567890123456789012";
            // 256-AES key      
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = toDecrypt;

            RijndaelManaged rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                //Padding = PaddingMode.Zeros,
                //KeySize = 128,
                //BlockSize = 128
            };

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            return cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        }
    }
}
