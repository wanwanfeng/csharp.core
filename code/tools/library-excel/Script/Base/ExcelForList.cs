using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Library.Excel
{
    /// <summary>
    /// DataTable与List
    /// </summary>
    public abstract partial class ExcelByBase
    {
        #region  Convert List<List<object>> and DataTable

        /// <summary>
        /// 行集合转换为DataTable
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="dtName"></param>
        /// <returns></returns>
        public static DataTable ConvertListToDataTable(List<List<object>> vals, string dtName = "")
        {
            var dt = new DataTable(string.IsNullOrEmpty(dtName) ? "Sheet1" : dtName);

            foreach (object o in vals.First())
                dt.Columns.Add(o.ToString(), typeof (string));

            foreach (List<object> objects in vals.Skip(1))
                dt.Rows.Add(objects.ToArray());

            return dt;
        }

        /// <summary>
        /// 行集合
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<List<object>> ConvertDataTableToList(DataTable dt)
        {
            var vals = new List<List<object>>();

            foreach (DataRow dr in dt.Rows)
            {
                vals.Add(dr.ItemArray.ToList());
            }
            return vals;
        }

        /// <summary>
        /// 列集合转换为DataTable
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="dtName"></param>
        /// <returns></returns>
        public static DataTable ConvertRowsListToDataTable(List<List<object>> vals, string dtName = "")
        {
            var dt = new DataTable(string.IsNullOrEmpty(dtName) ? "Sheet1" : dtName);

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

        public static List<string> GetHeaderList(DataTable dt)
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