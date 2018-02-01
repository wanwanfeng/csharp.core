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
                sBuilder.Append(bt.ToString("x2"));
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
        /// <returns></returns>
        public static bool ComparerString(string hash, string str)
        {
            return Encrypt(str) == hash;
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
        /// 成功加密时返回值
        /// 否则返回空
        /// </summary>
        public static string Encrypt(string toE, string key = null)
        {
            if (string.IsNullOrEmpty(toE)) return null;
            if (toE.Substring(0, Head.Length) == Head) return null;
            byte[] resultArray = Encrypt(Encoding.UTF8.GetBytes(toE), key);
            return Head + Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        ///  AES-256  内容加密
        /// </summary>
        public static byte[] Encrypt(byte[] toEncryptArray, string key = null)
        {
            if (!IsOpen) return toEncryptArray;

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
            if (string.IsNullOrEmpty(toD)) return null;
            if (toD.Substring(0, Head.Length) != Head) return null;
            toD = toD.Substring(Head.Length, toD.Length - Head.Length);
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(toD), key));
        }

        /// <summary>
        /// AES-256 内容解密
        /// </summary>
        public static byte[] Decrypt(byte[] toEncryptArray, string key = null)
        {
            if (!IsOpen) return toEncryptArray;

            //加密和解密采用相同的key,具体值自己填，但是必须为32位//
            byte[] keyArray = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(key) ? Key : key);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();

            return cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        }
    }
}
