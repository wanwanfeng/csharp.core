using Library.Helper;
using System;
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
        /// 
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <param name="keyLine">自定义的行为key</param>
        /// <param name="SkipRows">是否跳过一些有效行</param>
        /// <param name="SkipColumns">是否跳过一些有效列</param>
        /// <param name="TakeColumns">读取的总行数</param>
        /// <returns></returns>
        public static IEnumerable<DataTable> ImportFromExcel(string file, int keyLine = -1, int SkipRows = 0, int SkipColumns = 0, int TakeColumns = int.MaxValue)
        {
            return ExcelByNpoi.ImportExcelToDataTable(file, keyLine, SkipRows, SkipColumns, TakeColumns);
        }

        /// <summary>
        /// 未经过滤原样输出
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static IEnumerable<DataTable> ImportFromExcel(string file)
        {
            if (!int.TryParse(Environment.GetEnvironmentVariable("KeyLine"), out int keyLine)) keyLine = -1;
            if (!int.TryParse(Environment.GetEnvironmentVariable("SkipRows"), out int SkipRows)) SkipRows = 0;
            if (!int.TryParse(Environment.GetEnvironmentVariable("SkipColumns"), out int SkipColumns)) SkipColumns = 0;
            if (!int.TryParse(Environment.GetEnvironmentVariable("TakeColumns"), out int TakeColumns)) TakeColumns = int.MaxValue;
            return ExcelByNpoi.ImportExcelToDataTable(file, keyLine, SkipRows, SkipColumns, TakeColumns);
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