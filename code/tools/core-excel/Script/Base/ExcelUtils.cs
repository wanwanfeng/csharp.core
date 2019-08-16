﻿using Library.Helper;
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
        /// <param name="skip">是否跳过一些有效行</param>
        /// <returns></returns>
        public static IEnumerable<DataTable> ImportFromExcel(string file, bool containsFirstLine, int skip = 0)
        {
            return ExcelByNpoi.ImportExcelToDataTable(file, containsFirstLine, skip);
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

        public static void ExportToExcel(DataTable dt, string file, string extension = ".xlsx")
        {
            string newPath = CheckExport(dt, file, extension);
            ExcelByNpoi.ExportDataTableToExcel(newPath, dt);
        }

        public static void ExportToExcel(IEnumerable<DataTable> dts, string file, string extension = ".xlsx")
        {
            string newPath = Path.ChangeExtension(file, extension);
            DirectoryHelper.CreateDirectory(newPath);
            ExcelByNpoi.ExportDataTableToExcel(newPath, dts.ToArray());
        }
    }
}