using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Library.Extensions
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// 获取列名称
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<string> GetHeaderList(this System.Data.DataTable dt)
        {
            List<string> list = new List<string>();
            foreach (DataColumn dc in dt.Columns)
                list.Add(dc.ColumnName);
            return list;
        }

        /// <summary>
        /// 获取某行数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static object[] GetLineList(this System.Data.DataTable dt, int line = 1)
        {
            line = Math.Max(0, Math.Min(line, dt.Rows.Count));
            return dt.Rows[line].ItemArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ListTable ToListTable(this DataTable dt)
        {
            var vals = new List<List<object>>();
            foreach (DataRow dr in dt.Rows)
            {
                vals.Add(dr.ItemArray.Select(p => (p is System.DBNull) ? "" : p).ToList());
            }
            return new ListTable()
            {
                TableName = dt.TableName,
                Columns = dt.GetHeaderList(),
                Rows = vals,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lt"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this ListTable lt)
        {
            var dt = new DataTable()
            {
                TableName = string.IsNullOrEmpty(lt.TableName) ? "Sheet1" : lt.TableName,
            };

            foreach (object o in lt.Columns)
                dt.Columns.Add(o.ToString(), typeof(object));

            foreach (List<object> objects in lt.Rows)
                dt.Rows.Add(objects.ToArray());

            return dt;
        }
    }
}