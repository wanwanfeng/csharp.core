using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Library;
using Library.Excel;
namespace search.Script
{
    public class SearchIP : BaseSearch
    {
        protected override string[] urls
        {
            get
            {
                return Enumerable.Range(1, 100).Select(p => "https://www.kuaidaili.com/free/inha/" + p + "/").ToArray();
            }
        }

        public SearchIP()
        {
            var res = new ListTable();

            urls.ForEachPaths(url =>
            {
                HtmlWeb webClient = new HtmlWeb();
                HtmlDocument doc = webClient.Load(url);

                HtmlNodeCollection headList = doc.DocumentNode.SelectNodes("//*[@id=\"list\"]/table/thead/tr/th");
                HtmlNodeCollection valueList = doc.DocumentNode.SelectNodes("//*[@id=\"list\"]/table/tbody/tr");

                if (res.Columns.Count == 0)
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
            });

            ExcelByBase.Data.ExportToExcel(res, "temp.xlsx");

            //File.WriteAllText("temp.txt", JsonHelper.ToJson((JsonData) res, indentLevel: 2));
        }
    }
}