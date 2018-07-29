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

        public static DataTable ImportXmlToDataTable(string path)
        {
            path = Path.ChangeExtension(path, ".xml");
            if (!File.Exists(path))
                Ldebug.Log("文件不存在!");
            if (path == null) return null;
            string content = File.ReadAllText(path);

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

        public static void ExportDataTableToXml(DataTable dt, string file)
        {
            file = string.IsNullOrEmpty(file) ? dt.FullName : file;

            StringWriter dsw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(dsw);
            dt.WriteXml(xw, XmlWriteMode.WriteSchema);
            string newPath = Path.ChangeExtension(file, ".xml");
            FileHelper.CreateDirectory(newPath);
            File.WriteAllText(newPath, dsw.ToString(), new UTF8Encoding(false));
            xw.Close();
            dsw.Close();
        }

        #endregion
    }
}