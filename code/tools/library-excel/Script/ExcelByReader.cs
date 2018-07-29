using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Excel;
using Library.LitJson;

#if true

namespace Library.Excel
{
    /// <summary>
    /// 引用 ICSharpCode.SharpZipLib.dll
    /// 引用 System.Data.dll
    /// 引用 Excel.dll
    /// </summary>
    public class ExcelByReader : ExcelByBase
    {
        public override Dictionary<string, ListTable> ReadFromExcels(string filename)
        {
            var dt = ExcelToTable(filename);
            return dt.ToDictionary(p => p.TableName, ConvertDataTableToList);
        }

        [Obsolete("此类本方法无效！", true)]

        public override void WriteToExcel(string filename, ListTable list)
        {
      
        }

        [Obsolete("此类本方法无效！", true)]
        public override void WriteToOneExcel(string fileName, List<ListTable> dic)
        {
            
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
            IExcelDataReader excelReader = null;
            var dts = new List<DataTable>();
            try
            {
                var extension = Path.GetExtension(path);
                if (extension != null)
                    switch (extension.ToLower())
                    {
                        case ".xls":
                            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                            break;
                        case ".xlsx":
                            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                            break;
                        default:
                            Ldebug.LogError("文件类型错误！！！");
                            break;
                    }

                if (excelReader != null)
                {
                    DataSet dataSet = excelReader.AsDataSet();
                    foreach (DataTable table in dataSet.Tables)
                    {
                        dts.Add(table);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                stream.Dispose();
                stream.Close();
            }
            return dts;
        }
    }
}
#endif