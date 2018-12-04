using System;
using System.Collections.Generic;
using System.ComponentModel;
using Library.Excel;

namespace Script
{
    public class ActionExcel : ActionBase
    {
        public override string selectExtension
        {
            get { return ".xlsx|.xls"; }
        }

        public override Func<string, IEnumerable<DataTable>> import
        {
            get { return ExcelByBase.Data.ImportToDataTable; }
        }

        public override Action<DataTable, string> export
        {
            get { return ExcelByBase.Data.ExportToExcel; }
        }

        public class ToXml : ActionExcel
        {
            public ToXml()
            {
                ToCommon();
            }
        }

        public class ToCsv : ActionExcel
        {
            public ToCsv()
            {
                ToCommon();
            }
        }

        public class ToJson : ActionExcel
        {
            public ToJson()
            {
                ToCommon();
            }
        }

        [Description("读取多个Excel文件分解每一个文件的sheet到一个Excel文件")]
        public class ToExcel : ActionExcel
        {
            public ToExcel()
            {
                ToCommon();
            }
        }

        [Description("读取多个Excel文件的多个sheet合并到一个Excel文件")]
        public class ToOneExcel : ActionExcel
        {
            public ToOneExcel()
            {
                ToOneExcel();
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public class ToKvExcel : ActionExcel
        {
            public ToKvExcel()
            {
                ToKvExcelAll();
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public class KvExcelTo : ActionExcel
        {
            public KvExcelTo()
            {
                KvExcelTo();
            }
        }
    }
}