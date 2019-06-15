using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Extensions
{
    public static class DataTableExtensions
    {
        ////获取列名称
        //public static List<string> GetHeaderList(this System.Data.DataTable dt)
        //{
        //    List<string> list = new List<string>();
        //    foreach (DataColumn dc in dt.Columns)
        //        list.Add(dc.ColumnName);
        //    return list;
        //}

        ////获取某行数据
        //public static object[] GetLineList(this System.Data.DataTable dt, int line = 1)
        //{
        //    line = Math.Max(0, Math.Min(line, dt.Rows.Count));
        //    return dt.Rows[line].ItemArray;
        //}
    }
}