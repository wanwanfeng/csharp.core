using System;
using System.ComponentModel;
using Library.Extensions;
using Script;

namespace Library.Excel
{
    public enum CaoType
    {
        [Category("两个文件比较")] [TypeValue(typeof (CompareExcel))] CompareExcel,
        [Category("两个文件比较")] [TypeValue(typeof (CompareJson))] CompareJson,


        [Category("文件转换")] [Description("Excel->ToXml")] [TypeValue(typeof (ActionExcel.ToXml))] ExcelToXml,
        [Category("文件转换")] [Description("Excel->ToCsv")] [TypeValue(typeof (ActionExcel.ToCsv))] ExcelToCsv,
        [Category("文件转换")] [Description("Excel->ToJson")] [TypeValue(typeof (ActionExcel.ToJson))] ExcelToJson,
        [Category("文件转换")] [Description("Excel->ToExcel")] [TypeValue(typeof (ActionExcel.ToExcel))] ExcelToExcel,
        [Category("文件转换")] [Description("Excel->ToOneExcel")] [TypeValue(typeof (ActionExcel.ToOneExcel))] ExcelToOneExcel,


        [Category("文件转换")] [Description("Json->ToXml")] [TypeValue(typeof (ActionJson.ToXml))] JsonToXml,
        [Category("文件转换")] [Description("Json->ToCsv")] [TypeValue(typeof (ActionJson.ToCsv))] JsonToCsv,
        [Category("文件转换")] [Description("Json->ToExcel")] [TypeValue(typeof (ActionJson.ToExcel))] JsonToExcel,
        [Category("文件转换")] [Description("Json->ToOneExcel")] [TypeValue(typeof (ActionJson.ToOneExcel))] JsonToOneExcel,

        [Category("文件转换")] [Description("Xml->ToCsv")] [TypeValue(typeof (ActionXml.ToCsv))] XmlToCsv,
        [Category("文件转换")] [Description("Xml->ToJson")] [TypeValue(typeof (ActionXml.ToJson))] XmlToJson,
        [Category("文件转换")] [Description("Xml->ToExcel")] [TypeValue(typeof (ActionXml.ToExcel))] XmlToExcel,
        [Category("文件转换")] [Description("Xml->ToOneExcel")] [TypeValue(typeof (ActionXml.ToOneExcel))] XmlToOneExcel,


        [Category("文件转换")] [Description("CSV->ToXml")] [TypeValue(typeof (ActionCSV.ToXml))] CsvToXml,
        [Category("文件转换")] [Description("CSV->ToJson")] [TypeValue(typeof (ActionCSV.ToJson))] CsvToJson,
        [Category("文件转换")] [Description("CSV->ToExcel")] [TypeValue(typeof (ActionCSV.ToExcel))] CsvToExcel,
        [Category("文件转换")] [Description("CSV->ToOneExcel")] [TypeValue(typeof (ActionCSV.ToOneExcel))] CsvToOneExcel,

        [Category("文件内容提取与替换")] [Description("Excel->ToKvExcel")] [TypeValue(typeof (ActionExcel.ToKvExcel))] ExcelToKvExcel,
        [Category("文件内容提取与替换")] [Description("Excel->FromKvExcel")] [TypeValue(typeof (ActionExcel.KvExcelTo))] ExcelFromKvExcel,
        [Category("文件内容提取与替换")] [Description("Json->ToKvExcel")] [TypeValue(typeof (ActionJson.ToKvExcel))] JsonToKvExcel,
        [Category("文件内容提取与替换")] [Description("Json->FromKvExcel")] [TypeValue(typeof (ActionJson.KvExcelTo))] JsonFromKvExcel,
        [Category("文件内容提取与替换")] [Description("Xml->ToKvExcel")] [TypeValue(typeof (ActionXml.ToKvExcel))] XmlToKvExcel,
        [Category("文件内容提取与替换")] [Description("Xml->FromKvExcel")] [TypeValue(typeof (ActionXml.KvExcelTo))] XmlFromKvExcel,
        [Category("文件内容提取与替换")] [Description("CSV->ToKvExcel")] [TypeValue(typeof (ActionCSV.ToKvExcel))] CsvToKvExcel,
        [Category("文件内容提取与替换")] [Description("CSV->FromKvExcel")] [TypeValue(typeof (ActionCSV.KvExcelTo))] CsvFromKvExcel
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
            SystemConsole.Run<CaoType>(columnsCount: 4);
        }
    }
}
