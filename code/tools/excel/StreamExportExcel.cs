using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace excel
{
    public class StreamExportExcel
    {
        /// <summary>
        /// https://www.cnblogs.com/Sandon/p/5175829.html
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="vals"></param>
        public static void WriteToExcel(string filename, List<List<object>> vals)
        {
            DataTable dt = new DataTable(Path.GetFileNameWithoutExtension(filename));
            foreach (List<object> objects in vals)
            {
                foreach (object o in objects)
                {
                    DataColumn dc = new DataColumn();
                    dt.Columns.Add(dc);
                }
                dt.Rows.Add(objects.ToArray());
            }
            ExportExcelByFileStream(filename, dt);
        }


        public static void ExportExcelByFileStream(string filename, DataTable dt)
        {
            //设置导出文件路径
            string path = Environment.CurrentDirectory + "/Export";

            //设置新建文件路径及名称
            string savePath = path + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";

            //创建文件
            FileStream file = new FileStream(savePath, FileMode.CreateNew, FileAccess.Write);

            //以指定的字符编码向指定的流写入字符
            StreamWriter sw = new StreamWriter(file, Encoding.GetEncoding("GB2312"));

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
            file.Flush();

            sw.Close();
            sw.Dispose();

            file.Close();
            file.Dispose();
        }

        public static byte[] ExportExcelByMemoryStream(DataTable dt)
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