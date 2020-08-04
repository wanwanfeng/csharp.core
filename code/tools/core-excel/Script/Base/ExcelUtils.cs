using Library.Helper;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Library.Excel
{
    public partial class ExcelUtils
    {
        private static string CheckExport(DataTable dt, string file, string extension)
        {
            string newPath = Path.ChangeExtension(file, extension);
            DirectoryHelper.CreateDirectory(newPath);
            return newPath;
        }

        /// <summary>
        /// 自定义的输出（首行为key）
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <param name="containsFirstLine">是否包含起始行（排除跳过行）</param>
        /// <param name="skip1">是否跳过一些有效行</param>
        /// <param name="skip2">是否跳过一些有效列</param>
        /// <returns></returns>
        public static IEnumerable<DataTable> ImportFromExcel(string file, bool containsFirstLine, int skip1 = 0, int skip2 = 0)
        {
            return ExcelByNpoi.ImportExcelToDataTable(file, containsFirstLine, skip1, skip2);
        }

        /// <summary>
        /// 未经过滤原样输出
        /// </summary>
        /// <param name="file"></param>
        /// <param name="lineCount"></param>
        /// <returns></returns>
        public static IEnumerable<DataTable> ImportFromExcel(string file, int lineCount = int.MaxValue)
        {
            return ExcelByNpoi.ImportExcelToDataTable(file, lineCount);
        }

        public static string ExportToExcel(DataTable dt, string file, string extension = ".xlsx")
        {
            string newPath = CheckExport(dt, file, extension);
            ExcelByNpoi.ExportDataTableToExcel(newPath, dt);
            return newPath;
        }

        public static string ExportToExcel(IEnumerable<DataTable> dts, string file, string extension = ".xlsx")
        {
            string newPath = Path.ChangeExtension(file, extension);
            DirectoryHelper.CreateDirectory(newPath);
            ExcelByNpoi.ExportDataTableToExcel(newPath, dts.ToArray());
            return newPath;
        }
    }
}