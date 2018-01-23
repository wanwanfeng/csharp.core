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
        public static void WriteToExcel(string filename, List<List<object>> vals)
        {
            ActionExcel(filename, workSheet =>
            {
                Console.WriteLine(workSheet.Name);
                for (int j = 0; j < vals.Count; j++)
                {
                    for (int i = 0; i < vals[j].Count; i++)
                    {
                        workSheet.Cells[j + 1, i + 1] = vals[j][i];
                    }
                }
            });
        }

        public static void WriteToExcelOne(string filename, List<List<object>> vals)
        {
            ActionExcelToOne(filename, workSheet =>
            {
                workSheet.Name = Path.GetFileNameWithoutExtension(filename);
                Console.WriteLine(workSheet.Name);
                for (int j = 0; j < vals.Count; j++)
                {
                    for (int i = 0; i < vals[j].Count; i++)
                    {
                        workSheet.Cells[j + 1, i + 1] = vals[j][i];
                    }
                }
            });
        }

        public static List<List<object>> ReadFromExcel(string filename)
        {
            List<List<object>> list = new List<List<object>>();
            ActionExcel(filename, workSheet =>
            {
                Console.WriteLine(workSheet.Name);

                for (int i = 0; i < workSheet.Cells.Column; i++)
                {
                    List<object> res = new List<object>();
                    for (int j = 0; j < workSheet.Cells.Row; j++)
                    {
                        res.Add(workSheet.Cells[j + 1, i + 1]);
                    }
                    list.Add(res);
                }
            }, false);
            return list;
        }


        public static void ActionExcel(string filename, Action<Worksheet> action, bool isWrite = true)
        {
            //引用Excel对象
            Application excelApp = new Application();
            //new an excel object  
            if (excelApp == null)
            {
                // if equal null means EXCEL is not installed.  
                Console.WriteLine("Excel is not properly installed!");
                return;
            }
            //set visible the Excel will run in background  
            excelApp.Visible = false;
            //set false the alerts will not display  
            excelApp.DisplayAlerts = false;
            // open a workbook,if not exist, create a new one  
            Workbook workBook;
            if (File.Exists(filename))
            {
                workBook = excelApp.Workbooks.Open(filename, 0, false, 5, "", "", true, XlPlatform.xlWindows, "\t",
                    false, false, 0, true, 1, 0);
            }
            else
            {
                workBook = excelApp.Workbooks.Add(true);
            }

            //new a worksheet  
            //Worksheet workSheet = workBook.ActiveSheet as Worksheet;
            //write data  
            //workSheet = (Worksheet)workBook.Worksheets.get_Item(1); //获得第i个sheet，准备写入  
            //workSheet.Cells[1, 3] = "(1,3)Content";

            action((Worksheet) workBook.ActiveSheet);
            if (isWrite)
            {
                //workBook.SaveAs(filename, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);  
                workBook.SaveAs(filename.Replace("/", "\\"));
            }
            workBook.Close(false, Missing.Value, Missing.Value);

            //quit and clean up objects  
            excelApp.Quit();
            //workSheet = null;
            workBook = null;
        }

        public static void ActionExcelToOne(string filename, Action<Worksheet> action, bool isWrite = true)
        {
            //引用Excel对象
            Application excelApp = new Application();
            //new an excel object  
            if (excelApp == null)
            {
                // if equal null means EXCEL is not installed.  
                Console.WriteLine("Excel is not properly installed!");
                return;
            }
            //set visible the Excel will run in background  
            excelApp.Visible = false;
            //set false the alerts will not display  
            excelApp.DisplayAlerts = false;
            string outName = Path.GetDirectoryName(filename) + "\\One.xlsx";
            // open a workbook,if not exist, create a new one  
            Workbook workBook;
            if (File.Exists(outName))
            {
                workBook = excelApp.Workbooks.Open(outName, 0, false, 5, "", "", true, XlPlatform.xlWindows, "\t",
                    false, false, 0, true, 1, 0);
            }
            else
            {
                workBook = excelApp.Workbooks.Add(true); //引用Excel工作簿
            }

            action((Worksheet) workBook.ActiveSheet);

            if (isWrite)
            {
                workBook.Worksheets.Add();
                //workBook.SaveAs(filename, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);  
                workBook.SaveAs(outName.Replace("/", "\\"));
                workBook.Close(false, Missing.Value, Missing.Value);
            }

            //quit and clean up objects  
            excelApp.Quit();
            workBook = null;
        }
    }
}
