using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class ExcelByNpoi : ExcelByBase
    {
        public override Dictionary<string, List<List<object>>> ReadFromExcels(string filename)
        {
            var dt = ExcelToDataTable(filename);
            return dt.ToDictionary(p => p.TableName, q=>ConvertDataTableToList(q));
        }

        public override void WriteToExcel(string filename, List<List<object>> vals)
        {
            var dt = ConvertListToDataTable(vals);
            DataTableToExcel(filename, dt);
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
            DataTableToExcel(fileName, dts.ToArray());
        }

        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <returns></returns>
        public static List<DataTable> ExcelToDataTable(string file)
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
                    return null;
                }

                var list = new List<DataTable>();
                for (int index = 0; index < workbook.NumberOfSheets; index++)
                {
                    ISheet sheet = workbook.GetSheetAt(index);
                    if (sheet.LastRowNum == 0) continue;

                    DataTable dt = new DataTable(sheet.SheetName);

                    //表头  
                    IRow header = sheet.GetRow(sheet.FirstRowNum);
                    List<int> columns = new List<int>();
                    for (int i = 0; i < header.LastCellNum; i++)
                    {
                        object obj = GetValueType(header.GetCell(i));
                        if (obj == null || obj.ToString() == string.Empty)
                        {
                            dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                        }
                        else
                            dt.Columns.Add(new DataColumn(obj.ToString()));
                        columns.Add(i);
                    }
                    //数据  
                    //for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)//不包括第一行
                    for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)//包括第一行
                    {
                        DataRow dr = dt.NewRow();
                        bool hasValue = false;
                        foreach (int j in columns)
                        {
                            dr[j] = GetValueType(sheet.GetRow(i).GetCell(j));
                            if (dr[j] != null && dr[j].ToString() != string.Empty)
                            {
                                hasValue = true;
                            }
                        }
                        if (hasValue)
                        {
                            dt.Rows.Add(dr);
                        }
                    }

                    list.Add(dt);
                }
                return list;
            }
        }

        public static Func<ISheet, DataTable, DataTable> OnSheetAction;

        /// <summary>
        /// Datable导出成Excel
        /// </summary>
        /// <param name="file">导出路径(包括文件名与扩展名)</param>
        /// <param name="dts"></param>
        public static void DataTableToExcel(string file, params DataTable[] dts)
        {
            IWorkbook workbook;
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
                    workbook = null;
                    break;
            }
            if (workbook == null)
            {
                return;
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
                    cell.SetCellValue(dt.Columns[i].ColumnName);
                }

                //数据  
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row1 = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        ICell cell = row1.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][j].ToString());
                    }
                }
                if (OnSheetAction != null)
                {
                    if (OnSheetAction.Invoke(sheet, dt) == null)
                    {
                        workbook.RemoveSheetAt(workbook.GetSheetIndex(sheet));
                    }
                }

                //string regex = "([\u4E00-\u9FA5]+)|([\u4E00-\u9FA5]')|([\u30A0-\u30FF])|([\u30A0-\u30FF])";
                //var list = ConvertDataTableToRowsList(dt).Select(p => string.Join("|", p.Cast<string>().ToArray())).Select(p => Regex.IsMatch(p, regex)).ToList();
                //for (int i = 0; i < list.Count; i++)
                //{
                //    var show = list[i];
                //    sheet.SetColumnHidden(i, !show);
                //    //if (show)
                //    //    sheet.SetColumnWidth(0, 200*256 + 200);
                //}
            }


            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
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
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;
            }
        }
    }
}


#endif