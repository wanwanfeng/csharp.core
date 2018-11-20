using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Helper;

namespace Library.Excel
{
    public abstract partial class ExcelByBase
    {
        public partial class Data
        {
            private static string CheckExport(DataTable dt, string file, string extension)
            {
                file = string.IsNullOrEmpty(file) ? dt.FullName : file;
                string newPath = Path.ChangeExtension(file, extension);
                DirectoryHelper.CreateDirectory(newPath);
                return newPath;
            }

            /// <summary>
            /// 自定义的输出（首行为key）
            /// </summary>
            /// <param name="file"></param>
            /// <param name="containsFirstLine"></param>
            /// <returns></returns>
            public static IEnumerable<DataTable> ImportToDataTable(string file, bool containsFirstLine)
            {
                return ExcelByNpoi.ImportExcelToDataTable(file, containsFirstLine);
            }

            /// <summary>
            /// 未经过滤原样输出
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            public static IEnumerable<DataTable> ImportToDataTable(string file)
            {
                return ExcelByNpoi.ImportExcelToDataTable(file);
            }

            public static void ExportToExcel(DataTable dt, string file)
            {
                string newPath = CheckExport(dt, file, ".xlsx");
                ExcelByNpoi.ExportDataTableToExcel(newPath, dt);
            }

            public static void ExportToOneExcel(IEnumerable<DataTable> dts, string file)
            {
                string newPath = Path.ChangeExtension(file, "xlsx");
                FileHelper.CreateDirectory(newPath);
                ExcelByNpoi.ExportDataTableToExcel(newPath, dts.ToArray());
            }
        }

        public void HaHa(DataTable dt)
        {
            var headers = dt.GetHeaderList();

            //DataTable dataTable = (DataTable) dt.Clone();
            //dataTable.DefaultView.ToTable()
            string regex = "";

            foreach (string header in headers)
            {
                //List<object> vals = new List<object>();
                //foreach (DataRow dr in dt.Rows)
                //{
                //    vals.Add(dr[header]);
                //}
                //var content = string.Join(",", vals.Select(p => p.ToString()).ToArray());
                //if (Regex.IsMatch(content, regex))
                //{

                //}

                foreach (DataRow dr in dt.Rows)
                {
                    var content = dr[header].ToString();
                    if (Regex.IsMatch(content, regex))
                    {
                        dr.Delete();
                        break;
                    }
                }
            }
            //dataTable.AcceptChanges();
        }
    }
}