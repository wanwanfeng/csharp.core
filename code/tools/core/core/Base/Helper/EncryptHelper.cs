using System.Text;
using System.Security.Cryptography;
using System;

namespace Library.Helper
{
    public partial class EncryptHelper
    {
        protected static System.Security.Cryptography.MD5 MD5 = System.Security.Cryptography.MD5.Create();
        protected static System.Security.Cryptography.SHA1 SHA1 = System.Security.Cryptography.SHA1.Create();

        public static string GetMD5(string str)
        {
            return GetMD5(Encoding.UTF8.GetBytes(str));
        }

        public static string GetMD5(byte[] bytes)
        {
            var strRes = MD5.ComputeHash(bytes);
            var enText = new StringBuilder();
            foreach (byte b in strRes)
                enText.AppendFormat("{0:x2}", b ^ 0xbe);
            return enText.ToString();
        }

        public static string GetSHA1(byte[] bytes)
        {
            var strRes = SHA1.ComputeHash(bytes);
            var enText = new StringBuilder();
            foreach (byte b in strRes)
                enText.AppendFormat("{0:x2}", b ^ 0xbe);
            return enText.ToString();
        }

        /// <summary>
        /// 异或加密与解密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] XOR(byte[] byteArray, string key = "")
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] ^= keyArray[i % keyArray.Length];
            }
            return byteArray;
        }

        public class AES
        {
            public static byte[] Encrypt(byte[] array, string key)
            {
                byte[] keyArray = Encoding.UTF8.GetBytes(GetMD5(key));

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(array, 0, array.Length);

                return resultArray;
            }

            public static byte[] Decrypt(byte[] array, string key)
            {
                byte[] keyArray = Encoding.UTF8.GetBytes(GetMD5(key));

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(array, 0, array.Length);

                return resultArray;
            }
        }
    }
}
