using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using Library;
using Library.Helper;
using LitJson;

namespace search.Script
{
    public class SearchMovie : BaseSearch
    {
        protected override string[] url
        {
            get { return new[] {"http://www.7dds.com/?m=vod-type-id-2.html"}; }
        }

        public override void Run(HtmlDocument doc)
        {
            HtmlNodeCollection valueList = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div/div/div[2]/ul/li");

            var res = new ListTable();

            foreach (HtmlNode node in valueList)
            {
                var temp = new List<object>();
                foreach (HtmlNode child in node.SelectNodes("div/div/h4/a"))
                {
                    temp.Add(child.InnerText);
                }
                res.Rows.Add(temp);
            }

            //var childs = doc.DocumentNode.Descendants().Where(p => !p.HasChildNodes).Where(p => p.XPath.Contains("tbody")).ToDictionary(p => p.XPath);
            //string xPath = "//*[@id=\"list\"]/table/tbody";
            //HtmlNode oldNode = null;
            //childs.TryGetValue(xPath, out oldNode);

            File.WriteAllText("temp.txt", JsonHelper.ToJson(res.Rows, indentLevel: 2));
        }
    }
}