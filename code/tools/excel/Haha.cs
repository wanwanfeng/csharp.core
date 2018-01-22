using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using  Microsoft.Office.Interop.Excel;

namespace excel
{
    class Haha
    {
        public Haha(string filename, int sheet = 1)
        {
            WriteExcel(filename, sheet, workSheet =>
            {
                Console.WriteLine(workSheet.Name);
            });
        }

        public Haha(string filename, List<string> keys, List<List<object>> vals, int sheet = 1)
        {
            WriteExcel(filename, sheet, workSheet =>
            {

                Console.WriteLine(workSheet.Name);

                vals.Insert(0, keys.Select(p => (object) p).ToList());
                for (int j = 0; j < vals.Count; j++)
                {
                    for (int i = 0; i < vals[j].Count; i++)
                    {
                        workSheet.Cells[j + 1, i + 1] = vals[j][i];
                    }
                }
            });
        }

        public void WriteExcel(string filename,int sheet,Action<Worksheet> action)
        {
            //new an excel object  
            Application excelApp = new Application();
            if (excelApp == null)
            {
                // if equal null means EXCEL is not installed.  
                Console.WriteLine("Excel is not properly installed!");
                return;
            }

            // open a workbook,if not exist, create a new one  
            Workbook workBook;
            if (File.Exists(filename))
            {
                workBook = excelApp.Workbooks.Open(filename, 0, false, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            }
            else
            {
                workBook = excelApp.Workbooks.Add(true);
            }

            //new a worksheet  
            Worksheet workSheet = workBook.ActiveSheet as Worksheet;

            //write data  
            //workSheet = (Worksheet)workBook.Worksheets.get_Item(1); //获得第i个sheet，准备写入  
            //workSheet.Cells[1, 3] = "(1,3)Content";

            action((Worksheet)workBook.Worksheets.get_Item(sheet));

            //set visible the Excel will run in background  
            excelApp.Visible = false;
            //set false the alerts will not display  
            excelApp.DisplayAlerts = false;

            //workBook.SaveAs(filename, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);  
            workBook.SaveAs(filename.Replace("/","\\"));
            workBook.Close(false, Missing.Value, Missing.Value);

            //quit and clean up objects  
            excelApp.Quit();
            workSheet = null;
            workBook = null;
            excelApp = null;
            GC.Collect();
        }
    }
}
