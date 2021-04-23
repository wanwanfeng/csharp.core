using Library.Excel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Script
{
    public class ActionXml : ActionBase
    {
        public override string selectExtension
        {
            get { return ".xml"; }
        }

        public override Func<string, IEnumerable<DataTable>> import
        {
            get { return file => new[] { ExcelUtils.ImportFromXml(file)}; }
        }

        public override Action<DataTable, string> export
        {
            get { return ExcelUtils.ExportToXml; }
        }

        public class ToCsv : ActionXml
        {
            public ToCsv()
            {
                ToCsv();
            }
        }

        public class ToJson : ActionXml
        {
            public ToJson()
            {
                ToJson();
            }
        }

        public class ToExcel : ActionXml
        {
            public ToExcel()
            {
                ToExcel();
            }
        }

        public class ToOneExcel : ActionXml
        {
            public ToOneExcel()
            {
                ToOneExcel();
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public class ToKvExcel : ActionXml
        {
			public ToKvExcel(object obj) : base()
			{
				ToKvExcelAll();
			}
		}

        /// <summary>
        /// 还原键值对
        /// </summary>
        public class KvExcelTo : ActionXml
        {
			public KvExcelTo(object obj) : base()
			{
                KvExcelTo(isCustomAction: (fullpath, list) =>
                {
                    //Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    //foreach (List<object> objects in list)
                    //{
                    //    object id = objects[1];
                    //    string key = objects[2].ToString();
                    //    object value = objects[3];
                    //    string value_zh_cn = objects[4].ToString();

                    //    dictionary[id.ToString()] = value_zh_cn;
                    //}

                    //HtmlDocument doc = new HtmlDocument();
                    //string content = File.ReadAllText(fullpath);
                    //doc.LoadHtml(content);

                    //var childs = doc.DocumentNode.Descendants().Where(p => !p.HasChildNodes).ToDictionary(p => p.XPath);

                    //bool isSave = false;

                    //foreach (KeyValuePair<string, object> pair in dictionary)
                    //{
                    //    HtmlNode oldNode = null;
                    //    childs.TryGetValue(pair.Key, out oldNode);
                    //    if (oldNode == null) continue;
                    //    HtmlNode newNode = HtmlNode.CreateNode(newStr);
                    //    if (oldNode.InnerText == oldStr)
                    //        continue;
                    //    oldNode.ParentNode.ReplaceChild(newNode, oldNode);
                    //    isSave = true;
                    //}

                    //if (isSave)
                    //    doc.Save(path, new UTF8Encoding(false));

                    //JsonHelper.RevertDictionaryToJson(jsonData, dictionary);
                    //return isIndent ? JsonHelper.ToJson(jsonData, indentLevel: 2) : JsonHelper.ToJson(jsonData);
                    return "";
                });
            }
        }
    }
}