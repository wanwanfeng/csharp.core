using System;
using System.Collections.Generic;
using Library.Excel;

namespace Script
{
    public class ActionXml : ActionBase
    {
        public override string selectExtension
        {
            get { return ".xml"; }
        }

        public override Func<string, IEnumerable<DataTable>> import
        {
            get { return file => new[] {ExcelByBase.Xml.ImportToDataTable(file)}; }
        }

        public override Action<DataTable, string> export
        {
            get { return ExcelByBase.Data.ExportToXml; }
        }

        public class ToCsv : ActionXml
        {
            public ToCsv()
            {
                ToCommon();
            }
        }

        public class ToJson : ActionXml
        {
            public ToJson()
            {
                ToCommon();
            }
        }

        public class ToExcel : ActionXml
        {
            public ToExcel()
            {
                ToCommon();
            }
        }

        public class ToOneExcel : ActionXml
        {
            public ToOneExcel()
            {
                ToOneExcel();
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public class ToKvExcel : ActionXml
        {
            public ToKvExcel()
            {
                ToKvExcelAll();
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public class KvExcelTo : ActionXml
        {
            public KvExcelTo()
            {
                KvExcelTo();
            }
        }
    }
}