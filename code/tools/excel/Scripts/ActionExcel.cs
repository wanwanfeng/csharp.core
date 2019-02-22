using System;
using System.Collections.Generic;
using System.ComponentModel;
using Library.Excel;
using Library.Extensions;

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
            get
            {
                if (firstIsKey)
                {
                    //自定义的输出（首行为key）
                    return (file) => ExcelByBase.Data.ImportToDataTable(file, false);
                }
                //未经过滤原样输出
                return (file) => ExcelByBase.Data.ImportToDataTable(file);
            }
        }

        public override Action<DataTable, string> export
        {
            get { return ExcelByBase.Data.ExportToExcel; }
        }

        public ActionExcel()
        {
            firstIsKey = SystemConsole.GetInputStr("Excel文件导入时首行作为Key还是内容？(true:false)", def: "true").AsBool(true);
        }

        public class ToXml : ActionExcel
        {
            public ToXml()
            {
                ToXml();
            }
        }

        public class ToCsv : ActionExcel
        {
            public ToCsv()
            {
                ToCsv();
            }
        }

        public class ToJson : ActionExcel
        {
            public ToJson()
            {
                ToJson();
            }
        }

        [Description("读取多个Excel文件分解每一个文件的sheet到一个Excel文件")]
        public class ToExcel : ActionExcel
        {
            public ToExcel()
            {
                ToExcel();
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