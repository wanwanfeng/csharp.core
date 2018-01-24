using System.Text;
using System.Security.Cryptography;
using System;
using System.IO;

namespace Library.Encrypt
{
    /// <summary>
    /// 加密解密工具
    /// </summary>
    public class MD5
    {
        public static System.Security.Cryptography.MD5 Md5Hash = System.Security.Cryptography.MD5.Create();
        public static bool IsOpen = true;

        //MD5加密  
        public static string Encrypt(string input, string key, bool isHead = true)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (isHead)
                    input = key + input;
                else
                    input = input + key;
            }
            return Encrypt(input);
        }

        //MD5加密  
        public static string Encrypt(byte[] input, string key, bool isHead = true)
        {
            return Encrypt(Encoding.UTF8.GetString(input));
        }

        //MD5加密  
        public static string Encrypt(string input)
        {
            return Encrypt(Encoding.UTF8.GetBytes(input));
        }

        //MD5加密  
        public static string Encrypt(byte[] input)
        {
            if (!IsOpen)
                return Encoding.UTF8.GetString(input);
            byte[] data = Md5Hash.ComputeHash(input);
            var sBuilder = new StringBuilder();
            foreach (var bt in data)
            {
                sBuilder.Append(bt.ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 校验文件MD5
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ComparerFile(string hash, string path)
        {
            return File.Exists(path) && Encrypt(File.ReadAllBytes(path)) == hash;
        }

        /// <summary>
        /// 校验字符串MD5
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <param name="isHead"></param>
        /// <returns></returns>
        public static bool ComparerString(string hash, string str, string key = "", bool isHead = true)
        {
            return Encrypt(str, key, isHead) == hash;
        }
    }

    /// <summary>
    /// 加密解密工具
    /// </summary>
    public class AES
    {
        //加密和解密采用相同的key,具体值自己填，但是必须为32位//
        public static string Head = "5986157849545freho950173582krhd0";
        public static string Key = "59861578495186345095017358264920";
        public static bool IsOpen = true;

        /// <summary>
        ///  AES-256  内容加密
        /// </summary>
        public static string Encrypt(string toE, string key = null)
        {
            byte[] resultArray = Encrypt(Encoding.UTF8.GetBytes(toE), key);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        ///  AES-256  内容加密
        /// </summary>
        public static byte[] Encrypt(byte[] toEncryptArray, string key = null)
        {
            if (!IsOpen)
                return toEncryptArray;
            //加密和解密采用相同的key,具体自己填，但是必须为32位//
            byte[] keyArray = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(key) ? Key : key);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();

            return cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        }

        /// <summary>
        /// AES-256 内容解密
        /// </summary>
        public static string Decrypt(string toD, string key = null)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(toD), key));
        }

        /// <summary>
        /// AES-256 内容解密
        /// </summary>
        public static byte[] Decrypt(byte[] toEncryptArray, string key = null)
        {
            if (!IsOpen)

                return toEncryptArray;
            //加密和解密采用相同的key,具体值自己填，但是必须为32位//
            byte[] keyArray = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(key) ? Key : key);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();

            return cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        }

        /// <summary>
        /// 内容加密
        /// </summary>
        public static string Encrypt(string toE, out bool isEncrypt, string key = null)
        {
            if (string.IsNullOrEmpty(toE))
            {
                isEncrypt = false;
                return toE;
            }
            isEncrypt = toE.Substring(0, 32) != Head;
            return isEncrypt ? (Head + Encrypt(toE, key)) : toE;
        }

        /// <summary>
        /// 内容解密
        /// </summary>
        public static string Decrypt(string toD, out bool isDecrypt, string key = null)
        {
            if (string.IsNullOrEmpty(toD))
            {
                isDecrypt = false;
                return toD;
            }
            isDecrypt = toD.Substring(0, 32) == Head;
            return isDecrypt ? Decrypt(toD.Substring(32, toD.Length - 32), key) : toD;
        }
    }
}
