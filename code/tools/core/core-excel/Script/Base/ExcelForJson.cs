using System;
using System.Data;
using System.IO;
using System.Text;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace Library.Excel
{
    /// <summary>
    /// DataTable与Json
    /// </summary>
    public abstract partial class ExcelUtils
    {
        #region  Convert Json and DataTable

        public static DataTable ImportFromJson(string path)
        {
            return JsonHelper.ImportJsonToListTable(path).ToDataTable();
        }

        public static void ExportToJson(DataTable dt, string file)
        {
            string newPath = CheckExport(dt, file, ".json");
            JsonData resJsonDatas = (JsonData)dt.ToListTable();
            File.WriteAllText(newPath, ExportToJson(resJsonDatas), new UTF8Encoding(false));
        }

        public static string ExportToJson(object obj)
        {
            int.TryParse(Environment.GetEnvironmentVariable("JsonIsIndent"), out int JsonIsIndent);
            var isIndent = JsonIsIndent == 1;
            return isIndent ? JsonHelper.ToJson(obj, indentLevel: 2) : JsonHelper.ToJson(obj);
        }

        #endregion
    }
}