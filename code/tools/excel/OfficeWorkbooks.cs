using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using  Microsoft.Office.Interop.Excel;

namespace excel
{
    /// <summary>
    /// 使用 Microsoft.Office.Interop.Excel.dll
    /// 首先需要安装 office 的 excel，然后再找到 Microsoft.Office.Interop.Excel.dll 组件，添加到引用。
    /// </summary>
    public class OfficeWorkbooks
    {


        public static List<string> zimu = new List<string>()
        {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "AA",
            "AB",
            "AC",
            "AD",
            "AE",
            "AF",
            "AG",
            "AH",
            "AI",
            "AJ",
            "AK",
            "AL",
            "AM",
            "AN",
            "AO",
            "AP",
            "AQ",
            "AR",
            "AS",
            "AT",
            "AU",
            "AV",
            "AW",
            "AX",
            "AY",
            "AZ",
            "BA",
            "BB",
            "BC",
            "BD",
            "BE",
            "BF",
            "BG",
            "BH",
            "BI",
            "BJ",
            "BK",
            "BL",
            "BM",
            "BN",
            "BO",
            "BP",
            "BQ",
            "BR",
            "BS",
            "BT",
            "BU",
            "BV",
            "BW",
            "BX",
            "BY",
            "BZ",
            "CA",
            "CB",
            "CC",
            "CD",
            "CE",
            "CF",
            "CG",
            "CH",
            "CI",
            "CJ",
            "CK",
            "CL",
            "CM",
            "CN",
            "CO",
            "CP",
            "CQ",
            "CR",
            "CS",
            "CT",
            "CU",
            "CV",
            "CW",
            "CX",
            "CY",
            "CZ",
            "DA",
            "DB",
            "DC",
            "DD",
            "DE",
            "DF",
            "DG",
            "DH",
            "DI",
            "DJ",
            "DK",
            "DL",
            "DM",
            "DN",
            "DO",
            "DP",
            "DQ",
            "DR",
            "DS",
            "DT",
            "DU",
            "DV",
            "DW",
            "DX",
            "DY",
            "DZ",
            "EA",
            "EB",
            "EC",
            "ED",
            "EE",
            "EF",
            "EG",
            "EH",
            "EI",
            "EJ",
            "EK",
            "EL",
            "EM",
            "EN",
            "EO",
            "EP",
            "EQ",
            "ER",
            "ES",
            "ET",
            "EU",
            "EV",
            "EW",
            "EX",
            "EY",
            "EZ",
            "FA",
            "FB",
            "FC",
            "FD",
            "FE",
            "FF",
            "FG",
            "FH",
            "FI",
            "FJ",
            "FK",
            "FL",
            "FM",
            "FN",
            "FO",
            "FP",
            "FQ",
            "FR",
            "FS",
            "FT",
            "FU",
            "FV",
            "FW",
            "FX",
            "FY",
            "FZ",
            "GA",
            "GB",
            "GC",
            "GD",
            "GE",
            "GF",
            "GG",
            "GH",
            "GI",
            "GJ",
            "GK",
            "GL",
            "GM",
            "GN",
            "GO",
            "GP",
            "GQ",
            "GR",
            "GS",
            "GT",
            "GU",
            "GV",
            "GW",
            "GX",
            "GY",
            "GZ",
            "HA",
            "HB",
            "HC",
            "HD",
            "HE",
            "HF",
            "HG",
            "HH",
            "HI",
            "HJ",
            "HK",
            "HL",
            "HM",
            "HN",
            "HO",
            "HP",
            "HQ",
            "HR",
            "HS",
            "HT",
            "HU",
            "HV",
            "HW",
            "HX",
            "HY",
            "HZ",
            "IA",
            "IB",
            "IC",
            "ID",
            "IE",
            "IF",
            "IG",
            "IH",
            "II",
            "IJ",
            "IK",
            "IL",
            "IM",
            "IN",
            "IO",
            "IP",
            "IQ",
            "IR",
            "IS",
            "IT",
            "IU",
            "IV",
            "IW",
            "IX",
            "IY",
            "IZ",
            "JA",
            "JB",
            "JC",
            "JD",
            "JE",
            "JF",
            "JG",
            "JH",
            "JI",
            "JJ",
            "JK",
            "JL",
            "JM",
            "JN",
            "JO",
            "JP",
            "JQ",
            "JR",
            "JS",
            "JT",
            "JU",
            "JV",
            "JW",
            "JX",
            "JY",
            "JZ",
            "KA",
            "KB",
            "KC",
            "KD",
            "KE",
            "KF",
            "KG",
            "KH",
            "KI",
            "KJ",
            "KK",
            "KL",
            "KM",
            "KN",
            "KO",
            "KP",
            "KQ",
            "KR",
            "KS",
            "KT",
            "KU",
            "KV",
            "KW",
            "KX",
            "KY",
            "KZ",
            "LA",
            "LB",
            "LC",
            "LD",
            "LE",
            "LF",
            "LG",
            "LH",
            "LI",
            "LJ",
            "LK",
            "LL",
            "LM",
            "LN",
            "LO",
            "LP",
            "LQ",
            "LR",
            "LS",
            "LT",
            "LU",
            "LV",
            "LW",
            "LX",
            "LY",
            "LZ",
            "MA",
            "MB",
            "MC",
            "MD",
            "ME",
            "MF",
            "MG",
            "MH",
            "MI",
            "MJ",
            "MK",
            "ML",
            "MM",
            "MN",
            "MO",
            "MP",
            "MQ",
            "MR",
            "MS",
            "MT",
            "MU",
            "MV",
            "MW",
            "MX",
            "MY",
            "MZ",
            "NA",
            "NB",
            "NC",
            "ND",
            "NE",
            "NF",
            "NG",
            "NH",
            "NI",
            "NJ",
            "NK",
            "NL",
            "NM",
            "NN",
            "NO",
            "NP",
            "NQ",
            "NR",
            "NS",
            "NT",
            "NU",
            "NV",
            "NW",
            "NX",
            "NY",
            "NZ",
            "OA",
            "OB",
            "OC",
            "OD",
            "OE",
            "OF",
            "OG",
            "OH",
            "OI",
            "OJ",
            "OK",
            "OL",
            "OM",
            "ON",
            "OO",
            "OP",
            "OQ",
            "OR",
            "OS",
            "OT",
            "OU",
            "OV",
            "OW",
            "OX",
            "OY",
            "OZ",
        };

