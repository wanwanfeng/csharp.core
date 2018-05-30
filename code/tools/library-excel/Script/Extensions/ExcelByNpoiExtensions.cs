using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Library.Excel
{
    public class ExcelByNpoiExtensions : ExcelByNpoi
    {
        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <param name="regex">正则匹配列</param>
        /// <returns></returns>
        public static void DataTable(string file, string regex)
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
                    var dt = GetDataTable(index, workbook);
                    SetDataTable(fileExt, dt, workbook);
                }
            }

            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            var fileName = Path.GetFileNameWithoutExtension(file);
            var newName = file.Replace(fileName, fileName + "_New");
            using (FileStream fs = new FileStream(newName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }

        private static void SetDataTable(string fileExt, DataTable dt, IWorkbook workbook)
        {
            if (dt == null)
            {
                return;
            }
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
        }

        private static DataTable GetDataTable(int index,IWorkbook workbook)
        {
            ISheet sheet = workbook.GetSheetAt(index);
            if (sheet.LastRowNum == 0) return null;
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
            for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++) //包括第一行
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
            return dt;
        }
    }
}