using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;
using Library.LitJson;
using Script;
//#define ExcelByOleDb
//#define ExcelByNpoi
//#define ExcelByOffice
//#define ExcelByStream

#if ExcelByOleDb
using ExcelClass = Library.Excel.ExcelByOleDb;
#elif ExcelByNpoi
using ExcelClass = Library.Excel.ExcelByNpoi;
#elif ExcelByOffice
using ExcelClass = Library.Excel.ExcelByOffice;
#elif ExcelByStream
using ExcelClass = Library.Excel.ExcelByStream;
#endif

namespace Library.Excel
{
    public enum CaoType
    {
        CompareExcel = 0,
        JsonToCsv = 1,
        JsonToExcel = 2,
        JsonToOneExcel = 3,
        ExcelToJson = 4,
        ExcelToOneExcel = 5,
    }


    internal class Program
    {
        static Program()
        {
            Ldebug.OnActionLog += Console.WriteLine;
            Ldebug.OnActionLogError += Console.WriteLine;
        }

        public static string InputPath { get; set; }

        private static void Main(string[] args)
        {
            /* var hh = new ExcelClass().ReadFromExcels("ff.xlsx");
            foreach (KeyValuePair<string, List<List<object>>> pair in hh)
            {
                //ExcelClass.ConvertListToJson(pair);
               // ExcelClass.ConvertDataTableToXml(ExcelClass.ConvertListToDataTable(pair.Value, pair.Key), pair.Key);
                ExcelClass.ConvertDataTableToCsv(ExcelClass.ConvertListToDataTable(pair.Value, pair.Key), pair.Key);
            }
            Console.ReadKey();
            return;
            Dictionary<string, List<List<object>>> dic = new Dictionary<string, List<List<object>>>()
            {
                {
                    "xx.xlsx", new List<List<object>>()
                    {
                        new List<object>() {"haha", "lala", "xx"},
                        new List<object>() {"2", "3", "4"},
                        new List<object>() {"p", "6", "2"},
                    }
                },
                 {
                    "yy.xlsx", new List<List<object>>()
                    {
                        new List<object>() {"haha", "lala", "xx"},
                        new List<object>() {"2", "3", "4"},
                        new List<object>() {"p", "6", "2"},
                    }
                }
            };

            new ExcelClass().WriteToOneExcel(
                Environment.CurrentDirectory + "/" + Path.ChangeExtension("ff.x", ".xlsx"), dic);

            //Console.ReadKey();

            //foreach (KeyValuePair<string, List<List<object>>> pair in dic)
            //{
            //    new ExcelClass().WriteToExcel(
            //        Environment.CurrentDirectory + "/" + Path.ChangeExtension(pair.Key, ".xlsx"), pair.Value);
            //}
            //Console.ReadKey();

            return;
            

            ReadExcelToJson(new List<string>() {"ff.xlsx", "xx.xlsx", "yy.xlsx"});
            return;*/


            do
            {
                Console.Clear();
                Console.WriteLine("----------命令索引----------");
                foreach (var value in Enum.GetValues(typeof (CaoType)))
                {
                    Console.WriteLine("\t" + (int) value + "：" + value);
                }
                Console.WriteLine("----------------------------");

                var s = SystemExtensions.GetInputStr("请选择，然后回车：");
                if (s == null) continue;

                CaoType caoType = (CaoType) Enum.Parse(typeof (CaoType), s);
                switch (caoType)
                {
                    case CaoType.CompareExcel:
                        new CompareExcel();
                        break;
                    case CaoType.JsonToCsv:
                        ReadJsonToCsv();
                        break;
                    case CaoType.JsonToExcel:
                        ReadJsonToExcel();
                        break;
                    case CaoType.JsonToOneExcel:
                        ReadJsonToOneExcel();
                        break;
                    case CaoType.ExcelToJson:
                        ReadExcelToJson();
                        break;
                    case CaoType.ExcelToOneExcel:
                        ReadExcelToOneExcel();
                        break;
                    default:
                        Console.Write("不存在的命令！");
                        break;
                }
                GC.Collect();
            } while (SystemExtensions.ContinueY());
        }

