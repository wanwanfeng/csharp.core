using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using HtmlAgilityPack;
using Library;
using Library.Helper;
using LitJson;

namespace search.Script
{
    public class SearchIP : BaseSearch
    {
        protected override string[] url
        {
            get { return Enumerable.Range(1, 100).Select(p => "https://www.kuaidaili.com/free/inha/" + p + "/").ToArray(); }
        }

        public SearchIP()
        {
            var res = new ListTable();


            foreach (string s in url)
            {
                Thread.Sleep(1000);
                string path = Path.GetTempFileName();
                path = HttpDownloadFile(s, path);
                HtmlDocument doc = new HtmlDocument();
                string content = File.ReadAllText(path);
                doc.LoadHtml(content);

                HtmlNodeCollection headList = doc.DocumentNode.SelectNodes("//*[@id=\"list\"]/table/thead/tr/th");
                HtmlNodeCollection valueList = doc.DocumentNode.SelectNodes("//*[@id=\"list\"]/table/tbody/tr");

                if (res.Columns.Count==0)
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
            }

            //var childs = doc.DocumentNode.Descendants().Where(p => !p.HasChildNodes).Where(p => p.XPath.Contains("tbody")).ToDictionary(p => p.XPath);
            //string xPath = "//*[@id=\"list\"]/table/tbody";
            //HtmlNode oldNode = null;
            //childs.TryGetValue(xPath, out oldNode);

            File.WriteAllText("temp.txt", JsonHelper.ToJson((JsonData) res, indentLevel: 2));
        }
    }
}