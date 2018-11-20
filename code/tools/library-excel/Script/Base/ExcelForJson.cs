using Library.Helper;
using LitJson;

namespace Library.Excel
{
    /// <summary>
    /// DataTable与Json
    /// </summary>
    public abstract partial class ExcelByBase
    {
        #region  Convert Json and DataTable

        public class Json
        {
            public static DataTable ConvertToDataTable(string content)
            {
                return JsonHelper.ConvertJsonToListTable(content);
            }

            public static DataTable ImportToDataTable(string path)
            {
                return JsonHelper.ImportJsonToListTable(path);
            }
        }

        public partial class Data
        {
            public static JsonData ConvertToJson(DataTable dt)
            {
                return List.ConvertToJson(dt);
            }

            public static void ExportToJson(DataTable dt, string file, bool isIndent = true)
            {
                string newPath = CheckExport(dt, file, ".json");
                List.ExportToJson(dt, newPath);
            }
        }

        #endregion
    }
}