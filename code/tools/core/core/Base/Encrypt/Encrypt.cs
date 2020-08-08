using System.Text;
using System.Security.Cryptography;
using System;
using Library.Extensions;
using System.Linq;

namespace Library.Extensions
{
    public static class EncryptExtensions
    {
        public static Encoding DefaultEncoding = Encoding.UTF8;

        #region MD516

        public static string MD516(this string input, string key = "")
        {
            return input.MD5(key).Substring(8, 24);
        }

        public static string MD516(this byte[] input, string key = "")
        {
            return input.MD5(key).Substring(8, 24);
        }

        public static bool ComparerMD516(this string hash, string input, string key = "")
        {
            return hash.MD516(key).Equals(input.MD516(key));
        }

        public static bool ComparerMD516(this string hash, byte[] input, string key = "")
        {
            return hash.MD516(key).Equals(input.MD516(key));
        }

        #endregion

        #region MD5

        public static string MD5(this string input, string key = "")
        {
            return new MD5().Encrypt(input, key);
        }

        public static string MD5(this byte[] input, string key = "")
        {
            return new MD5().Encrypt(input, key);
        }

        public static bool ComparerMD5(this string hash, string input, string key = "")
        {
            return hash.MD5(key).Equals(input.MD5(key));
        }

        public static bool ComparerMD5(this string hash, byte[] input, string key = "")
        {
            return hash.MD5(key).Equals(input.MD5(key));
        }

        #endregion

        #region SHA1

        public static string SHA1(this string input, string key = "")
        {
            return new SHA1().Encrypt(input, key);
        }

        public static string SHA1(this byte[] input, string key = "")
        {
            return new SHA1().Encrypt(input, key);
        }

        public static bool ComparerSHA1(this string hash, string input, string key = "")
        {
            return hash.SHA1(key).Equals(input.SHA1(key));
        }

        public static bool ComparerSHA1(this string hash, byte[] input, string key = "")
        {
            return hash.SHA1(key).Equals(input.SHA1(key));
        }

        #endregion

        #region AES

        public static string AES_Encrypt(this string input, string key = "")
        {
            return new AES().Encrypt(input, key);
        }

        public static string AES_Encrypt(this byte[] input, string key = "")
        {
            return new AES().Encrypt(input, key);
        }

        public static string AES_Dencrypt(this string input, string key = "")
        {
            return new AES().Dencrypt(input, key);
        }

        public static string AES_Dencrypt(this byte[] input, string key = "")
        {
            return new AES().Dencrypt(input, key);
        }

        #endregion

        #region XOR

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


        #endregion
    }
}

namespace Library
{
    internal interface IEncrypt
    {
        string Encrypt(byte[] input, string key = "");
        string Encrypt(string input, string key = "");
    }

    internal interface IDencrypt
    {
        string Dencrypt(byte[] input, string key = "");
        string Dencrypt(string input, string key = "");
    }

    public abstract class BaseEncrypt : IEncrypt
    {
        public Encoding DefaultEncoding
        {
            get { return Extensions.EncryptExtensions.DefaultEncoding; }
        }

        /// <summary>
        /// 加密  
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual string Encrypt(byte[] input, string key = "")
        {
            return Encrypt(DefaultEncoding.GetString(input), key);
        }

        /// <summary>
        /// 加密  
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract string Encrypt(string input, string key = "");
    }

    /// <summary>
    /// 加密工具
    /// </summary>
    internal class SHA1 : BaseEncrypt
    {
        //SHA1加密  
        public override string Encrypt(string input, string key = "")
        {
            try
            {
                byte[] bytes = DefaultEncoding.GetBytes(input + key);
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                byte[] data = sha1.ComputeHash(bytes);
                (sha1 as IDisposable).Dispose();
                string result = BitConverter.ToString(data);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }
    }

    /// <summary>
    /// 加密工具
    /// </summary>
    internal class MD5 : BaseEncrypt
    {
        //MD5加密  
        public override string Encrypt(string input, string key = "")
        {
            try
            {
                byte[] bytes = DefaultEncoding.GetBytes(input + key);
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] data = md5.ComputeHash(bytes);
                (md5 as IDisposable).Dispose();
                var sBuilder = new StringBuilder();
                foreach (var bt in data)
                    sBuilder.Append(bt.ToString("x2"));
                return sBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("MD5加密出错：" + ex.Message);
            }
        }
    }

    /// <summary>
    /// 加密解密工具
    /// </summary>
    internal class AES : BaseEncrypt, IDencrypt
    {
        //加密和解密采用相同的key,具体值自己填，但是必须为32位//
        public static string Head = "5986157849545freho950173582krhd0";

        /// <summary>
        /// AES-256  内容加密
        /// 成功加密时返回值
        /// 否则返回空
        /// </summary>
        public override string Encrypt(string input, string key = "")
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (!string.IsNullOrEmpty(Head))
                if (input.Length > Head.Length)
                    //已经加密
                    if (input.Substring(input.Length - Head.Length, Head.Length) == Head)
                        return input;


            byte[] toEncryptArray = DefaultEncoding.GetBytes(input);

            //加密和解密采用相同的key,具体自己填，但是必须为32位//
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = DefaultEncoding.GetBytes(key.MD5()),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rm.CreateEncryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length) + Head;
        }

        public string Dencrypt(byte[] input, string key = "")
        {
            return Dencrypt(DefaultEncoding.GetString(input), key);
        }

        /// <summary>
        /// AES-256 内容解密
        /// </summary>
        public string Dencrypt(string input, string key = "")
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (!string.IsNullOrEmpty(Head))
                if (input.Length > Head.Length)
                    //已经解密
                    if (input.Substring(input.Length - Head.Length, Head.Length) != Head)
                        return input;
            input = input.Substring(0, input.Length - Head.Length);
            byte[] toEncryptArray = Convert.FromBase64String(input);

            //加密和解密采用相同的key,具体值自己填，但是必须为32位//
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = DefaultEncoding.GetBytes(key.MD5()),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rm.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return DefaultEncoding.GetString(resultArray);
        }
    }
}
