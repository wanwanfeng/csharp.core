using System;
using System.ComponentModel;
using Library.Extensions;
using Script;

namespace Library.Excel
{
    public enum CaoType
    {
        [TypeValue(typeof(CompareExcel))] CompareExcel,

        [Description("Excel->ToXml")] [TypeValue(typeof(ActionExcel.ToXml))] ExcelToXml,
        [Description("Excel->ToCsv")] [TypeValue(typeof(ActionExcel.ToCsv))] ExcelToCsv,
        [Description("Excel->ToJson")] [TypeValue(typeof(ActionExcel.ToJson))] ExcelToJson,
        [Description("Excel->ToExcel")] [TypeValue(typeof(ActionExcel.ToExcel))] ExcelToExcel,
        [Description("Excel->ToOneExcel")] [TypeValue(typeof(ActionExcel.ToOneExcel))] ExcelToOneExcel,
        [Description("Excel->ToKvExcel")] [TypeValue(typeof(ActionExcel.ToKvExcel))] ExcelToKvExcel,
        [Description("Excel->KvExcelTo")] [TypeValue(typeof(ActionExcel.KvExcelTo))] ExcelKvExcelTo,

        [Description("Json->ToXml")] [TypeValue(typeof(ActionJson.ToXml))] JsonToXml,
        [Description("Json->ToCsv")] [TypeValue(typeof(ActionJson.ToCsv))] JsonToCsv,
        [Description("Json->ToExcel")] [TypeValue(typeof(ActionJson.ToExcel))] JsonToExcel,
        [Description("Json->ToOneExcel")] [TypeValue(typeof(ActionJson.ToOneExcel))] JsonToOneExcel,
        [Description("Json->ToKvExcel")] [TypeValue(typeof(ActionJson.ToKvExcel))] JsonToKvExcel,
        [Description("Json->KvExcelTo")] [TypeValue(typeof(ActionJson.KvExcelTo))] JsonKvExcelTo,

        [Description("Xml->ToCsv")] [TypeValue(typeof(ActionXml.ToCsv))] XmlToCsv,
        [Description("Xml->ToJson")] [TypeValue(typeof(ActionXml.ToJson))] XmlToJson,
        [Description("Xml->ToExcel")] [TypeValue(typeof(ActionXml.ToExcel))] XmlToExcel,
        [Description("Xml->ToOneExcel")] [TypeValue(typeof(ActionXml.ToOneExcel))] XmlToOneExcel,
        [Description("Xml->ToKvExcel")] [TypeValue(typeof(ActionXml.ToKvExcel))] XmlToKvExcel,
        [Description("Xml->KvExcelTo")] [TypeValue(typeof(ActionXml.KvExcelTo))] XmlKvExcelTo,

        [Description("CSV->ToXml")] [TypeValue(typeof(ActionCSV.ToXml))] CsvToXml,
        [Description("CSV->ToJson")] [TypeValue(typeof(ActionCSV.ToJson))] CsvToJson,
        [Description("CSV->ToExcel")] [TypeValue(typeof(ActionCSV.ToExcel))] CsvToExcel,
        [Description("CSV->ToOneExcel")] [TypeValue(typeof(ActionCSV.ToOneExcel))] CsvToOneExcel,
        [Description("CSV->ToKvExcel")] [TypeValue(typeof(ActionCSV.ToKvExcel))] CsvToKvExcel,
        [Description("CSV->KvExcelTo")] [TypeValue(typeof(ActionCSV.KvExcelTo))] CsvKvExcelTo,
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
            SystemConsole.Run<CaoType>(null, 3);
        }
    }
}
