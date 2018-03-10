using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace excel.Script
{
    public class ExcelByStream : ExcelByBase
    {
        /// <summary>
        /// https://www.cnblogs.com/Sandon/p/5175829.html
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="vals"></param>
        public override void WriteToExcel(string filename, List<List<object>> vals)
        {
            var dt = ConvertToDataTable(vals);
            File.WriteAllBytes(filename, ExportExcelByMemoryStream(dt));
        }

        private static byte[] ExportExcelByMemoryStream(DataTable dt)
        {
            //创建一个内存流
            MemoryStream ms = new MemoryStream();

            //以指定的字符编码向指定的流写入字符
            StreamWriter sw = new StreamWriter(ms, Encoding.GetEncoding("GB2312"));

            StringBuilder strbu = new StringBuilder();

            //写入标题
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                strbu.Append(dt.Columns[i].ColumnName + "\t");
            }
            //加入换行字符串
            strbu.Append(Environment.NewLine);

            //写入内容
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    strbu.Append(dt.Rows[i][j] + "\t");
                }
                strbu.Append(Environment.NewLine);
            }

            sw.Write(strbu.ToString());
            sw.Flush();

            sw.Close();
            sw.Dispose();

            //转换为字节数组
            byte[] bytes = ms.ToArray();

            ms.Close();
            ms.Dispose();

            return bytes;
        }
    }
}