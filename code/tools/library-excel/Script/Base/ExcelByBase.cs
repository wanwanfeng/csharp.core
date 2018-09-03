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
                FileHelper.CreateDirectory(newPath);
                return newPath;
            }

            public static List<DataTable> ImportToDataTable(string file)
            {
                return ExcelByNpoi.ImportExcelToDataTable(file, false);
            }

            public static void ExportToExcel(DataTable dt, string file)
            {
                string newPath = CheckExport(dt, file, ".xlsx");
                ExcelByNpoi.ExportDataTableToExcel(newPath, dt);
            }

            public static void ExportToOneExcel(List<DataTable> dts, string file)
            {
                string newPath = Path.ChangeExtension(file, ".xlsx");
                FileHelper.CreateDirectory(newPath);
                ExcelByNpoi.ExportDataTableToExcel(newPath, dts.ToArray());
            }

            #region ListTable

            private static string CheckExport(ListTable lt, string file, string extension)
            {
                file = string.IsNullOrEmpty(file) ? lt.FullName : file;
                string newPath = Path.ChangeExtension(file, extension);
                FileHelper.CreateDirectory(newPath);
                return newPath;
            }

            public static List<ListTable> ImportToListTable(string file)
            {
                return ImportToDataTable(file).Select(ConvertToListTable).ToList();
            }

            public static void ExportToExcel(ListTable lt, string file)
            {
                ExportToExcel(List.ConvertToDataTable(lt), file);
            }

            public static void ExportToOneExcel(List<ListTable> dts, string file)
            {
                ExportToOneExcel(dts.Select(List.ConvertToDataTable).ToList(), file);
            }

            #endregion
        }


        public void HaHa(DataTable dt)
        {
            var headers = Data.GetHeaderList(dt);

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