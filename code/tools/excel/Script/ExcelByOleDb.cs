using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace excel.Script
{
    public class ExcelByOleDb : ExcelByBase
    {
        public override Dictionary<string, List<List<object>>> ReadFromExcels(string filename)
        {
            Dictionary<string, DataTable> dt = ExcelToTable(filename);
            return dt.ToDictionary(pair => pair.Key, pair => ConvertToList(pair.Value));
        }

        public override void WriteToExcel(string filename, List<List<object>> vals)
        {
            var dt = ConvertToDataTable(vals);
            TableToExcel(filename, dt);
        }

        public override void WriteToOneExcel(string fileName, Dictionary<string, List<List<object>>> dic)
        {
            List<DataTable> dts = new List<DataTable>();
            foreach (KeyValuePair<string, List<List<object>>> pair in dic)
            {
                var dt = ConvertToDataTable(pair.Value);
                dt.TableName = Path.GetFileNameWithoutExtension(pair.Key);
                dts.Add(dt);
            }
            TableToExcel(fileName, dts.ToArray());
        }

        /// <summary>
        /// 获取dataset
        /// http://www.jb51.net/article/52681.htm
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Dictionary<string, System.Data.DataTable> ExcelToTable(string path)
        {
            var connectionString = ConnectionString(path, "HDR=NO;IMEX=1");
            if (string.IsNullOrEmpty(connectionString)) return null;

            var connection = new System.Data.OleDb.OleDbConnection(connectionString);
            Console.WriteLine(path);
            connection.Open();
            var tables = connection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] {});
            Dictionary<string, DataTable> dts = new Dictionary<string, DataTable>();

            if (tables != null)
                for (int i = 0; i < tables.Rows.Count; i++)
                {
                    var firstTableName = tables.Rows[i]["TABLE_NAME"].ToString();
                    System.Data.OleDb.OleDbCommand sql =
                        new System.Data.OleDb.OleDbCommand("select * from [" + firstTableName + "] ", connection);
                    System.Data.OleDb.OleDbDataAdapter adapter = new System.Data.OleDb.OleDbDataAdapter(sql);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    dts[firstTableName] = dt;
                }

            connection.Close();
            return dts;

            //var sql = "SELECT * FROM  [Sheet1$]";
            //var adapter = new System.Data.OleDb.OleDbDataAdapter(sql, connection);
            //DataSet dataSet = new DataSet();
            //adapter.Fill(dataSet);
            //var table = dataSet.Tables[0]; //返回第一张表
            //connection.Close();
            //return table;
        }

        private static string ConnectionString(string path, string ext = null)
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
                                "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;{1}\"",
                                path, ext); //xls HDR=NO;不忽略第一行 HDR=YES;忽略第一行
                        break;
                    case ".xlsx":
                        //Excel2007的连接字符串  
                        connectionString =
                            string.Format(
                                "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;{1}\"",
                                path, ext); //xlsx HDR=NO;不忽略第一行 HDR=YES;忽略第一行
                        break;
                    default:
                        Console.WriteLine("文件类型错误！！！");
                        break;
                }
            return connectionString;
        }

        private static void TableToExcel(string path, params DataTable[] dts)
        {
            //path = Path.ChangeExtension(path, ".xls");

            if (File.Exists(path))
            {
                Console.WriteLine("文件已存在");
                return;
            }

            var connectionString = ConnectionString(path);
            if (string.IsNullOrEmpty(connectionString)) return;

            //实例化一个Oledbconnection类(实现了IDisposable,要using)
            using (var connection = new System.Data.OleDb.OleDbConnection(connectionString))
            {
                Console.WriteLine(path);
                connection.Open();
                using (var sql = connection.CreateCommand())
                {
                    foreach (DataTable dt in dts)
                    {
                        var tableName = dts.Length <= 1 ? "Sheet1" : dt.TableName;

                        //写入标题
                        var header = new List<string>();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            header.Add(string.Format("[{0}] VarChar", dt.Columns[i].ColumnName));
                        }
                        sql.CommandText = string.Format("CREATE TABLE {0} ({1})", tableName,
                            string.Join(",", header.ToArray()));
                        sql.ExecuteNonQuery();

                        //数据  

                        header.Clear();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            header.Add(dt.Columns[i].ColumnName);
                        }
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var values = new List<string>();
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                values.Add(string.Format("'{0}'", dt.Rows[i][j]));
                            }
                            sql.CommandText = string.Format("insert into {0} ({1}) values ({2})", tableName,
                                string.Join(",", header.ToArray()), string.Join(",", values.ToArray()));
                            sql.ExecuteNonQuery();
                        }
                    }
                    //sql.CommandText = "CREATE TABLE CustomerInfo ([CustomerID] VarChar,[Customer] VarChar)";
                    //sql.ExecuteNonQuery();
                    //sql.CommandText = "insert into CustomerInfo(CustomerID,Customer)values('DJ001','点击科技')";
                    //sql.ExecuteNonQuery();
                }
            }
        }
    }
}