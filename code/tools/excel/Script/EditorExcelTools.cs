using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using Library.Helper;

namespace Excel
{
    public enum ExportType
    {
        [Description(".xml")] ToXml,
        [Description(".xml")] ToXmlAttribute,
        [Description(".json")] ToJson,
        [Description(".txt")] ToTxt,
        [Description(".txt")] ToCsv,
        [Description(".byte")] ToBytes,
        [Description(".cs")] ToCs,
        [Description(".php")] ToPhp,
    }

    /// <summary>
    /// 每个格子的信息
    /// </summary>
    public struct Cell
    {
        public string remark; //每格备注
        public string name; //每格名
        public string type; //每格类型名
        public string value; //每格值
    }

    public class EditorExcelTools
    {
        public  static IDictionary<ExportType, string> CacheDictionary;

        static EditorExcelTools()
        {
            CacheDictionary = AttributeHelper.GetCacheDescription<ExportType>();
        }

        #region excel 导出核心

        private static Dictionary<int, List<Cell>> ExportTable(string path, int sheet)
        {
            return MS_GetDataRow(path, sheet);
        }

        /// <summary>
        /// 数据组合成字典
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rowCollection"></param>
        /// <returns></returns>
        public static Dictionary<int, List<Cell>> GetDictionary(string path, DataRowCollection rowCollection)
        {
            int curHang = 0;
            var remarks = rowCollection[curHang].ItemArray.ToList();

            curHang++;

            var names = rowCollection[curHang].ItemArray.ToList();
            for (var i = 0; i < names.Count; i++)
            {
                if (names[i] != null && !string.IsNullOrEmpty(names[i].ToString())) continue;
                Console.WriteLine(Path.GetFileName(path) +
                               string.Format(":变量名称有空余项！\n错误位置在坐标(行：{0}、列：{1})处！共应该有{2}个变量名称！", 0 + 2, i + 1,
                                   names.Count));
                return null;
            }

            curHang++;

            var types = rowCollection[curHang].ItemArray.ToList();
            for (var i = 0; i < types.Count; i++)
            {
                if (types[i] != null && !string.IsNullOrEmpty(types[i].ToString())) continue;
                Console.WriteLine(Path.GetFileName(path) +
                               string.Format(":变量类型有空余项！\n错误位置在坐标(行：{0}、列：{1})处！共应该有{2}个变量名称！", 1 + 2, i + 1,
                                   types.Count));
                return null;
            }

            curHang++;

            var dic = new Dictionary<int, List<Cell>>();
            for (var j = curHang; j < rowCollection.Count; j++)
            {
                var rows = rowCollection[j].ItemArray.ToList();
                if (string.IsNullOrEmpty(rows[0].ToString()))
                {
                    continue; //首格为空时此行跳过
                }
                var rowresult = new List<Cell>();
                for (var i = 0; i < rows.Count; i++)
                {
                    if (rows[i] == null) continue;
                    if (types[i].ToString().StartsWith("#") || names[i].ToString().StartsWith("#"))
                    {
                        //备注列（不属于有效数据）
                        continue;
                    }
                    rowresult.Add(new Cell
                    {
                        remark = remarks[i].ToString(),
                        name = names[i].ToString(),
                        type = types[i].ToString(),
                        value = rows[i].ToString()
                    });
                }
                dic[j - curHang] = rowresult;
            }
            return dic;
        }


