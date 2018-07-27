using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Excel;

namespace Script
{
    public class ActionXml : ActionBase
    {
        public class ToCsv 
        {
            public ToCsv()
            {
                ToCsv(".xml", file => new List<DataTable>() {ExcelByBase.ConvertXmlToDataTable(file)});
            }
        }

        public class ToJson
        {
            public ToJson()
            {
                ToJson(".xml", file => new List<DataTable>() { ExcelByBase.ConvertXmlToDataTable(file) });
            }
        }

        public class ToExcel
        {
            public ToExcel()
            {
                ToExcel(".xml", file => new List<DataTable>() { ExcelByBase.ConvertXmlToDataTable(file) });
            }
        }

        public class ToOneExcel
        {
            public ToOneExcel()
            {
                ToOneExcel(".xml", file => new List<DataTable>() { ExcelByBase.ConvertXmlToDataTable(file) });
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public class ToKvExcel
        {
            public ToKvExcel()
            {
                ArrayToKvExcel(".xml", ExcelByBase.ConvertXmlToDataTable);
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public class KvExcelTo
        {
            public KvExcelTo()
            {
                ArrayKvExcelTo(ExcelByBase.ConvertDataTableToXml);
            }
        }
    }
}