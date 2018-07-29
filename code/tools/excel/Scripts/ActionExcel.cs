using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Library.Excel;
using DataTable = Library.Excel.DataTable;

namespace Script
{
    public class ActionExcel : ActionBase
    {
        public class ToXml
        {
            public ToXml()
            {
                ToXml(".xlsx|.xls", file => ExcelByNpoi.ImportExcelToDataTable(file, false));
            }
        }

        public class ToCsv
        {
            public ToCsv()
            {
                ToCsv(".xlsx|.xls", file => ExcelByNpoi.ImportExcelToDataTable(file, false));
            }
        }

        public class ToJson
        {
            public ToJson()
            {
                ToJson(".xlsx|.xls", file => ExcelByNpoi.ImportExcelToDataTable(file, false));
            }
        }

        [Description("读取多个Excel文件分解每一个文件的sheet到一个Excel文件")]
        public class ToExcel
        {
            public ToExcel()
            {
                ToExcel(".xlsx|.xls", file => ExcelByNpoi.ImportExcelToDataTable(file, false));
            }
        }

        [Description("读取多个Excel文件的多个sheet合并到一个Excel文件")]
        public class ToOneExcel
        {
            public ToOneExcel()
            {
                ToOneExcel(".xlsx|.xls", file => ExcelByNpoi.ImportExcelToDataTable(file, false));
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public class ToKvExcel
        {
            public ToKvExcel()
            {
                ToKvExcel(".xlsx|.xls", file => ExcelByNpoi.ImportExcelToDataTable(file, false).FirstOrDefault());
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public class KvExcelTo
        {
            public KvExcelTo()
            {
                KvExcelTo((file) => ExcelByNpoi.ImportExcelToDataTable(file, false).FirstOrDefault(),
                    (dt, file) => ExcelByNpoi.ExportDataTableToExcel(file, dt));
            }
        }
    }
}