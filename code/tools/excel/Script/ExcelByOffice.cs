using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using  Microsoft.Office.Interop.Excel;

namespace excel.Script
{
    /// <summary>
    /// 使用 Microsoft.Office.Interop.Excel.dll
    /// 首先需要安装 office 的 excel，然后再找到 Microsoft.Office.Interop.Excel.dll 组件，添加到引用。
    /// </summary>
    public class ExcelByOffice : ExcelByBase
    {
        public override Dictionary<string, List<List<object>>> ReadFromExcels(string filename)
        {
            Dictionary<string, List<List<object>>> cache = new Dictionary<string, List<List<object>>>();

            ActionExcelRead(filename, workbook =>
            {
                foreach (var item in workbook.Sheets)
                {
                    var workSheet = (Worksheet) item;
                    Console.WriteLine(workSheet.Name);

                    var hangCount = workSheet.UsedRange.Cells.Rows.Count;
                    var lieCount = workSheet.UsedRange.Cells.Columns.Count;

                    if (hangCount == 0 || lieCount == 0)
                        continue;
                    List<object[,]> tempList = new List<object[,]>();
                    for (int j = 0; j < lieCount; j++)
                        tempList.Add((object[,]) workSheet.Cells.Range[zimu[j] + "1", zimu[j] + hangCount].Value);


                    Dictionary<object, List<object>> dic = new Dictionary<object, List<object>>();
                    foreach (object[,] objectse in tempList)
                    {
                        if (objectse == null)
                            continue;
                        int index = 0;
                        foreach (object o in objectse)
                        {
                            index++;
                            List<object> res = null;
                            if (dic.TryGetValue(index, out res))
                                res.Add(o);
                            else
                                dic.Add(index, new List<object> {o});
                        }
                    }

                    if (dic.Count == 0) continue;
                    cache[workSheet.Name] = dic.Values.ToList();
                }
            });
            return cache;
        }

        public override void WriteToExcel(string filename, List<List<object>> vals)
        {
            //http://bbs.csdn.net/topics/390201171
            ActionExcelWrite(filename, workbook =>
            {
                var workSheet = (Worksheet) workbook.ActiveSheet;
                var f = Path.GetFileNameWithoutExtension(filename);
                Console.WriteLine(workSheet.Name);
                //第一种：Excel.Application的Cell by Cell
                for (int j = 0; j < vals.Count; j++)
                {
                    for (int i = 0; i < vals[j].Count; i++)
                    {
                        workSheet.Cells[j + 1, i + 1] = vals[j][i];
                    }
                    Console.WriteLine(f + "/" + j + "/" + vals.Count);
                }
                workSheet = null;
            });
        }

        public override void WriteToOneExcel(string fileName, Dictionary<string, List<List<object>>> dic)
        {
            ActionExcelWrite(fileName, workbook =>
            {
                var keys = dic.Keys.Reverse().ToList();
                foreach (string filename in keys)
                {
                    var vals = dic[filename];
                    var workSheet = (Worksheet) workbook.ActiveSheet;
                    var f = Path.GetFileNameWithoutExtension(filename);
                    workSheet.Name = f;
                    Console.WriteLine(workSheet.Name);
                    for (int j = 0; j < vals.Count; j++)
                    {
                        for (int i = 0; i < vals[j].Count; i++)
                        {
                            workSheet.Cells[j + 1, i + 1] = vals[j][i];
                        }
                        Console.WriteLine(f + "/" + j + "/" + vals.Count);
                    }
                    if (filename != keys.LastOrDefault())
                        workbook.Worksheets.Add();
                    workSheet = null;
                }
            });
        }

        private static void ActionExcelRead(string filename, Action<Workbook> action)
        {
            //引用Excel对象
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            if (excel == null)
            {
                // if equal null means EXCEL is not installed.  
                Console.WriteLine("Excel is not properly installed!");
                return;
            }

            //设置为不可见，操作在后台执行，为 true 的话会打开 Excel
            excel.Visible = false;

            //打开时设置为全屏显式
            //excel.DisplayFullScreen = true;

            //初始化工作簿
            Microsoft.Office.Interop.Excel.Workbooks workbooks = excel.Workbooks;

            Microsoft.Office.Interop.Excel.Workbook workbook = null;

            if (File.Exists(filename))
                workbook = workbooks.Open(filename, 0, false, 5, "", "", true, XlPlatform.xlWindows, "\t", false,
                    false, 0, true, 1, 0);
            try
            {
                action(workbook);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                workbook.Close(false, Type.Missing, Type.Missing);
                workbooks.Close();

                //关闭退出
                excel.Quit();

                KillExcel.Kill(new IntPtr(excel.Hwnd));

                //释放 COM 对象
                //Marshal.ReleaseComObject(worksheet);
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(workbooks);
                Marshal.ReleaseComObject(excel);

                //worksheet = null;
                workbook = null;
                workbooks = null;
                excel = null;

                GC.Collect();
            }
        }

        private static void ActionExcelWrite(string filename, Action<Workbook> action)
        {
            //引用Excel对象
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            if (excel == null)
            {
                // if equal null means EXCEL is not installed.  
                Console.WriteLine("Excel is not properly installed!");
                return;
            }

            //设置为不可见，操作在后台执行，为 true 的话会打开 Excel
            excel.Visible = false;

            //打开时设置为全屏显式
            //excel.DisplayFullScreen = true;

            //初始化工作簿
            Microsoft.Office.Interop.Excel.Workbooks workbooks = excel.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;

            //新增加一个工作簿，Add（）方法也可以直接传入参数 true
            workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            //同样是新增一个工作簿，但是会弹出保存对话框
            //Microsoft.Office.Interop.Excel.Workbook workbook = excel.Application.Workbooks.Add(true);

            //新增加一个 Excel 表(sheet)
            //Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet) workbook.Worksheets[1];
            try
            {
                action(workbook);
                //是否提示，如果想删除某个sheet页，首先要将此项设为fasle。
                excel.DisplayAlerts = false;
                //保存写入的数据，这里还没有保存到磁盘
                workbook.Saved = true;

                //workBook.SaveAs(filename, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);  
                if (File.Exists(filename))
                    File.Delete(filename);
                workbook.SaveAs(filename.Replace("/", "\\"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                workbook.Close(false, Type.Missing, Type.Missing);
                workbooks.Close();

                //关闭退出
                excel.Quit();

                KillExcel.Kill(new IntPtr(excel.Hwnd));

                //释放 COM 对象
                //Marshal.ReleaseComObject(worksheet);
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(workbooks);
                Marshal.ReleaseComObject(excel);

                //worksheet = null;
                workbook = null;
                workbooks = null;
                excel = null;

                GC.Collect();
            }
        }
    }
}
