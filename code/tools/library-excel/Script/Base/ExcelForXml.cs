using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using Library.Helper;

namespace Library.Excel
{
    /// <summary>
    /// DataTable与Xml
    /// </summary>
    public abstract partial class ExcelByBase
    {
        #region  Convert Xml and DataTable

        public static DataTable ConvertXmlToDataTable(string path, string dtName = "")
        {
            path = Path.ChangeExtension(path, ".xml");
            if (!File.Exists(path))
                Ldebug.Log("文件不存在!");
            if (path == null) return null;
            string content = File.ReadAllText(path);

            StringReader dsr = new StringReader(content);
            XmlTextReader xr = new XmlTextReader(dsr);

            var dt = new DataTable();
            //dt.TableName = string.IsNullOrEmpty(dtName) ? "Sheet1" : dtName;
            dt.ReadXml(xr);
            dsr.Close();
            xr.Close();
            return dt;
        }

        public static void ConvertDataTableToXml(DataTable dt, string file = null)
        {
            StringWriter dsw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(dsw);
            dt.WriteXml(xw, XmlWriteMode.WriteSchema);
            string newPath = Path.ChangeExtension(string.IsNullOrEmpty(file) ? dt.TableName : file, ".xml");
            FileHelper.CreateDirectory(newPath);
            File.WriteAllText(newPath, dsw.ToString(), new UTF8Encoding(false));
            xw.Close();
            dsw.Close();
        }

        #endregion
    }
}