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

        //MD5加密  
        public static string Encrypt(string input)
        {
            return Encrypt(Encoding.UTF8.GetBytes(input));
        }

        //MD5加密  
        public static string Encrypt(byte[] input)
        {
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

        /// <summary>
        ///  AES-256  内容加密
        /// 成功加密时返回值
        /// 否则返回空
        /// </summary>
        public static string Encrypt(string toE)
        {
            if (string.IsNullOrEmpty(toE)) return toE;
            if (!string.IsNullOrEmpty(Head) && toE.Substring(toE.Length - Head.Length, Head.Length) == Head) return toE;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toE);

            //加密和解密采用相同的key,具体自己填，但是必须为32位//
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(Key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rm.CreateEncryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length) + Head;
        }

        /// <summary>
        /// AES-256 内容解密
        /// </summary>
        public static string Decrypt(string toD)
        {
            if (string.IsNullOrEmpty(toD)) return toD;
            if (!string.IsNullOrEmpty(Head) && toD.Substring(toD.Length - Head.Length, Head.Length) != Head) return toD;
            toD = toD.Substring(0, toD.Length - Head.Length);
            byte[] toEncryptArray = Convert.FromBase64String(toD);

            //加密和解密采用相同的key,具体值自己填，但是必须为32位//
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(Key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rm.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
