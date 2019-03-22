using System;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace search.Script
{
    public class BaseSearch
    {
        protected virtual string[] urls { get; set; }

        //public BaseSearch()
        //{
        //    if (urls.Length > 1) return;

        //    //string path = Path.GetTempFileName();
        //    //path = HttpDownloadFile(s, path);
        //    //HtmlDocument doc = new HtmlDocument();
        //    //string content = File.ReadAllText(path);
        //    //doc.LoadHtml(content);


        //    HtmlWeb webClient = new HtmlWeb();
        //    HtmlDocument doc = webClient.Load(urls.First()); 

        //    Run(doc);
        //}


        //public virtual void Run(HtmlDocument doc)
        //{
            
        //}

        /// <summary>
        /// Http下载文件
        /// </summary>
        public static string HttpDownloadFile(string url, string path)
        {
            Console.WriteLine("下载：" + url);
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            //创建本地文件写入流
            Stream stream = new FileStream(path, FileMode.Create);
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
            return path;
        }
    }
}