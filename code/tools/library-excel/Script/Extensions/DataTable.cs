using System.Collections.Generic;
using System.Data;
using System.Linq;

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


        public static implicit operator ListTable(DataTable dt)
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
                dt.Columns.Add(o.ToString(), typeof(string));

            foreach (List<object> objects in lt.Rows)
                dt.Rows.Add(objects.ToArray());

            return dt;
        }
    }
}

public static class DataTableExtensions
{
    public static List<string> GetHeaderList(this System.Data.DataTable dt)
    {
        var vals = new List<string>();
        foreach (DataColumn dc in dt.Columns)
        {
            vals.Add(dc.ColumnName);
        }
        return vals;
    }
}