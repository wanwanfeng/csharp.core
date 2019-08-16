using HtmlAgilityPack;
using Library;
using Library.Excel;
using Library.Extensions;
using System.Collections.Generic;
using System.Linq;

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

            urls.ForEachPathsAndSleep(url =>
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
            ExcelUtils.ExportToExcel(res.ToDataTable(), "temp.xlsx");
            //File.WriteAllText("temp.txt", JsonHelper.ToJson((JsonData) res, indentLevel: 2));
        }
    }
}