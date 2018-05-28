using System.Text;
using System.Security.Cryptography;
using System;

namespace Library
{
    public static class Encrypt
    {
        public static Encoding DefaultEncoding = Encoding.UTF8;

        public static string MD5(string input)
        {
            return new MD5().Encrypt(input);
        }

        public static bool ComparerMD5(string hash, string input)
        {
            return new MD5().Comparer(hash, input);
        }

        public static string MD5(byte[] input)
        {
            return new MD5().Encrypt(input);
        }

        public static bool ComparerMD5(string hash, byte[] input)
        {
            return new MD5().Comparer(hash, input);
        }

        public static string SHA1(string input)
        {
            return new SHA1().Encrypt(input);
        }

        public static bool ComparerSHA1(string hash, string input)
        {
            return new SHA1().Comparer(hash, input);
        }

        public static string SHA1(byte[] input)
        {
            return new SHA1().Encrypt(input);
        }

        public static bool ComparerSHA1(string hash, byte[] input)
        {
            return new SHA1().Comparer(hash, input);
        }

        public static string AES(string input)
        {
            return new AES().Encrypt(input);
        }

        public static string AES(byte[] input)
        {
            return new AES().Encrypt(input);
        }
    }

    public class Dencrypt
    {
        public static string AES(string input)
        {
            return new AES().Dencrypt(input);
        }

        public static string AES(byte[] input)
        {
            return new AES().Dencrypt(input);
        }
    }

    public interface IEncrypt
    {
        string Encrypt(byte[] input);
        string Encrypt(string input);
        bool Comparer(string hash, byte[] str);
        bool Comparer(string hash, string str);
    }

    public interface IDencrypt
    {
        string Dencrypt(byte[] input);
        string Dencrypt(string input);
    }

    public abstract class BaseEncrypt : IEncrypt
    {
        public Encoding DefaultEncoding
        {
            get { return Library.Encrypt.DefaultEncoding; }
        }

        /// <summary>
        /// 加密  
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public abstract string Encrypt(byte[] input);

        /// <summary>
        /// 加密  
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual string Encrypt(string input)
        {
            return Encrypt(DefaultEncoding.GetBytes(input));
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool Comparer(string hash, string input)
        {
            return Encrypt(input) == hash;
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool Comparer(string hash, byte[] input)
        {
            return Encrypt(input) == hash;
        }
    }

    /// <summary>
    /// 加密解密工具
    /// </summary>
    public class SHA1 : BaseEncrypt
    {
        //SHA1加密  
        public override string Encrypt(byte[] input)
        {
            try
            {
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                byte[] data = sha1.ComputeHash(input);
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
    /// 加密解密工具
    /// </summary>
    public class MD5 : BaseEncrypt
    {
        //MD5加密  
        public override string Encrypt(byte[] input)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] data = md5.ComputeHash(input);
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
    public class AES : BaseEncrypt, IDencrypt
    {
        //加密和解密采用相同的key,具体值自己填，但是必须为32位//
        public static string Head = "5986157849545freho950173582krhd0";
        public static string Key = "59861578495186345095017358264920";

        public override string Encrypt(byte[] input)
        {
            return Encrypt(DefaultEncoding.GetString(input));
        }

        /// <summary>
        ///  AES-256  内容加密
        /// 成功加密时返回值
        /// 否则返回空
        /// </summary>
        public override string Encrypt(string toE)
        {
            if (string.IsNullOrEmpty(toE)) return toE;
            if (!string.IsNullOrEmpty(Head))
                if (toE.Length > Head.Length)
                    //已经加密
                    if (toE.Substring(toE.Length - Head.Length, Head.Length) == Head)
                        return toE;


            byte[] toEncryptArray = DefaultEncoding.GetBytes(toE);

            //加密和解密采用相同的key,具体自己填，但是必须为32位//
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = DefaultEncoding.GetBytes(new MD5().Encrypt(Key)),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rm.CreateEncryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length) + Head;
        }

        public string Dencrypt(byte[] input)
        {
            return Dencrypt(DefaultEncoding.GetString(input));
        }

        /// <summary>
        /// AES-256 内容解密
        /// </summary>
        public string Dencrypt(string toD)
        {
            if (string.IsNullOrEmpty(toD)) return toD;
            if (!string.IsNullOrEmpty(Head))
                if (toD.Length > Head.Length)
                    //已经解密
                    if (toD.Substring(toD.Length - Head.Length, Head.Length) != Head)
                return toD;
            toD = toD.Substring(0, toD.Length - Head.Length);
            byte[] toEncryptArray = Convert.FromBase64String(toD);

            //加密和解密采用相同的key,具体值自己填，但是必须为32位//
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = DefaultEncoding.GetBytes(new MD5().Encrypt(Key)),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rm.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return DefaultEncoding.GetString(resultArray);
        }
    }
}
