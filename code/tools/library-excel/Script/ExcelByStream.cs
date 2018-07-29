using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Library.LitJson;

#if true

namespace Library.Excel
{
    /// <summary>
    /// 引用 ICSharpCode.SharpZipLib.dll
    /// 引用 System.Data.dll
    /// </summary>
    public class ExcelByStream : ExcelByBase
    {
        /// <summary>
        /// https://www.cnblogs.com/Sandon/p/5175829.html
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="vals"></param>
        public override void WriteToExcel(string fileName, ListTable vals)
        {
            fileName = Path.ChangeExtension(fileName, ".xls");
            var dt = ConvertListToDataTable(vals);

            var value = ExportExcelByMemoryStream(dt);
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate);
            fs.Write(value, 0, value.Length);
            fs.Flush();
            fs.Close(); 
            return;
            File.WriteAllBytes(fileName, ExportExcelByMemoryStream(dt));
        }

        public override void WriteToOneExcel(string fileName, List<ListTable> list)
        {
            fileName = Path.ChangeExtension(fileName, ".xls");
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
            foreach (var table in list)
            {
                var value = ExportExcelByMemoryStream(ConvertListToDataTable(table));
                fs.Write(value, 0, value.Length);
                fs.Flush();
            }
            fs.Close();
            return;
        }

        public static byte[] ConvertToByteArray(DataTable dt)
        {
            byte[] bArrayResult = null; //用于存放序列化后的数据
            dt.RemotingFormat = SerializationFormat.Binary; //指定DataSet串行化格式是二进制
            MemoryStream ms = new MemoryStream();//定义内存流对象，用来存放DataSet序列化后的值
            IFormatter IF = new BinaryFormatter();//产生二进制序列化格式
            IF.Serialize(ms, dt);//串行化到内存中
            bArrayResult = ms.ToArray(); // 将DataSet转化成byte[]
            ms.Close();
            ms.Dispose();
            return bArrayResult;
        }

        private static byte[] ExportExcelByMemoryStream(DataTable dt)
        {
            //return ConvertToByteArray(dt);
            //创建一个内存流
            MemoryStream ms = new MemoryStream();

            //以指定的字符编码向指定的流写入字符
            StreamWriter sw = new StreamWriter(ms, Encoding.GetEncoding("GB2312"));

            StringBuilder sb = new StringBuilder();

            //写入标题
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append(dt.Columns[i].ColumnName + "\t");
            }
            //加入换行字符串
            sb.Append(Environment.NewLine);

            //写入内容
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sb.Append(dt.Rows[i][j] + "\t");
                }
                sb.Append(Environment.NewLine);
            }

            sw.Write(sb.ToString());
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
#endif