using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Library.Excel;
using Library.Helper;
using Library.LitJson;

namespace Script
{
    public class ActionExcel : ActionBase
    {
        public class ToXml
        {
            public ToXml()
            {
                ToXml(".xlsx|.xls", file => new List<DataTable>(ExcelByNpoi.ExcelToDataTable(file, false)));
            }
        }

        public class ToCsv
        {
            public ToCsv()
            {
                ToCsv(".xlsx|.xls", file => new List<DataTable>(ExcelByNpoi.ExcelToDataTable(file, false)));
            }
        }

        public class ToJson
        {
            public ToJson()
            {
                ToJson(".xlsx|.xls", file => new List<DataTable>(ExcelByNpoi.ExcelToDataTable(file, false)));
            }
        }

        [Description("读取多个Excel文件分解每一个文件的sheet到一个Excel文件")]
        public class ToExcel
        {
            public ToExcel()
            {
                ToExcel(".xlsx|.xls", file => new List<DataTable>(ExcelByNpoi.ExcelToDataTable(file, false)));
            }
        }

        [Description("读取多个Excel文件的多个sheet合并到一个Excel文件")]
        public class ToOneExcel
        {
            public ToOneExcel()
            {
                ToOneExcel(".xlsx|.xls", file => new List<DataTable>(ExcelByNpoi.ExcelToDataTable(file, false)));
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public class ToKvExcel
        {
            public ToKvExcel()
            {
                ToKvExcel(".xlsx|.xls", (file, b) => ExcelByNpoi.ExcelToDataTable(file, false).FirstOrDefault());
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public class KvExcelTo
        {
            public KvExcelTo()
            {
                KvExcelTo((dt, file) => ExcelByNpoi.DataTableToExcel(file, dt));
            }
        }
    }
}