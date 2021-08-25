using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
        public static void ImportExcel(string file, Action<string, IWorkbook, ISheet> runAction)
        {
            IWorkbook workbook = null;
            string fileExt = Path.GetExtension(file).ToLower();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                switch (fileExt)
                {
                    case ".xlsx":
                    case ".xlsm":
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

                if (!int.TryParse(Environment.GetEnvironmentVariable("SkipSheet"), out int skipSheetIndex)) skipSheetIndex = 0;
                if (!int.TryParse(Environment.GetEnvironmentVariable("TakeSheet"), out int takeSheetIndex)) takeSheetIndex = int.MaxValue;

                for (var index = 0; index < workbook.NumberOfSheets; index++)
				{
                    if (index < skipSheetIndex) continue;
                    if (index >= skipSheetIndex + takeSheetIndex) continue;

                    ISheet sheet = workbook.GetSheetAt(index);
                    if (sheet.LastRowNum == 0) continue;
                    runAction.Invoke(file, workbook, sheet);
                }
            }
        }

        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        protected static object GetValueType(ICell cell, IWorkbook workbook)
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
					switch (workbook)
					{
						case XSSFWorkbook _:
							return GetValueType(new XSSFFormulaEvaluator(workbook).EvaluateInCell(cell), workbook);
						case HSSFWorkbook _:
							return GetValueType(new HSSFFormulaEvaluator(workbook).EvaluateInCell(cell), workbook);
						default:
							return cell.CellFormula;
					}
				default:
                    return "";
            }
        }

        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <returns></returns>
        public static List<DataTable> ImportExcelToDataTable(string file, int keyLine = -1, int SkipRows = 0, int SkipColumns = 0, int TakeColumns = int.MaxValue)
        {
            var list = new List<DataTable>();

            ImportExcel(file, (fileName, workbook, sheet) =>
            {
                DataTable dt = new DataTable
                {
                    TableName = sheet.SheetName,
                };

				//表头
                IRow header = sheet.GetRow(Math.Max(0, keyLine));
				foreach (var item in header)
				{
					object obj = GetValueType(item, workbook);
					var columnsName = (obj == null || obj.ToString() == string.Empty || keyLine == -1) ? ("Columns" + dt.Columns.Count) : obj.ToString();
					dt.Columns.Add(new DataColumn(columnsName));
				}

                //数据
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    if (i == keyLine) continue;//跳过作为key的行
                    if (i < SkipRows) continue;//跳过放弃的行
                    if (i >= SkipColumns + TakeColumns) continue;//跳过放弃的行

                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.Count == 0) continue;

                    //创建行
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        dr[j] = GetValueType(row.GetCell(j), workbook);
                    }
                    dt.Rows.Add(dr);
                }

                //处理跳过列
                for (int i = 0, max = Math.Max(0, SkipColumns); i < max; i++)
                {
                    dt.Columns.RemoveAt(0);
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
                case ".xlsm":
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
				Dictionary<string, HashSet<int>> charactersDict = new Dictionary<string, HashSet<int>>();

				IRow row = sheet.CreateRow(0);
				for (int i = 0; i < dt.Columns.Count; i++)
				{
					ICell cell = row.CreateCell(i);
					var value = dt.Columns[i].ColumnName;
					cell.SetCellValue(value);
					charactersDict[value] = new HashSet<int>() { value.Length };
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
							cell.SetCellValue((int)value);
						if (value is bool)
							cell.SetCellValue((bool)value);
						cell.SetCellValue(value.ToString());

						charactersDict[dt.Columns[j].ColumnName].Add(value.ToString().Length);
					}
				}

				//默认一个样式以及
				for (int i = 0; i < dt.Columns.Count; i++)
				{
					var style = workbook.CreateCellStyle();
					sheet.SetDefaultColumnStyle(i, style);

					//sheet.AutoSizeColumn(i); //自动宽度

					var columnWidth = charactersDict[dt.Columns[i].ColumnName].OrderBy(p => p).Skip(1).Reverse().Skip(1).FirstOrDefault();
					sheet.SetColumnWidth(i, Math.Min(columnWidth, 50) * 256);
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
    }
}


#endif