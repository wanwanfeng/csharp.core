using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Library.Extensions;
using Library.Helper;

namespace fileUtils
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SystemConsole.Run(config: new Dictionary<string, Action>()
            {
                {"Merge", new Merge().Run},
                {"Down", new Down().Run},
            });
        }

        public class Merge : BaseSystemConsole
        {
            public void Run()
            {
                var xx = CheckPath("*.*", SelectType.Folder)
                    .SelectMany(p => File.ReadAllBytes(p))
                    .ToArray();
                File.WriteAllBytes(Path.ChangeExtension(InputPath.Trim('.'), ".ts"), xx);
            }
        }

        public class Down : BaseSystemConsole
        {
            public void Run()
            {
                var url = SystemConsole.GetInputStr("输入下载地址：");

                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(url);
                if (string.IsNullOrEmpty(fileNameWithoutExtension)) return;
                url = url.Replace(fileNameWithoutExtension, "{0}");
                int start = fileNameWithoutExtension
                    .Reverse()
                    .TakeWhile(char.IsDigit)
                    .Aggregate("", (b, a) => a + b)
                    .AsInt();
                string qianzhui = fileNameWithoutExtension
                    .Reverse()
                    .SkipWhile(char.IsDigit)
                    .Aggregate("", (b, a) => a + b);
                Enumerable.Range(start, 500)
                    .Select(p => string.Format(url, qianzhui + p))
                    .Select(p =>
                    {
                        DownLoad(p);
                        return p;
                    }).ToList();
                //.AsParallel()
                //.WithDegreeOfParallelism(4)
                //.ForAll(DownLoad);
            }

            public void DownLoad(string url)
            {
                var uri = new Uri(url);
                string newName = uri.LocalPath.TrimStart('/');
                string tempName = Path.ChangeExtension(newName, "temp");
                FileHelper.CreateDirectory(newName);               

                HttpWebRequest request;
                HttpWebResponse response;
                try
                {
                    Console.WriteLine(url);
                    if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                    {
                        ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                        request = WebRequest.Create(url) as HttpWebRequest;
                        request.ProtocolVersion = HttpVersion.Version10;
                        //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)";
                        //request.Method = "GET";
                        //发送请求并获取相应回应数据
                        response = request.GetResponse() as HttpWebResponse;
                        string cookieString = response.Headers["Set-Cookie"];
                        CookieCollection cookies = new CookieCollection();
                        Regex re = new Regex("([^;,]+)=([^;,]+); path=([^;,]+); expires=([^;,]+)",
                            RegexOptions.IgnoreCase);
                        //视具体内容进行调整
                        foreach (Match m in re.Matches(cookieString))
                        {
                            Cookie c = new Cookie(m.Groups[1].Value, m.Groups[2].Value);
                            c.Domain = uri.Authority; //放你要访问网站的域名
                            cookies.Add(c);
                        }
                    }
                    else
                    {
                        request = WebRequest.Create(url) as HttpWebRequest;
                        //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)";
                        //request.Method = "GET";

                        //发送请求并获取相应回应数据
                        response = request.GetResponse() as HttpWebResponse;
                    }

                    //直到request.GetResponse()程序才开始向目标网页发送Post请求
                    if (response == null) return;
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var fs = new FileStream(tempName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        {
                            //Stream stream = new FileStream(tempFile, FileMode.Create);
                            byte[] bArr = new byte[1024];
                            if (responseStream != null)
                            {
                                int size = responseStream.Read(bArr, 0, (int) bArr.Length);
                                while (size > 0)
                                {
                                    //stream.Write(bArr, 0, size);
                                    fs.Write(bArr, 0, size);
                                    size = responseStream.Read(bArr, 0, (int) bArr.Length);
                                }
                            }
                        }
                        File.Move(tempName, newName);
                    }
                }
                catch (Exception ex)
                {
                    if (File.Exists(tempName))
                        File.Delete(tempName); //存在则删除
                    Console.WriteLine(ex.StackTrace);
                }
            }

            private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
                SslPolicyErrors errors)
            {
                return true; //总是接受
            }
        }
    }
}
