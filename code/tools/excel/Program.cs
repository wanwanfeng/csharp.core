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
        [TypeValue(typeof (ActionJson.ToCsv))] JsonToCsv,
        [TypeValue(typeof (ActionJson.ToExcel))] JsonToExcel,
        [TypeValue(typeof (ActionJson.ToOneExcel))] JsonToOneExcel,

        [TypeValue(typeof (ActionExcel.ToCsv))] ExcelToCsv,
        [TypeValue(typeof (ActionExcel.ToJson))] ExcelToJson,
        [TypeValue(typeof (ActionExcel.ToOneExcel))] ExcelToOneExcel,

        [TypeValue(typeof (ActionCSV.ToJson))] CsvToJson,
        [TypeValue(typeof (ActionCSV.ToExcel))] CsvToExcel,
        [TypeValue(typeof (ActionCSV.ToOneExcel))] CsvToOneExcel,
        [TypeValue(typeof (ActionCSV.ToKvExcel))] CsvToKvExcel,
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

                //try
                //{
                var cache = AttributeHelper.GetCacheTypeValue<CaoType>();
                CaoType caoType = (CaoType) SystemExtensions.GetInputStr("请选择，然后回车：").AsInt();
                //new ActionCSV.ToKvExcel();
                ActionBase actionBase = (ActionBase) Activator.CreateInstance(cache[caoType]);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.Message);
                //}

                GC.Collect();
            } while (SystemExtensions.ContinueY());
        }
    }
}
