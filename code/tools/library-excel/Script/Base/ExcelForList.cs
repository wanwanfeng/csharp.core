using System.Collections.Generic;
using System.Data;
using System.Linq;
using Library.LitJson;

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
        #region  Convert List<List<object>> and DataTable

        /// <summary>
        /// 行集合转换为DataTable
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ConvertListToDataTable(ListTable list)
        {
            var dt = new DataTable()
            {
                TableName = string.IsNullOrEmpty(list.TableName) ? "Sheet1" : list.TableName,
                FullName = list.FullName,
                IsArray = list.IsArray,
            };

            foreach (object o in list.Key)
                dt.Columns.Add(o.ToString(), typeof (string));

            foreach (List<object> objects in list.List)
                dt.Rows.Add(objects.ToArray());

            return dt;
        }

        /// <summary>
        /// 行集合
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ListTable ConvertDataTableToList(DataTable dt)
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
                Key = GetHeaderList(dt).Cast<object>().ToList(),
                List = vals,
            };
        }

        /// <summary>
        /// 列集合转换为DataTable
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="dtName"></param>
        /// <returns></returns>
        public static DataTable ConvertRowsListToDataTable(List<List<object>> vals, string dtName = "")
        {
            var dt = new DataTable()
            {
                TableName = string.IsNullOrEmpty(dtName) ? "Sheet1" : dtName,
            };

            var header = vals.Select(p => p.First()).ToList();
            foreach (object o in header)
                dt.Columns.Add(o.ToString(), typeof (string));

            for (int i = 1; i < vals.First().Count; i++)
            {
                var i1 = i;
                var val = vals.Select(p => p[i1]).ToArray();
                dt.Rows.Add(val);
            }

            return dt;
        }

        /// <summary>
        /// 列集合
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<List<object>> ConvertDataTableToRowsList(DataTable dt)
        {
            var vals = new List<List<object>>();

            GetHeaderList(dt).ForEach(p =>
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
        public static Dictionary<string, List<object>> ConvertDataTableToRowsDic(DataTable dt)
        {
            var vals = new Dictionary<string, List<object>>();

            GetHeaderList(dt).ForEach(p =>
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

        public static List<string> GetHeaderList(System.Data.DataTable dt)
        {
            var vals = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                vals.Add(dc.ColumnName);
            }
            return vals;
        }

        #endregion
    }
}