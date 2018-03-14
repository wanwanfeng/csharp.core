using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Helper;
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
        JsonToCsv = 1,
        JsonToExcel = 2,
        JsonToOneExcel = 3,
        ExcelToJson = 4,
    }


    class Program
    {
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
            Console.WriteLine("----------命令索引----------");
            foreach (var value in Enum.GetValues(typeof(CaoType)))
            {
                Console.WriteLine("\t" + (int) value + "：" + value);
            }
            Console.WriteLine("----------------------------");
            Console.Write("请选择，然后回车：");
            string s = Console.ReadLine();
            if (s != null)
            {
                CaoType caoType = (CaoType) Enum.Parse(typeof (CaoType), s);
                switch (caoType)
                {
                    case CaoType.JsonToCsv:
                        ReadjsonToCsv();
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
                    default:
                        Console.Write("不存在的命令！");
                        break;
                }
            }
            GC.Collect();
            Console.Write("请按任意键退出...");
            Console.ReadKey();
        }

        #region Json->

        /// <summary>
        /// json->csv
        /// </summary>
        private static void ReadjsonToCsv()
        {
            List<string> files = CheckPath();
            if (files.Count == 0) return;
            foreach (var file in files)
            {
                Console.WriteLine(" is now : " + file);
                ExcelClass.ConvertDataTableToCsv(ExcelClass.ConvertListToDataTable(ExcelClass.ConvertJsonToListByPath(file)));
            }
        }

        /// <summary>
        /// json->xlsx
        /// </summary>
        private static void ReadJsonToExcel()
        {
            List<string> files = CheckPath();
            if (files.Count == 0) return;
            foreach (string file in files)
            {
                Console.WriteLine(" is now : " + file);
                List<List<object>> vals = ExcelClass.ConvertJsonToListByPath(file);
                new ExcelClass().WriteToExcel(Path.ChangeExtension(file, ".xls"), vals);
            }
        }

        /// <summary>
        /// / json->xlsx
        /// </summary>
        private static void ReadJsonToOneExcel()
        {
            List<string> files = CheckPath();
            if (files.Count == 0) return;

            var outName = "OneExcel";
            Console.WriteLine("----------------------------");
            Console.Write("请输入输出文件名，然后回车：");
            string s = Console.ReadLine();
            if (string.IsNullOrEmpty(s))
            {
                s = outName;
            }
            var dic = new Dictionary<string, List<List<object>>>();
            foreach (string file in files)
            {
                Console.WriteLine(" is now : " + file);
                dic[file] = ExcelClass.ConvertJsonToListByPath(file);
            }
            new ExcelClass().WriteToOneExcel(s + ".xlsx", dic);
        }

        #endregion

        #region Excel->

        /// <summary>
        /// xlsx->json
        /// </summary>
        private static void ReadExcelToJson()
        {
            List<string> files = CheckPath();
            if (files.Count == 0) return;
            ReadExcelToJson(files);
        }

        private static void ReadExcelToJson(List<string> files)
        {
            foreach (var file in files)
            {
                Console.WriteLine(" is now : " + file);
                var vals = new ExcelClass().ReadFromExcels(Path.ChangeExtension(file, ".xlsx"));
                if (vals.Count == 1)
                {
                    ExcelClass.ConvertListToJson(vals.First(), file);
                }
                else
                {
                    foreach (KeyValuePair<string, List<List<object>>> pair in vals)
                    {
                        if (file == null) continue;
                        string newPath = file.Replace(Path.GetExtension(file), "\\" + pair.Key);
                        FileHelper.CreateDirectory(newPath);
                        ExcelClass.ConvertListToJson(pair, newPath);
                    }
                }
            }
        }
        #endregion

        private static List<string> CheckPath(string dir = "/json/", string exce = ".json")
        {
            //OpenFileDialog fd = new OpenFileDialog();
            //fd.Filter = "EXCEL文件(*.xls)|*.xls|EXCEL文件(*.xlsx)|*.xlsx";

            //if (fd.ShowDialog() == DialogResult.OK)
            //{
            //    //这里面就可以对选择的文件进行处理了
            //}
            List<string> files = new List<string>();
            string path = (Environment.CurrentDirectory + dir).Replace("\\", "/");
            if (!Directory.Exists(path))
            {
                Console.WriteLine(path + " is not exists !");
                return files;
            }
            files = Directory.GetFiles(path).Where(p => p.EndsWith(exce)).ToList();
            files.Sort();
            return files;
        }
    }
}
