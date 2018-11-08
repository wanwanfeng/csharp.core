using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Library;
using Library.Excel;
using Library.Helper;
using LitJson;

namespace findText.Script
{
    public class ActionForHtml2 : ActionForHtml
    {
        protected override string textName
        {
            get { return "Find_Html_Text"; }
        }

        protected override string exName
        {
            get { return "*.html|*.htm|*.tpl"; }
        }

        public override ListTable GetJsonDataArray(string content)
        {
            return JsonHelper.ConvertJsonToListTable(content);
        }

        //public void GetValue(HtmlNode node, ref Dictionary<string, string> cache)
        //{
        //    if (node.HasChildNodes)
        //    {
        //        foreach (HtmlNode childNode in node.ChildNodes)
        //        {
        //            GetValue(childNode, ref cache);
        //        }
        //    }
        //    else
        //    {
        //        if (string.IsNullOrEmpty(node.InnerText.Trim()))
        //            return;
        //        if (node.InnerText == "\r\n")
        //            return;
        //        MatchCollection mc = regex.Matches(node.InnerText);
        //        if (mc.Count == 0) return;
        //        cache.Add(node.XPath, node.InnerText);
        //    }
        //}

        //protected override void OpenRun()
        //{
        //    Dictionary<string, string> dic = new Dictionary<string, string>();

        //    for (int i = 0; i < all.Count; i++)
        //    {
        //        HtmlDocument doc = new HtmlDocument();
        //        doc.LoadHtml(GetShowAll(i));
        //        HtmlNode rootnode = doc.DocumentNode;
        //        Dictionary<string, string> temp = new Dictionary<string, string>();

        //        for (int j = 0; j < rootnode.ChildNodes.Count; j++)
        //        {
        //            GetValue(rootnode.ChildNodes[j], ref temp);
        //        }
        //        foreach (KeyValuePair<string, string> keyValuePair in temp)
        //        {
        //            dic[all[i] + keyValuePair.Key] = keyValuePair.Value;
        //            Console.WriteLine(keyValuePair.Key + "/" + keyValuePair.Value);
        //            GetJsonValue(keyValuePair.Value, i, keyValuePair.Key, keyValuePair.Value);
        //        }
        //    }
        //}

        protected override void OpenRun()
        {
            for (int i = 0; i < all.Count; i++)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(GetShowAll(i));
                var childs = doc.DocumentNode.Descendants()
                    .Where(p => !p.HasChildNodes)
                    .Where(node => !string.IsNullOrEmpty(node.InnerText.Trim()))
                    .Where(node => node.InnerText != "\r\n")
                    .Where(node => !node.InnerText.TrimStart().StartsWith("<%"))
                    .Where(node => regex.Matches(node.InnerText).Count != 0).ToList();
                foreach (HtmlNode node in childs)
                {
                    GetJsonValue(node.InnerText, i, node.XPath, node.InnerText);
                }
            }
        }

        protected override List<JsonData> SetJsonDataArray(ListTable list, bool isReverse = false)
        {
            List<JsonData> jsonDatas = new List<JsonData>();
            List<string> first = list.Key;
            if (isReverse)
                list.List.Reverse();
            foreach (List<object> objects in list.List)
            {
                JsonData data = new JsonData();
                for (int j = 0; j < first.Count; j++)
                {
                    string val = objects[j].ToString();
                    //val = val.Replace("::", ":").Replace("\\n", "\n");
                    data[first[j].ToString()] = val;
                }
                jsonDatas.Add(data);
            }
            return jsonDatas;
        }

        public override void Revert(string inputPath)
        {
            var tables = ExcelByBase.Data.ImportToListTable(inputPath);
            foreach (var table in tables)
            {
                Dictionary<string, List<JsonData>> cache =
                     SetJsonDataArray(table, true).ToLookup(p => p["文件名"].ToString(), q => q).ToDictionary(p => p.Key, q => q.ToList());

                var i = 0;

                foreach (KeyValuePair<string, List<JsonData>> valuePair in cache)
                {
                    string temp = valuePair.Key;
                    Console.WriteLine("还原中...请稍后" + ((float) (++i)/cache.Count).ToString("p1") + "\t" + temp);

                    string path = (Path.GetDirectoryName(inputPath) + temp).Replace("\\", "/");
                    string content = File.ReadAllText(path);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(content);
                    HtmlNode rootnode = doc.DocumentNode;
                    var childs = rootnode.Descendants().Where(p => !p.HasChildNodes).ToList();

                    bool isSave = false;

                    foreach (JsonData data in valuePair.Value)
                    {
                        string xPath = data["行号"].ToString();
                        string oldStr = data["原文"].ToString();
                        string newStr = data["译文"].ToString();
                        HtmlNode oldNode = childs.FirstOrDefault(p => p.XPath == xPath);
                        if (oldNode == null) continue;
                        HtmlNode newNode = HtmlNode.CreateNode(newStr);
                        if (oldNode.InnerText == oldStr)
                            continue;
                        oldNode.ParentNode.ReplaceChild(newNode, oldNode);
                        isSave = true;
                    }
                    if (isSave)
                        doc.Save(path, new UTF8Encoding(false));
                }
            }
        }
    }
}