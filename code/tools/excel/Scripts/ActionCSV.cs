﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using Library.Excel;
using Script;

namespace ActionCSV
{
    public partial class ActionCSV : ActionBase
    {
        public override string selectExtension => ".csv";
        public override Func<string, IEnumerable<DataTable>> import => file => new[] { ExcelUtils.ImportFromCsv(file) };
        public override Action<DataTable, string> export => ExcelUtils.ExportToCsv;
    }

    public class ToXml : ActionCSV { public ToXml() => ToXml(); }

    public class ToJson : ActionCSV { public ToJson() => ToJson(); }

    public class ToExcel : ActionCSV { public ToExcel() => ToExcel(); }

    public class ToOneExcel : ActionCSV { public ToOneExcel() => ToOneExcel(); }

    /// <summary>
    /// 导出为键值对
    /// </summary>
    public class ToKvExcel : ActionCSV { public ToKvExcel(object obj) : base() => ToKvExcelAll(); }

    /// <summary>
    /// 还原键值对
    /// </summary>
    public class KvExcelTo : ActionCSV { public KvExcelTo(object obj) : base() => KvExcelTo(); }

    public partial class ActionCSV
    {
        public static void SaveCSV(DataTable dt, string fullPath) //table数据写入csv
        {
            FileInfo fi = new FileInfo(fullPath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "";

            for (int i = 0; i < dt.Columns.Count; i++) //写入列名
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            for (int i = 0; i < dt.Rows.Count; i++) //写入各行数据
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string str = dt.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\""); //替换英文冒号 英文冒号需要换成两个冒号

                    if (str.Contains(',') || str.Contains('\"') || str.Contains('\r') || str.Contains('\n'))
                    //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }

        public static DataTable OpenCSVOledb(string filePath) //从csv读取数据返回table  
        {
            using (
                OleDbConnection conn =
                    new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:csv;Extended Properties=Text;")
                )
            {
                DataTable dtTable = new DataTable();

                var name = Path.GetFileName(filePath);
                OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}]", name), conn);
                try
                {
                    adapter.Fill(dtTable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    dtTable = new DataTable();
                }
                return dtTable;
            }
        }

        public static DataTable OpenCSV(string filePath) //从csv读取数据返回table  
        {
            System.Text.Encoding encoding = GetType(filePath); //Encoding.ASCII;//  
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            StreamReader sr = new StreamReader(fs, encoding);

            //记录每次读取的一行记录  
            string strLine = "";
            //记录每行记录中的各字段内容  
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数  
            int columnCount = 0;
            //标示是否是读取的第一行  
            bool IsFirst = true;
            //逐行读取CSV中的数据  
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列  
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();
            return dt;
        }

        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型  
        /// <param name="FILE_NAME">文件路径</param>  
        /// <returns>文件的编码类型</returns>  

        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open,
                FileAccess.Read);
            System.Text.Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// 通过给定的文件流，判断文件的编码类型  
        /// <param name="fs">文件流</param>  
        /// <returns>文件的编码类型</returns>  
        public static System.Text.Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM  
            System.Text.Encoding reVal = System.Text.Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = System.Text.Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = System.Text.Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = System.Text.Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式  
        /// <param name="data"></param>  
        /// <returns></returns>  
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数  
            byte curByte; //当前分析的字节.  
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前  
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　  
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1  
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
    }
}