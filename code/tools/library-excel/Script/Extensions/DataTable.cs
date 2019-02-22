using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Library.Helper;
using LitJson;

namespace Library.Excel
{
    public class DataTable : System.Data.DataTable
    {
        public bool IsArray = true;
        public string FullName = "";

        public DataTable(string table)
            : base(table)
        {
        }

        public DataTable()
        {

        }

        #region ListTable DataTable 隐式转换


        public static implicit operator ListTable(DataTable dt)
        {
            var vals = new List<List<object>>();
            foreach (DataRow dr in dt.Rows)
            {
                vals.Add(dr.ItemArray.Select(p => (p is System.DBNull) ? "" : p).ToList());
            }
            return new ListTable()
            {
                TableName = dt.TableName,
                FullName = dt.FullName,
                IsArray = dt.IsArray,
                Columns = dt.GetHeaderList(),
                Rows = vals,
            };
        }

        /// <summary>
        /// 行集合转换为DataTable
        /// </summary>
        /// <param name="lt"></param>
        /// <returns></returns>
        public static implicit operator DataTable(ListTable lt)
        {
            var dt = new DataTable()
            {
                TableName = string.IsNullOrEmpty(lt.TableName) ? "Sheet1" : lt.TableName,
                FullName = lt.FullName,
                IsArray = lt.IsArray,
            };

            foreach (object o in lt.Columns)
                dt.Columns.Add(o.ToString(), typeof (object));

            foreach (List<object> objects in lt.Rows)
                dt.Rows.Add(objects.ToArray());

            return dt;
        }

        #endregion

        #region JsonData DataTable 显示转换（需强制）


        public static explicit operator JsonData(DataTable dt)
        {
            return JsonHelper.ConvertListTableToJson((ListTable)dt);
        }

        public static explicit operator DataTable(JsonData jsonData)
        {
            return (DataTable) JsonHelper.ConvertJsonToListTable(JsonHelper.ToJson(jsonData));
        }

        #endregion
    }
}

public static class DataTableExtensions
{
    public static List<string> GetHeaderList(this System.Data.DataTable dt)
    {
        return (from DataColumn dc in dt.Columns select dc.ColumnName).ToList();
    }
}