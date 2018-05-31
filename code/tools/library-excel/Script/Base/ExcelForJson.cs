using System.Collections.Generic;
using System.Data;
using System.IO;
using LitJson;

namespace Library.Excel
{
    /// <summary>
    /// DataTable与Json
    /// </summary>
    public abstract partial class ExcelByBase
    {
        #region  Convert Json and DataTable

        public static DataTable ConvertJsonToDataTableByPath(string path, string dtName = "Sheet1")
        {
            return ConvertListToDataTable(ConvertJsonToListByPath(path), dtName);
        }

        public static DataTable ConvertJsonToDataTable(string content, string dtName = "Sheet1")
        {
            return ConvertListToDataTable(ConvertJsonToList(content), dtName);
        }

        public static JsonData ConvertDataTableToJson(DataTable dt)
        {
            var kv = new KeyValuePair<string, List<List<object>>>(dt.TableName, ConvertDataTableToList(dt));
            return ConvertListToJson(kv);
        }

        public static void ConvertDataTableToJsonByPath(DataTable dt, string file)
        {
            var dtName = string.IsNullOrEmpty(dt.TableName) ? Path.GetFileNameWithoutExtension(file) : dt.TableName;
            var kv = new KeyValuePair<string, List<List<object>>>(dtName, ConvertDataTableToList(dt));
            ConvertListToJsonFile(kv, file);
        }

        #endregion
    }
}