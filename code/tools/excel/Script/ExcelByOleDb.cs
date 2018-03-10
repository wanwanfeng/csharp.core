using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace excel.Script
{
    public class ExcelByOleDb : ExcelByBase
    {
        public override KeyValuePair<string, List<List<object>>> ReadFromExcel(string filename)
        {
            var dt = ExcelToTable(filename);
            return new KeyValuePair<string, List<List<object>>>(filename, ConvertToList(dt));
        }

        public override Dictionary<string, List<List<object>>> ReadFromExcels(string filename)
        {
            Dictionary<string, List<List<object>>> list = new Dictionary<string, List<List<object>>>();
            //var dt = ExcelToTable(filename);
            //foreach (KeyValuePair<string, DataTable> pair in dt)
            //{
            //    list.Add(new Dictionary<string, List<List<object>>>()
            //    {
            //        {filename, ConvertToList(dt.Values.First())}
            //    });
            //}
            return list;
        }

        public override void WriteToExcel(string filename, List<List<object>> vals)
        {
            var dt = ConvertToDataTable(vals);
            //TableToExcel(dt, filename);
        }

        /// <summary>
        /// 获取dataset
        /// http://www.jb51.net/article/52681.htm
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static System.Data.DataTable ExcelToTable(string path)
        {
            string connectionString = "";
            var extension = Path.GetExtension(path);
            if (extension != null)
                switch (extension.ToLower())
                {
                    case ".xls":
                        //Excel2003的连接字符串
                        connectionString =
                            string.Format(
                                "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1\"",
                                path); //xls HDR=NO;不忽略第一行 HDR=YES;忽略第一行
                        break;
                    case ".xlsx":
                        //Excel2007的连接字符串  
                        connectionString =
                            string.Format(
                                "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=NO;IMEX=1\"",
                                path); //xlsx HDR=NO;不忽略第一行 HDR=YES;忽略第一行
                        break;
                    default:
                        Console.WriteLine("文件类型错误！！！");
                        break;
                }
            if (string.IsNullOrEmpty(connectionString)) return null;

            var connection = new System.Data.OleDb.OleDbConnection(connectionString);
            Console.WriteLine(path);
            connection.Open();
            var sql = "SELECT * FROM  [Sheet1$]";
            var adapter = new System.Data.OleDb.OleDbDataAdapter(sql, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            var table = dataSet.Tables[0]; //返回第一张表
            connection.Close();
            return table;
        }
    }
}