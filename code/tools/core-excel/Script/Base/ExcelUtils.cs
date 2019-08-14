using Library.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Excel
{
    public partial class ExcelUtils
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
        public static IEnumerable<DataTable> ImportFromPath(string file, bool containsFirstLine)
        {
            return ExcelByNpoi.ImportExcelToDataTable(file, containsFirstLine);
        }

        /// <summary>
        /// 未经过滤原样输出
        /// </summary>
        /// <param name="file"></param>
        /// <param name="lineCount"></param>
        /// <returns></returns>
        public static IEnumerable<DataTable> ImportFromPath(string file, int lineCount = int.MaxValue)
        {
            return ExcelByNpoi.ImportExcelToDataTable(file, lineCount);
        }

        public static void ExportToExcel(DataTable dt, string file)
        {
            string newPath = CheckExport(dt, file, ".xlsx");
            ExcelByNpoi.ExportDataTableToExcel(newPath, dt);
        }

        public static void ExportToOneExcel(IEnumerable<DataTable> dts, string file)
        {
            string newPath = Path.ChangeExtension(file, "xlsx");
            DirectoryHelper.CreateDirectory(newPath);
            ExcelByNpoi.ExportDataTableToExcel(newPath, dts.ToArray());
        }
    }
}