        #region 微软 Excel

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sheet"></param>
        public static Dictionary<int, List<Cell>> MS_GetDataRow(string path, int sheet)
        {
            try
            {
                DataRowCollection rowCollection = null;
                MS_GetConnection(path, connection =>
                {
                    connection.Open();
                    var sql = "SELECT * FROM  [Sheet1$]";
                    var adapter = new System.Data.OleDb.OleDbDataAdapter(sql, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    DataTable table = dataSet.Tables[sheet]; //返回第一张表
                    rowCollection = table.Rows; //返回一个行的集合
                });

                //遍历行的集合，取得每一行的DataRow对象
                //默认表中第一行为表头，输出或不输出看参数
                //人为规定第二行为变量名称
                //人为规定第三行为变量类型
                //共有多少列由第二行第三行列数来决定
                return GetDictionary(path, rowCollection);
            }
            catch (Exception err)
            {
                Console.WriteLine("数据绑定Excel失败!失败原因：" + err.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取dataset
        /// http://www.jb51.net/article/52681.htm
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callAction"></param>
        /// <param name="isCreate"></param>
        /// <returns></returns>
        private static void MS_GetConnection(string path, Action<System.Data.OleDb.OleDbConnection> callAction,
            bool isCreate = false)
        {
            if (!isCreate && !Directory.Exists(path))
            {
                Console.WriteLine(path + " is not exist !");
            }

            string connectionString = "";
            switch (Path.GetExtension(path))
            {
                case ".xls":
                    //Excel2003的连接字符串
                    connectionString =
                        string.Format(
                            "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;",
                            path); //xls HDR=NO;不忽略第一行 HDR=YES;忽略第一行
                    break;
                case ".xlsx":
                    //Excel2007的连接字符串  
                    connectionString =
                        string.Format(
                            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;",
                            path); //xlsx HDR=NO;不忽略第一行 HDR=YES;忽略第一行
                    break;
                default:
                    Console.WriteLine("文件类型错误！！！");
                    return;
            }
            //if (!isCreate)
            {
                connectionString += "HDR=NO;IMEX=1\"";
            }
            var connection = new System.Data.OleDb.OleDbConnection(connectionString);
            Console.WriteLine(path);
            callAction.Invoke(connection);
            connection.Close();
        }

        #endregion

        #endregion

        #region 生成excel

        public static void MS_ExportExcel(string path, List<string> keys, List<List<object>> content)
        {
            MS_GetConnection(path, connection =>
            {
                File.Create(path);
                string sqlCreate = "CREATE TABLE TestSheet ({0})";
                string key = string.Join(",", keys.Select(p => string.Format("[{0}] VarChar", p)).ToArray());
                var cmd = new System.Data.OleDb.OleDbCommand(string.Format(sqlCreate, key), connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                foreach (List<object> objects in content)
                {
                    cmd.CommandText = string.Format("INSERT INTO TestSheet VALUES({0})",
                        string.Join(",", objects.Select(p => p.ToString()).ToArray()));
                    cmd.ExecuteNonQuery();
                }
            }, true);
        }

        public static void MS_ExportExcel(string path, Dictionary<int, List<Cell>> content)
        {
            MS_GetConnection(path, connection =>
            {
                string sqlCreate = "CREATE TABLE TestSheet ([ID] INTEGER,[Username] VarChar,[UserPwd] VarChar)";
                var cmd = new System.Data.OleDb.OleDbCommand(sqlCreate, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                foreach (KeyValuePair<int, List<Cell>> pair in content)
                {
                    cmd.CommandText = string.Format("INSERT INTO TestSheet VALUES({0})",
                        string.Join(",", pair.Value.Select(p => p.value).ToArray()));
                    cmd.ExecuteNonQuery();
                }
            });
        }

        /// <summary>
        /// 键值对
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public static void MS_ExportExcel(string path, Dictionary<object, List<object>> content)
        {
            MS_GetConnection(path, connection =>
            {
                string sqlCreate = "CREATE TABLE TestSheet ([ID] INTEGER,[Username] VarChar,[UserPwd] VarChar)";
                var cmd = new System.Data.OleDb.OleDbCommand(sqlCreate, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                foreach (var pair in content)
                {
                    cmd.CommandText = string.Format("INSERT INTO TestSheet VALUES({0})",
                        string.Join(",", pair.Value.Select(p => p.ToString()).ToArray()));
                    cmd.ExecuteNonQuery();
                }
            });
        }

        /// <summary>
        /// 键值对
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public static void MS_ExportExcel(string path, List<object> content)
        {
            if (content == null || content.Count == 0)
                return;

            MS_GetConnection(path, connection =>
            {
                {
                    var pair = content.First();
                    if (pair.GetType().IsClass)
                    {
                        var pair1 = pair;
                        var names = pair.GetType().GetFields().Select(p => p.Name.ToString()).ToArray();
                        var fieldTypes = pair.GetType().GetFields().Select(p => p.FieldType.ToString()).ToArray();
                        Console.WriteLine(string.Join(",", names));
                        Console.WriteLine(string.Join(",", fieldTypes));
                    }
                }

                string sqlCreate = "CREATE TABLE TestSheet ([ID] INTEGER,[Username] VarChar,[UserPwd] VarChar)";
                var cmd = new System.Data.OleDb.OleDbCommand(sqlCreate, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                foreach (var pair in content)
                {
                    if (pair.GetType().IsClass)
                    {
                        var pair1 = pair;
                        var haha = pair.GetType().GetFields().Select(p => p.GetValue(pair1).ToString()).ToArray();
                        Console.WriteLine(string.Join(",", haha));
                        cmd.CommandText = string.Format("INSERT INTO TestSheet VALUES({0})",string.Join(",", haha));
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        #endregion
    }
}