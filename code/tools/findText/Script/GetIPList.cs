using System.Collections.Generic;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using Library;
using Library.Helper;
using LitJson;

namespace findText.Script
{
    public class GetIPList : BaseActionFor
    {
        public GetIPList()
        {
            string url = "https://www.kuaidaili.com/free/", path = Path.GetTempFileName();
            path = HttpDownloadFile(url, path);
            HtmlDocument doc = new HtmlDocument();
            string content = File.ReadAllText(path);
            doc.LoadHtml(content);



            HtmlNodeCollection headList = doc.DocumentNode.SelectNodes("//*[@id=\"list\"]/table/thead/tr/th");
            HtmlNodeCollection valueList = doc.DocumentNode.SelectNodes("//*[@id=\"list\"]/table/tbody/tr");

            var res = new ListTable();

            foreach (HtmlNode node in headList)
            {
                res.Columns.Add(node.InnerText);
            }

            foreach (HtmlNode node in valueList)
            {
                var temp = new List<object>();
                foreach (HtmlNode child in node.SelectNodes("td"))
                {
                    temp.Add(child.InnerText);
                }
                res.Rows.Add(temp);
            }


            //var childs = doc.DocumentNode.Descendants().Where(p => !p.HasChildNodes).Where(p => p.XPath.Contains("tbody")).ToDictionary(p => p.XPath);
            //string xPath = "//*[@id=\"list\"]/table/tbody";
            //HtmlNode oldNode = null;
            //childs.TryGetValue(xPath, out oldNode);

            File.WriteAllText("temp.txt", JsonHelper.ToJson((JsonData)res, indentLevel: 2));
        }


        /// <summary>
        /// Http下载文件
        /// </summary>
        public static string HttpDownloadFile(string url, string path)
        {
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

        protected override void OpenRun(string file)
        {
            
        }
    }
}