using System.Collections.Generic;
using DataTable = Library.Excel.DataTable;
using Library.Excel;

namespace Script
{
    public class ActionJson : ActionBase
    {
        public class ToXml
        {
            public ToXml()
            {
                ToXml(".json", file => new List<DataTable>() {ExcelByBase.ImportJsonToDataTable(file)});
            }
        }

        public class ToCsv
        {
            public ToCsv()
            {
                ToCsv(".json", file => new List<DataTable>() {ExcelByBase.ImportJsonToDataTable(file)});
            }
        }

        public class ToExcel
        {
            public ToExcel()
            {
                ToExcel(".json", file => new List<DataTable>() {ExcelByBase.ImportJsonToDataTable(file)});
            }
        }

        public class ToOneExcel
        {
            public ToOneExcel()
            {
                ToOneExcel(".json", file => new List<DataTable>() {ExcelByBase.ImportJsonToDataTable(file)});
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public class ToKvExcel
        {
            public ToKvExcel()
            {
                ToKvExcel(".json", ExcelByBase.ImportJsonToDataTable);
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public class KvExcelTo
        {
            public KvExcelTo()
            {
                KvExcelTo(ExcelByBase.ImportJsonToDataTable, ExcelByBase.ExportDataTableToJson);
            }
        }
    }
}