        public static void WriteToExcel(string filename, List<List<object>> vals)
        {
            ActionExcel(filename, workbook =>
            {
                var workSheet = (Worksheet) workbook.ActiveSheet;
                Console.WriteLine(workSheet.Name);
                for (int j = 0; j < vals.Count; j++)
                {
                    for (int i = 0; i < vals[j].Count; i++)
                    {
                        workSheet.Cells[j + 1, i + 1] = vals[j][i];
                    }
                }
                workSheet = null;
            });
        }

        public static void WriteToExcelOne(Dictionary<string,List<List<object>>> dic)
        {
            ActionExcelToOne(dic.Keys.First(), workbook =>
            {
                foreach (KeyValuePair<string, List<List<object>>> pair in dic)
                {
                    var filename = pair.Key;
                    var vals = pair.Value;
                    workbook.Worksheets.Add();
                    var workSheet = (Worksheet)workbook.ActiveSheet;
                    workSheet.Name = Path.GetFileNameWithoutExtension(filename);
                    Console.WriteLine(workSheet.Name);
                    for (int j = 0; j < vals.Count; j++)
                    {
                        for (int i = 0; i < vals[j].Count; i++)
                        {
                            workSheet.Cells[j + 1, i + 1] = vals[j][i];
                        }
                    }
                }
            });
        }

        public static Dictionary<string, List<List<object>>> ReadFromExcel(string filename)
        {
            Dictionary<string, List<List<object>>> cache = new Dictionary<string, List<List<object>>>();

            ActionExcel(filename, workbook =>
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
            }, false);
            return cache;
        }


        public static void ActionExcel(string filename, Action<Workbook> action, bool isWrite = true)
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

            if (isWrite)
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                //新增加一个工作簿，Add（）方法也可以直接传入参数 true
                workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
                //同样是新增一个工作簿，但是会弹出保存对话框
                //Microsoft.Office.Interop.Excel.Workbook workbook = excel.Application.Workbooks.Add(true);
                //新增加一个 Excel 表(sheet)
                //Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet) workbook.Worksheets[1];
            }
            else
            {
                if (File.Exists(filename))
                    workbook = workbooks.Open(filename, 0, false, 5, "", "", true, XlPlatform.xlWindows, "\t", false,
                        false, 0, true, 1, 0);
            }

            try
            {
                action(workbook);
                if (isWrite)
                {
                    //是否提示，如果想删除某个sheet页，首先要将此项设为fasle。
                    excel.DisplayAlerts = false;

                    //保存写入的数据，这里还没有保存到磁盘
                    workbook.Saved = true;

                    //workBook.SaveAs(filename, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);  
                    workbook.SaveCopyAs(filename.Replace("/", "\\"));
                }
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

        public static void ActionExcelToOne(string filename, Action<Microsoft.Office.Interop.Excel.Workbook> action,
            bool isWrite = true)
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
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet) workbook.Worksheets[1];
            try
            {
                action(workbook);
                if (isWrite)
                {
                    //是否提示，如果想删除某个sheet页，首先要将此项设为fasle。
                    excel.DisplayAlerts = false;
                    //保存写入的数据，这里还没有保存到磁盘
                    workbook.Saved = true;

                    string outName = Path.GetDirectoryName(filename) + "\\One.xlsx";
                    if (File.Exists(outName))
                        File.Delete(outName);
                    workbook.SaveAs(outName.Replace("/", "\\"));
                }
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
                Marshal.ReleaseComObject(worksheet);
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(workbooks);
                Marshal.ReleaseComObject(excel);

                worksheet = null;
                workbook = null;
                workbooks = null;
                excel = null;

                GC.Collect();
            }
        }
    }
}
