using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Library.Helper;
using LitJson;

namespace Library.Excel
{
    /// <summary>
    /// DataTable与List
    /// </summary>
    public abstract partial class ExcelByBase
    {
        public class List
        {
            #region  Convert ListTable and Json

            public static JsonData ConvertToJson(ListTable list)
            {
                Ldebug.Log(" is now sheet: " + list.TableName);
                return JsonHelper.ConvertListTableToJson(list);
            }

            public static void ExportToJson(ListTable list, string file, bool isIndent = true)
            {
                JsonData resJsonDatas = ConvertToJson(list);
                string newPath = Path.ChangeExtension(string.IsNullOrEmpty(file) ? list.FullName : file, ".json");
                FileHelper.CreateDirectory(newPath);
                File.WriteAllText(newPath,
                    isIndent ? JsonHelper.ToJson(resJsonDatas, indentLevel: 2) : JsonHelper.ToJson(resJsonDatas),
                    new UTF8Encoding(false));
            }

            #endregion

            #region   Convert ListTable and DataTable

            /// <summary>
            /// 列集合转换为DataTable
            /// </summary>
            /// <param name="vals"></param>
            /// <param name="dtName"></param>
            /// <returns></returns>
            public static DataTable ConvertRowsToDataTable(List<List<object>> vals, string dtName = "")
            {
                var dt = new DataTable()
                {
                    TableName = string.IsNullOrEmpty(dtName) ? "Sheet1" : dtName,
                };

                var header = vals.Select(p => p.First()).ToList();
                foreach (object o in header)
                    dt.Columns.Add(o.ToString(), typeof(string));

                for (int i = 1; i < vals.First().Count; i++)
                {
                    var i1 = i;
                    var val = vals.Select(p => p[i1]).ToArray();
                    dt.Rows.Add(val);
                }

                return dt;
            }

            #endregion
        }

        public partial class Data
        {
            /// <summary>
            /// 列集合
            /// </summary>
            /// <param name="dt"></param>
            /// <returns></returns>
            public static List<List<object>> ConvertToRowsList(DataTable dt)
            {
                var vals = new List<List<object>>();

                dt.GetHeaderList().ForEach(p =>
                {
                    List<object> val = new List<object> {p};
                    foreach (DataRow dr in dt.Rows)
                    {
                        var obj = (dr[p] is System.DBNull) ? "" : dr[p];
                        val.Add(obj);
                    }
                    vals.Add(val);
                });
                return vals;
            }

            /// <summary>
            /// 列集合
            /// </summary>
            /// <param name="dt"></param>
            /// <returns></returns>
            public static Dictionary<string, List<object>> ConvertToRowsDictionary(DataTable dt)
            {
                var vals = new Dictionary<string, List<object>>();

                dt.GetHeaderList().ForEach(p =>
                {
                    List<object> val = new List<object> {p};
                    foreach (DataRow dr in dt.Rows)
                    {
                        var obj = (dr[p] is System.DBNull) ? "" : dr[p];
                        val.Add(obj);
                    }
                    vals.Add(p, val);
                });
                return vals;
            }
        }
    }
}