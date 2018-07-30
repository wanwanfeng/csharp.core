using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using Library.Helper;
using Library.LitJson;
using LitJson;

namespace Library.Excel
{
    /// <summary>
    /// DataTable与Xml
    /// </summary>
    public abstract partial class ExcelByBase
    {
        #region  Convert Xml and DataTable

        public enum XmlMode
        {
            Excel,
            Navicat,
        }

        public static XmlMode CurXmlMode = XmlMode.Navicat;

        public class Xml
        {
            public static DataTable ImportToDataTable(string path)
            {
                path = Path.ChangeExtension(path, ".xml");
                if (!File.Exists(path))
                    Ldebug.Log("文件不存在!");
                if (path == null) return null;
                string content = File.ReadAllText(path);

                switch (CurXmlMode)
                {
                    case XmlMode.Excel:
                    {
                        StringReader dsr = new StringReader(content);
                        XmlTextReader xr = new XmlTextReader(dsr);

                        var dt = new DataTable
                        {
                            FullName = path,
                            TableName = Path.GetFileNameWithoutExtension(path)
                        };

                        dt.ReadXml(xr);
                        dsr.Close();
                        xr.Close();
                        return dt;
                    }
                    case XmlMode.Navicat:
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(content);
                        var nodes = doc.SelectNodes("RECORDS/RECORD");

                        JsonData jsonData = new JsonData();
                        jsonData.SetJsonType(JsonType.Array);
                        if (nodes != null)
                        {
                            foreach (XmlNode xmlNode in nodes)
                            {
                                JsonData data = new JsonData();
                                foreach (XmlElement element in xmlNode)
                                {
                                    data[element.Name] = element.InnerText;
                                }
                                jsonData.Add(data);
                            }
                        }
                        return Json.ConvertToDataTable(LitJsonHelper.ToJson(jsonData));
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class Data
        {
            public static void ExportToXml(DataTable dt, string file)
            {
                file = string.IsNullOrEmpty(file) ? dt.FullName : file;
                string newPath = Path.ChangeExtension(file, ".xml");
                FileHelper.CreateDirectory(newPath);

                switch (CurXmlMode)
                {
                    case XmlMode.Excel:
                    {
                        StringWriter dsw = new StringWriter();
                        XmlTextWriter xw = new XmlTextWriter(dsw);
                        dt.WriteXml(xw, XmlWriteMode.WriteSchema);
                        File.WriteAllText(newPath, dsw.ToString(), new UTF8Encoding(false));
                        xw.Close();
                        dsw.Close();
                    }
                        break;
                    case XmlMode.Navicat:
                    {
                        XmlDocument doc = new XmlDocument();
                        XmlNode docNode = doc.CreateNode(XmlNodeType.Element, "RECORDS", "");
                        doc.AppendChild(docNode);

                        ListTable list = ConvertToListTable(dt);
                        foreach (List<object> objects in list.List)
                        {
                            var node = doc.CreateNode(XmlNodeType.Element, "RECORD", "");
                            docNode.AppendChild(node);
                            Queue<string> keys = new Queue<string>(list.Key);
                            foreach (object o in objects)
                            {
                                var xmlNode = doc.CreateNode(XmlNodeType.Element, keys.Dequeue(), "");
                                xmlNode.InnerText = o.ToString();
                                node.AppendChild(xmlNode);
                            }
                        }
                        doc.Save(newPath);
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

              
            }
        }

        #endregion
    }
}