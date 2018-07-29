using System;
using System.Collections.Generic;
using DataTable = Library.Excel.DataTable;
using Library.Excel;

namespace Script
{
    public class ActionXml : ActionBase
    {
        public class ToCsv 
        {
            public ToCsv()
            {
                ToCsv(".xml", file => new List<DataTable>() {ExcelByBase.ImportXmlToDataTable(file)});
            }
        }

        public class ToJson
        {
            public ToJson()
            {
                ToJson(".xml", file => new List<DataTable>() { ExcelByBase.ImportXmlToDataTable(file) });
            }
        }

        public class ToExcel
        {
            public ToExcel()
            {
                ToExcel(".xml", file => new List<DataTable>() { ExcelByBase.ImportXmlToDataTable(file) });
            }
        }

        public class ToOneExcel
        {
            public ToOneExcel()
            {
                ToOneExcel(".xml", file => new List<DataTable>() { ExcelByBase.ImportXmlToDataTable(file) });
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public class ToKvExcel
        {
            public ToKvExcel()
            {
                ToKvExcel(".xml", ExcelByBase.ImportXmlToDataTable);
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public class KvExcelTo
        {
            public KvExcelTo()
            {
                KvExcelTo(ExcelByBase.ImportXmlToDataTable, ExcelByBase.ExportDataTableToXml);
            }
        }
    }
}