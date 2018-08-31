using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Library.Helper;
using Library.LitJson;
using LitJson;

//using DataTable = Library.Excel.DataTable;
namespace Library.Excel
{
    public class DataTable : System.Data.DataTable
    {
        public bool IsArray = true;
        public string FullName = "";

        public DataTable(string table) : base(table)
        {
        }

        public DataTable()
        {
            
        }
    }

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
                return LitJsonHelper.ConvertListTableToJson(list);
            }

            public static void ExportToJson(ListTable list, string file)
            {
                JsonData resJsonDatas = ConvertToJson(list);
                string newPath = Path.ChangeExtension(string.IsNullOrEmpty(file) ? list.FullName : file, ".json");
                FileHelper.CreateDirectory(newPath);
                File.WriteAllText(newPath, LitJsonHelper.ToJson(resJsonDatas, true), new UTF8Encoding(false));
            }

            #endregion

            #region   Convert ListTable and DataTable

            /// <summary>
            /// 行集合转换为DataTable
            /// </summary>
            /// <param name="list"></param>
            /// <returns></returns>
            public static DataTable ConvertToDataTable(ListTable list)
            {
                var dt = new DataTable()
                {
                    TableName = string.IsNullOrEmpty(list.TableName) ? "Sheet1" : list.TableName,
                    FullName = list.FullName,
                    IsArray = list.IsArray,
                };

                foreach (object o in list.Key)
                    dt.Columns.Add(o.ToString(), typeof(string));

                foreach (List<object> objects in list.List)
                    dt.Rows.Add(objects.ToArray());

                return dt;
            }

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
            /// 行集合
            /// </summary>
            /// <param name="dt"></param>
            /// <returns></returns>
            public static ListTable ConvertToListTable(DataTable dt)
            {
                var vals = new List<List<object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    vals.Add(dr.ItemArray.ToList());
                }
                return new ListTable()
                {
                    TableName = dt.TableName,
                    FullName = dt.FullName,
                    IsArray = dt.IsArray,
                    Key = GetHeaderList(dt),
                    List = vals,
                };
            }

            /// <summary>
            /// 列集合
            /// </summary>
            /// <param name="dt"></param>
            /// <returns></returns>
            public static List<List<object>> ConvertToRowsList(DataTable dt)
            {
                var vals = new List<List<object>>();

                GetHeaderList(dt)
                    .ForEach(p =>
                    {
                        List<object> val = new List<object> { p };
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

                GetHeaderList(dt)
                    .ForEach(p =>
                    {
                        List<object> val = new List<object> { p };
                        foreach (DataRow dr in dt.Rows)
                        {
                            var obj = (dr[p] is System.DBNull) ? "" : dr[p];
                            val.Add(obj);
                        }
                        vals.Add(p, val);
                    });
                return vals;
            }

            public static List<string> GetHeaderList(System.Data.DataTable dt)
            {
                var vals = new List<string>();
                foreach (DataColumn dc in dt.Columns)
                {
                    vals.Add(dc.ColumnName);
                }
                return vals;
            }
        }
    }
}