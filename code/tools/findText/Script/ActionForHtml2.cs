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
            get { return "*.html|*.htm|*.tpl|*.xml"; }
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

        //protected override void OpenRun(string file)
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

        protected override void OpenRun(string file)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(File.ReadAllText(file));
            var childs = doc.DocumentNode.Descendants()
                .Where(p => !p.HasChildNodes)
                .Where(node => !string.IsNullOrEmpty(node.InnerText.Trim()))
                .Where(node => node.InnerText != "\r\n")
                .Where(node => !node.InnerText.TrimStart().StartsWith("<%"))
                .Where(node => regex.Matches(node.InnerText).Count != 0).ToList();
            foreach (HtmlNode node in childs)
            {
                GetJsonValue(node.InnerText, file, node.XPath, node.InnerText);
            }
        }

        public override void Revert()
        {
            var caches = GetFileCaches();
            if (caches.Count == 0) return;

            foreach (var dic in caches)
            {
                var i = 0;
                foreach (KeyValuePair<string, List<List<object>>> valuePair in dic)
                {
                    string temp = valuePair.Key;
                    Console.WriteLine("还原中...请稍后" + ((float)(++i) / dic.Count).ToString("p1") + "\t" + temp);

                    string path = (Path.GetDirectoryName(InputPath) + temp).Replace("\\", "/");
                    string content = File.ReadAllText(path);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(content);
                    HtmlNode rootnode = doc.DocumentNode;
                    var childs = rootnode.Descendants().Where(p => !p.HasChildNodes).ToList();

                    bool isSave = false;

                    foreach (List<object> data in valuePair.Value)
                    {
                        string xPath = data[Convert["行号"]].ToString();
                        string oldStr = data[Convert["原文"]].ToString();
                        string newStr = data[Convert["译文"]].ToString();
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