using System;
using System.ComponentModel;
using Library.Extensions;
using Script;

namespace Library.Excel
{

    public class Program
    {
		public enum CaoType
		{
			[Category("两个文件比较"), TypeValue(typeof(CompareExcel))] CompareExcel,
			[Category("两个文件比较"), TypeValue(typeof(CompareJson))] CompareJson,


			[Category("Excel文件转换"), Description("Excel->Xml"), TypeValue(typeof(ActionExcel.ToXml))] ExcelToXml,
			[Category("Excel文件转换"), Description("Excel->Csv"), TypeValue(typeof(ActionExcel.ToCsv))] ExcelToCsv,
			[Category("Excel文件转换"), Description("Excel->Json"), TypeValue(typeof(ActionExcel.ToJson))] ExcelToJson,
			[Category("Excel文件转换"), Description("Excel->Excel"), TypeValue(typeof(ActionExcel.ToExcel))] ExcelToExcel,
			[Category("Excel文件转换"), Description("Excel->OneExcel"), TypeValue(typeof(ActionExcel.ToOneExcel))] ExcelToOneExcel,


			[Category("Json文件转换"), Description("Json->Xml"), TypeValue(typeof(ActionJson.ToXml))] JsonToXml,
			[Category("Json文件转换"), Description("Json->Csv"), TypeValue(typeof(ActionJson.ToCsv))] JsonToCsv,
			[Category("Json文件转换"), Description("Json->Excel"), TypeValue(typeof(ActionJson.ToExcel))] JsonToExcel,
			[Category("Json文件转换"), Description("Json->OneExcel"), TypeValue(typeof(ActionJson.ToOneExcel))] JsonToOneExcel,

			[Category("Xml文件转换"), Description("Xml->Csv"), TypeValue(typeof(ActionXml.ToCsv))] XmlToCsv,
			[Category("Xml文件转换"), Description("Xml->Json"), TypeValue(typeof(ActionXml.ToJson))] XmlToJson,
			[Category("Xml文件转换"), Description("Xml->Excel"), TypeValue(typeof(ActionXml.ToExcel))] XmlToExcel,
			[Category("Xml文件转换"), Description("Xml->OneExcel"), TypeValue(typeof(ActionXml.ToOneExcel))] XmlToOneExcel,


			[Category("CSV文件转换"), Description("CSV->Xml"), TypeValue(typeof(ActionCSV.ToXml))] CsvToXml,
			[Category("CSV文件转换"), Description("CSV->Json"), TypeValue(typeof(ActionCSV.ToJson))] CsvToJson,
			[Category("CSV文件转换"), Description("CSV->Excel"), TypeValue(typeof(ActionCSV.ToExcel))] CsvToExcel,
			[Category("CSV文件转换"), Description("CSV->OneExcel"), TypeValue(typeof(ActionCSV.ToOneExcel))] CsvToOneExcel,

			[Category("文件内容提取与替换"), Description("Excel->KvExcel"), TypeValue(typeof(ActionExcel.ToKvExcel))] ExcelToKvExcel,
			[Category("文件内容提取与替换"), Description("Excel->FromKvExcel"), TypeValue(typeof(ActionExcel.KvExcelTo))] ExcelFromKvExcel,

            [Category("文件内容提取与替换【内容为数组】"), Description("Json ->KvExcel"), TypeValue(typeof(ActionJson.ToKvExcel), true)] JsonToKvExcel,
            [Category("文件内容提取与替换【内容为数组】"), Description("Json ->FromKvExcel"), TypeValue(typeof(ActionJson.KvExcelTo), true)] JsonFromKvExcel,
            [Category("文件内容提取与替换【内容为数组】"), Description("Xml  ->KvExcel"), TypeValue(typeof(ActionXml.ToKvExcel), true)] XmlToKvExcel,
            [Category("文件内容提取与替换【内容为数组】"), Description("Xml  ->FromKvExcel"), TypeValue(typeof(ActionXml.KvExcelTo), true)] XmlFromKvExcel,
            [Category("文件内容提取与替换【内容为数组】"), Description("CSV  ->KvExcel"), TypeValue(typeof(ActionCSV.ToKvExcel), true)] CsvToKvExcel,
            [Category("文件内容提取与替换【内容为数组】"), Description("CSV  ->FromKvExcel"), TypeValue(typeof(ActionCSV.KvExcelTo), true)] CsvFromKvExcel,
        }

		public enum CaoType2
		{
            [Category("文件内容提取与替换【内容为对象】"), Description("Json ->KvExcel"), TypeValue(typeof(ActionJson.ToKvExcel), false)]JsonToKvExcel,
            [Category("文件内容提取与替换【内容为对象】"), Description("Json ->FromKvExcel"), TypeValue(typeof(ActionJson.KvExcelTo), false)]JsonFromKvExcel,
            [Category("文件内容提取与替换【内容为对象】"), Description("Xml  ->KvExcel"), TypeValue(typeof(ActionXml.ToKvExcel), false)]XmlToKvExcel,
            [Category("文件内容提取与替换【内容为对象】"), Description("Xml  ->FromKvExcel"), TypeValue(typeof(ActionXml.KvExcelTo), false)]XmlFromKvExcel,
            [Category("文件内容提取与替换【内容为对象】"), Description("CSV  ->KvExcel"), TypeValue(typeof(ActionCSV.ToKvExcel), false)]CsvToKvExcel,
            [Category("文件内容提取与替换【内容为对象】"), Description("CSV  ->FromKvExcel"), TypeValue(typeof(ActionCSV.KvExcelTo), false)]CsvFromKvExcel,
        }

        private static void Main(string[] args)
        {
			SystemConsole.Run<CaoType, CaoType2, Json.CaoType>(columnsCount: 4);
        }

        internal class Json
        {
            public enum CaoType
            {
                [Category("Json"), Description("有效性检测"), TypeValue(typeof(CheckJson))] CheckJson,
                [Category("Json"), Description("格式缩进"), TypeValue(typeof(IndentJson))] IndentJson,
                [Category("Json"), Description("格式取消缩进"), TypeValue(typeof(CancelIndentJson))] CancelIndentJson,
                [Category("Json"), Description("读取值并覆盖Excel中值"), TypeValue(typeof(ActionExcel.FixExcel))] FixExcel,
            }

            private static void Main(string[] args)
            {
                SystemConsole.Run<CaoType>(columnsCount: 4);
            }
        }

        internal class Json2Excel
        {
            public enum CaoType
            {
                [Category("文件转换")] [TypeValue(typeof(ActionExcel.ToJson))] Excel2Json,
                [Category("文件转换")] [TypeValue(typeof(ActionJson.ToExcel))] Json2Excel
            }

            private static void Main(string[] args)
            {
                SystemConsole.Run<CaoType>();
            }
        }

        internal class Csv2Excel
        {
            public enum CaoType
            {
                [Category("文件转换")] [TypeValue(typeof(ActionExcel.ToCsv))] Excel2CSV,
                [Category("文件转换")] [TypeValue(typeof(ActionCSV.ToExcel))] CSV2Excel
            }

            private static void Main(string[] args)
            {
                SystemConsole.Run<CaoType>();
            }
        }

        internal class Xml2Excel
        {
            public enum CaoType
            {
                [Category("文件转换")] [TypeValue(typeof(ActionExcel.ToXml))] Excel2Xml,
                [Category("文件转换")] [TypeValue(typeof(ActionXml.ToExcel))] Xml2Excel
            }

            private static void Main(string[] args)
            {
                SystemConsole.Run<CaoType>();
            }
        }
    }
}
