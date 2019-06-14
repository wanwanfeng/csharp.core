using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using Library;
using Library.Extensions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace protobuf_excel
{
    class Program
    {
        private enum MyEnum
        {
            [Description("CreateProto2"), TypeValue(typeof(CreateProto20))] CreateProto2,
            [Description("WriteProto2"), TypeValue(typeof(WriteProto2))] WriteProto2,
            [Description("CreateProto3"), TypeValue(typeof(CreateProto3))] CreateProto3,
            [Description("WriteProto3"), TypeValue(typeof(WriteProto3))] WriteProto3,
        }

        static void Main(string[] args)
        {
            SystemConsole.Run<MyEnum>();
        }
    }

    public class ExportExcel
    {

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
        /// <param name="lineCount">只去取前三行</param>
        /// <returns></returns>
        public static List<DataTable> ImportExcelToDataTable(string file, int lineCount = Int32.MaxValue)
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

                    DataTable dt = new DataTable
                    {
                        TableName = sheet.SheetName,
                    };

                    //表头  
                    IRow header = sheet.GetRow(sheet.FirstRowNum);
                    List<int> columns = new List<int>();
                    for (int i = 0; i < header.LastCellNum; i++)
                    {
                        //object obj = GetValueType(header.GetCell(i));
                        //if (obj == null || obj.ToString() == string.Empty)
                        //{
                        //    dt.Columns.Add(new DataColumn("Columns" + i));
                        //}
                        //else
                        //    dt.Columns.Add(new DataColumn(obj.ToString()));
                        //columns.Add(i);

                        dt.Columns.Add(new DataColumn("Columns" + i));
                        columns.Add(i);
                    }

                    //数据  
                    int max = Math.Max(0, Math.Min(lineCount, sheet.LastRowNum));
                    for (int i = sheet.FirstRowNum; i < max; i++)
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
    }

    public static class DataTableExtensions
    {
        public static List<string> GetHeaderList(this System.Data.DataTable dt)
        {
            List<string> list = new List<string>();
            foreach (DataColumn dc in dt.Columns)
                list.Add(dc.ColumnName);
            return list;
        }
    }
}
