using System.IO;
using System.Text;
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
            public static DataTable ImportToDataTable(string path)
            {
                return (DataTable)JsonHelper.ImportJsonToListTable(path);
            }
        }

        public partial class Data
        {
            public static void ExportToJson(DataTable list, string file, bool isIndent = true)
            {
                string newPath = CheckExport(list, file, ".json");
                JsonData resJsonDatas = (JsonData)list;
                File.WriteAllText(newPath,
                    isIndent ? JsonHelper.ToJson(resJsonDatas, indentLevel: 2) : JsonHelper.ToJson(resJsonDatas),
                    new UTF8Encoding(false));
            }
        }

        #endregion
    }
}