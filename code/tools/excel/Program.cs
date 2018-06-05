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
        [TypeValue(typeof (CompareExcel))] CompareExcel = 0,
        [TypeValue(typeof (ActionJson.ToXml))] JsonToXml,
        [TypeValue(typeof (ActionJson.ToCsv))] JsonToCsv,
        [TypeValue(typeof (ActionJson.ToExcel))] JsonToExcel,
        [TypeValue(typeof (ActionJson.ToOneExcel))] JsonToOneExcel,
        [TypeValue(typeof (ActionJson.ToKvExcel))] JsonToKvExcel,
        [TypeValue(typeof (ActionJson.KvExcelTo))] JsonKvExcelTo,

        [TypeValue(typeof (ActionExcel.ToXml))] ExcelToXml,
        [TypeValue(typeof (ActionExcel.ToCsv))] ExcelToCsv,
        [TypeValue(typeof (ActionExcel.ToJson))] ExcelToJson,
        [TypeValue(typeof (ActionExcel.ToExcel))] ExcelToExcel,
        [TypeValue(typeof (ActionExcel.ToOneExcel))] ExcelToOneExcel,

        [TypeValue(typeof (ActionXml.ToCsv))] XmlToCsv,
        [TypeValue(typeof (ActionXml.ToJson))] XmlToJson,
        [TypeValue(typeof (ActionXml.ToExcel))] XmlToExcel,
        [TypeValue(typeof (ActionXml.ToOneExcel))] XmlToOneExcel,
        [TypeValue(typeof (ActionXml.ToKvExcel))] XmlToKvExcel,
        [TypeValue(typeof (ActionXml.KvExcelTo))] XmlKvExcelTo,

        [TypeValue(typeof (ActionCSV.ToXml))] CsvToXml,
        [TypeValue(typeof (ActionCSV.ToJson))] CsvToJson,
        [TypeValue(typeof (ActionCSV.ToExcel))] CsvToExcel,
        [TypeValue(typeof (ActionCSV.ToOneExcel))] CsvToOneExcel,
        [TypeValue(typeof (ActionCSV.ToKvExcel))] CsvToKvExcel,
        [TypeValue(typeof (ActionCSV.KvExcelTo))] CsvKvExcelTo,
    }


    internal class Program
    {
        static Program()
        {
            Ldebug.OnActionLog += Console.WriteLine;
            Ldebug.OnActionLogError += Console.WriteLine;
        }

        private static void Main(string[] args)
        {
            SystemConsole.Run<CaoType>();
        }
    }
}
