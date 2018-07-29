using System.Collections.Generic;
using System.Data;
using System.IO;
using Library.LitJson;
using LitJson;

namespace Library.Excel
{
    /// <summary>
    /// DataTable与Json
    /// </summary>
    public abstract partial class ExcelByBase
    {
        #region  Convert Json and DataTable

        public static DataTable ConvertJsonToDataTable(string content)
        {
            return ConvertListToDataTable(ConvertJsonToList(content));
        }

        public static JsonData ConvertDataTableToJson(DataTable dt)
        {
            return ConvertListToJson(ConvertDataTableToList(dt));
        }

        public static DataTable ImportJsonToDataTable(string path)
        {
            return ConvertListToDataTable(ImportJsonToList(path));
        }

        public static void ExportDataTableToJson(DataTable dt, string file)
        {
            file = string.IsNullOrEmpty(file) ? dt.FullName : file;
            ExportListToJson(ConvertDataTableToList(dt), Path.ChangeExtension(file, ".json"));
        }

        #endregion
    }
}