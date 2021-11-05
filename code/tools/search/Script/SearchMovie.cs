using HtmlAgilityPack;
using Library;
using Library.Extensions;
using Library.Helper;
using LitJson.P;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace search.Script
{
    public class SearchMovie : BaseSearch
    {
        protected override string[] urls
        {
            get
            {
                return
                    Enumerable.Range(1, 100)
                        .Select(p => p == 1
                            ? "http://www.7dds.com/?m=vod-type-id-2.html"
                            : "http://www.7dds.com/?m=vod-type-id-2-pg-" + p + ".html")
                        .ToArray();
            }
        }

        public SearchMovie()
        {
            var res = new ListTable();

            urls.ForEachPathsAndSleep(url =>
            {
                HtmlWeb webClient = new HtmlWeb();
                HtmlDocument doc = webClient.Load(url);
                //HtmlNodeCollection headList = doc.DocumentNode.SelectNodes("//*[@id=\"list\"]/table/thead/tr/th");
                HtmlNodeCollection valueList = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div/div/div[2]/ul/li");

                //if (res.Columns.Count == 0)
                //    foreach (HtmlNode node in headList)
                //    {
                //        res.Columns.Add(node.InnerText);
                //    }

                foreach (HtmlNode node in valueList)
                {
                    var temp = new List<object>();
                    foreach (HtmlNode child in node.SelectNodes("div/div/h4/a"))
                    {
                        temp.Add(child.InnerText);
                    }
                    res.Rows.Add(temp);
                }
            });

            File.WriteAllText("temp.txt", JsonHelper.ToJson((JsonData)res, indentLevel: 2));
        }
    }
    public class SearchIndexM3U8 : BaseSearch
    {
        protected override string[] urls
        {
            get { return new[] { "https://m.1024e.cc/?inviteCode=VNWRQM&channelCode=VNWRQM" }; }
        }

        public SearchIndexM3U8()
        {
            var res = new ListTable();

            urls.ForEachPathsAndSleep(url =>
            {
                HtmlWeb webClient = new HtmlWeb();
                HtmlDocument doc = webClient.Load(url);
                HtmlNodeCollection headList = doc.DocumentNode.SelectNodes("//*[@id=\"changelike-box\"]/li");
                //HtmlNodeCollection headList = doc.DocumentNode.SelectNodes("//*[@id=\"list\"]/table/thead/tr/th");
                //HtmlNodeCollection valueList = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div/div/div[2]/ul/li");

                if (res.Columns.Count == 0)
                    foreach (HtmlNode node in headList)
                    {
                        var xx = node.SelectNodes("a/@href");
                        Console.WriteLine(node.InnerHtml);
                        res.Columns.Add(node.InnerText);
                    }

                //foreach (HtmlNode node in valueList)
                //{
                //    var temp = new List<object>();
                //    foreach (HtmlNode child in node.SelectNodes("div/div/h4/a"))
                //    {
                //        temp.Add(child.InnerText);
                //    }
                //    res.Rows.Add(temp);
                //}
            });

            File.WriteAllText("temp.txt", JsonHelper.ToJson((JsonData)res, indentLevel: 2));
        }
    }
}