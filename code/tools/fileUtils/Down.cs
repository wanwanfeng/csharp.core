using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Library.Extensions;
using Library.Helper;

namespace fileUtils
{
    public abstract class Down : BaseSystemConsole
    {
         protected string RegexUrl = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";

        public virtual void Run()
        {

        }

        public string[] GetM3U8(string url)
        {
            return GetWebFileContent(url).Where(p => !p.StartsWith("#")).ToArray();
        }

        /// <summary>
        /// 暂放在临时文件夹
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string[] GetWebFileContent(string url)
        {
            var tempPath = DownLoad(url);
            return string.IsNullOrEmpty(tempPath) ? new string[0] : File.ReadAllLines(tempPath);
        }

        /// <summary>
        /// 已经永久保存
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetWebFile(string url)
        {
            var uri = new Uri(url);
            string newName = uri.LocalPath.TrimStart('/');
            if (File.Exists(newName)) return newName;

            var tempPath = DownLoad(url);

            if (string.IsNullOrEmpty(tempPath))
                return null;

            DirectoryHelper.CreateDirectory(newName);
            File.Move(tempPath, newName);
            return newName;
        }

        /// <summary>
        /// 下载到临时文件夹，返回临时路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string DownLoad(string url)
        {
            string tempName = Path.GetTempFileName();

            var uri = new Uri(url);

            try
            {
                HttpWebRequest request;
                HttpWebResponse response;

                switch (uri.Scheme)
                {
                    case "https":
                    {
                        ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                        request = WebRequest.CreateHttp(url);
                        request.ProtocolVersion = HttpVersion.Version10;
                        //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)";
                        //request.Method = "GET";
                        //发送请求并获取相应回应数据
                        response = request.GetResponse() as HttpWebResponse;
                        //string cookieString = response.Headers["Set-Cookie"];
                        //CookieCollection cookies = new CookieCollection();
                        //Regex re = new Regex("([^;,]+)=([^;,]+); path=([^;,]+); expires=([^;,]+)",RegexOptions.IgnoreCase);
                        ////视具体内容进行调整
                        //foreach (Match m in re.Matches(cookieString))
                        //{
                        //    Cookie c = new Cookie(m.Groups[1].Value, m.Groups[2].Value);
                        //    c.Domain = uri.Authority; //放你要访问网站的域名
                        //    cookies.Add(c);
                        //}
                    }
                        break;
                    case "http":
                    default:
                    {
                        request = WebRequest.CreateHttp(url);
                        ;
                        //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)";
                        //request.Method = "GET";

                        //发送请求并获取相应回应数据
                        response = request.GetResponse() as HttpWebResponse;
                    }
                        break;
                }

                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                if (response == null) return null;

                using (var fs = new FileStream(tempName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null) responseStream.CopyTo(fs);
                    }
                }

                return tempName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
                return null;
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
        {
            return true; //总是接受
        }
    }
}