using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Excel;

#if true

namespace Library.Excel
{
    public class ExcelByReader : ExcelByBase
    {
        public override Dictionary<string, List<List<object>>> ReadFromExcels(string filename)
        {
            var dt = ExcelToTable(filename);
            return dt.ToDictionary(p => p.TableName, ConvertDataTableToList);
        }

        public override void WriteToExcel(string filename, List<List<object>> vals)
        {
            var dt = ConvertListToDataTable(vals);
            TableToExcel(filename, dt);
        }

        public override void WriteToOneExcel(string fileName, Dictionary<string, List<List<object>>> dic)
        {
            List<DataTable> dts = new List<DataTable>();
            foreach (KeyValuePair<string, List<List<object>>> pair in dic)
            {
                var dt = ConvertListToDataTable(pair.Value);
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
        private static List<DataTable> ExcelToTable(string path)
        {
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet dataSet = excelReader.AsDataSet();
            var dts = new List<DataTable>();
            foreach (DataTable table in dataSet.Tables)
            {
                dts.Add(table);
            }
            stream.Dispose();
            stream.Close();
            return dts;
        }

        private void TableToExcel(string fileName, params DataTable[] dataTable)
        {
            throw new Exception("此方法无法写入文件！");
        }

    }
}
#endif