        #region Json->

        /// <summary>
        /// json->csv
        /// </summary>
        private static void ReadJsonToCsv()
        {
            List<string> files = CheckPath(".json");
            if (files.Count == 0) return;
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                ExcelClass.ConvertDataTableToCsv(ExcelClass.ConvertListToDataTable(ExcelClass.ConvertJsonToListByPath(file)));
            });
        }

        /// <summary>
        /// json->xlsx
        /// </summary>
        private static void ReadJsonToExcel()
        {
            List<string> files = CheckPath(".json");
            if (files.Count == 0) return;
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                List<List<object>> vals = ExcelClass.ConvertJsonToListByPath(file);
                new ExcelClass().WriteToExcel(Path.ChangeExtension(file, ".xls"), vals);
            });
        }

        /// <summary>
        /// / json->xlsx
        /// </summary>
        private static void ReadJsonToOneExcel()
        {
            List<string> files = CheckPath(".json", true);
            if (files.Count == 0) return;

            var dic = new Dictionary<string, List<List<object>>>();
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                dic[file] = ExcelClass.ConvertJsonToListByPath(file);
            });
            new ExcelClass().WriteToOneExcel(InputPath + ".xlsx", dic);
        }

        #endregion

        #region Excel->

        /// <summary>
        /// xlsx->json
        /// </summary>
        private static void ReadExcelToJson()
        {
            List<string> files = CheckPath(".xlsx");
            if (files.Count == 0) return;
            ReadExcelToJson(files);
        }

        private static void ReadExcelToJson(List<string> files)
        {
            ExcelClass excel = new ExcelClass();
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                var vals = excel.ReadFromExcels(file);

                if (vals.Count == 1)
                {
                    ExcelClass.ConvertListToJsonFile(vals.First(), file);
                    return;
                }

                foreach (KeyValuePair<string, List<List<object>>> pair in vals)
                {
                    string newPath = file.Replace(Path.GetExtension(file), "\\" + pair.Key.Replace("$", ""));
                    FileHelper.CreateDirectory(newPath);
                    ExcelClass.ConvertListToJsonFile(pair, newPath);
                }
            });
        }

        /// <summary>
        /// / json->xlsx
        /// </summary>
        private static void ReadExcelToOneExcel()
        {
            List<string> files = CheckPath(".xlsx", true);
            if (files.Count == 0) return;

            var dic = new Dictionary<string, List<List<object>>>();
            files.ForEach(file =>
            {
                var vals = new ExcelClass().ReadFromExcels(file);

                if (vals.Count == 1)
                {
                    dic[file] = ExcelClass.ConvertJsonToList(LitJsonHelper.ToJson(ExcelClass.ConvertListToJson(vals.First())));
                    return;
                }

                foreach (KeyValuePair<string, List<List<object>>> pair in vals)
                {
                    dic[file + "/" + pair.Key] = ExcelClass.ConvertJsonToList(LitJsonHelper.ToJson(ExcelClass.ConvertListToJson(pair)));
                }

            });
            new ExcelClass().WriteToOneExcel(InputPath + ".xlsx", dic);
        }


        #endregion

        private static List<string> CheckPath(string exce, bool isOnlydir = false)
        {
            List<string> files = new List<string>();

            string path = SystemExtensions.GetInputStr(isOnlydir ? "请拖入文件夹：" : "请拖入文件夹或文件：", "您选择的文件夹或文件：");
            if (string.IsNullOrEmpty(path))
                return files;

            if (Directory.Exists(path))
            {
                files = Directory.GetFiles(path).ToList();
            }
            else if (File.Exists(path))
            {
                files.Add(path);
            }
            else
            {
                Console.WriteLine("path is not valid!");
                return files;
            }

            InputPath = path;
            files = files.Where(p => p.EndsWith(exce)).ToList();
            files.Sort();
            return files;
        }
    }
}
