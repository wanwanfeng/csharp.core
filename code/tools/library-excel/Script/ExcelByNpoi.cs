﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Library.Helper;

#if ExcelByNpoi

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Library.Excel
{
    /// <summary>
    /// 引用 ICSharpCode.SharpZipLib.dll
    /// 引用 System.Data.dll
    /// 引用 NPOI
    /// 引用 NPOI.OOXML
    /// 引用 NPOI.OpenXml4Net
    /// 引用 NPOI.OpenXmlFormats
    /// </summary>
    internal class ExcelByNpoi : ExcelByBase
    {
        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <param name="runAction"></param>
        /// <returns></returns>
        public static void ImportExcel(string file, Action<string, ISheet> runAction)
        {
            IWorkbook workbook = null;
            string fileExt = Path.GetExtension(file).ToLower();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                switch (fileExt)
                {
                    case ".xlsx":
                        workbook = new XSSFWorkbook(fs);
                        break;
                    case ".xls":
                        workbook = new HSSFWorkbook(fs);
                        break;
                    default:
                        workbook = null;
                        break;
                }
                if (workbook == null)
                {
                    return;
                }

                for (int index = 0; index < workbook.NumberOfSheets; index++)
                {
                    ISheet sheet = workbook.GetSheetAt(index);
                    if (sheet.LastRowNum == 0) continue;

                    runAction.Invoke(file, sheet);
                }
            }
        }

        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        protected static object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC: 
                {
                    if (DateUtil.IsCellDateFormatted(cell))
                        return cell.DateCellValue; //修正日期显示
                    return cell.NumericCellValue; //返回正常数字
                }
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;
            }
        }

        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <param name="containsFirstLine">是否包含第一行</param>
        /// <returns></returns>
        public static List<DataTable> ImportExcelToDataTable(string file, bool containsFirstLine)
        {
            var list = new List<DataTable>();

            ImportExcel(file, (fileName, sheet) =>
            {
                DataTable dt = new DataTable
                {
                    FullName = file,
                    TableName = sheet.SheetName,
                };

                //表头  
                IRow header = sheet.GetRow(sheet.FirstRowNum);
                for (int i = 0; i < header.LastCellNum; i++)
                {
                    object obj = GetValueType(header.GetCell(i));
                    if (obj == null || obj.ToString() == string.Empty)
                        dt.Columns.Add(new DataColumn("Columns" + i));
                    else
                        dt.Columns.Add(new DataColumn(obj.ToString()));
                }
                //数据  
                //for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)//不包括第一行
                //for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)包括第一行

                var srartLine = sheet.FirstRowNum + (containsFirstLine ? 0 : 1);
                for (int i = srartLine; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.Count == 0) continue;
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                        dr[j] = GetValueType(row.GetCell(j));
                    dt.Rows.Add(dr);
                }
                list.Add(dt);
            });
            return list;
        }

        /// <summary>
        /// Datable导出成Excel
        /// </summary>
        /// <param name="file">导出路径(包括文件名与扩展名)</param>
        /// <param name="dts"></param>
        public static void ExportDataTableToExcel(string file, params DataTable[] dts)
        {
            IWorkbook workbook = null;
            string fileExt = Path.GetExtension(file).ToLower();
            switch (fileExt)
            {
                case ".xlsx":
                    workbook = new XSSFWorkbook();
                    break;
                case ".xls":
                    workbook = new HSSFWorkbook();
                    break;
                default:
                    throw new Exception("保存的文件格式不符合要求 ！");
            }

            foreach (DataTable dt in dts)
            {
                if (fileExt == ".xls" && dt.Rows.Count > 65536)
                {
                    throw new Exception("数据量过大，请保存为.xlsx");
                }

                ISheet sheet = string.IsNullOrEmpty(dt.TableName)
                    ? workbook.CreateSheet("Sheet1")
                    : workbook.CreateSheet(dt.TableName);

                //表头  
                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ICell cell = row.CreateCell(i);
                    var value = dt.Columns[i].ColumnName;
                    cell.SetCellValue(value);
                }

                //数据  
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row1 = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        var value = dt.Rows[i][j];
                        ICell cell = row1.CreateCell(j);

                        if (value is int)
                            cell.SetCellValue((int) value);
                        if (value is bool)
                            cell.SetCellValue((bool) value);
                        cell.SetCellValue(value.ToString());
                    }
                }

                //默认一个样式以及自动宽度
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var style = workbook.CreateCellStyle();
                    sheet.SetDefaultColumnStyle(i, style);
                    sheet.AutoSizeColumn(i);
                }
            }


            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            DirectoryHelper.CreateDirectory(file);
            //保存为Excel文件  
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }


        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <returns></returns>
        public static List<DataTable> ImportExcelToDataTable(string file)
        {
            var list = new List<DataTable>();
            ImportExcel(file, (fileName, sheet) =>
            {
                //表名
                DataTable dt = new DataTable
                {
                    FullName = file,
                    TableName = sheet.SheetName,
                };

                //表头  
                var maxColumsNum = 0;
                for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.Count == 0) continue;
                    maxColumsNum = Math.Max(sheet.GetRow(i).LastCellNum, maxColumsNum);
                }
                for (int i = 0; i < maxColumsNum; i++)
                    dt.Columns.Add(new DataColumn("Columns" + i));

                //数据  
                for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.Count == 0) continue;

                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                        dr[j] = GetValueType(row.GetCell(j));

                    dt.Rows.Add(dr);
                }
                list.Add(dt);
            });
            return list;
        }
    }
}


#endif