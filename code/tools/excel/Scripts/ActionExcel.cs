using System.ComponentModel;
using System.Linq;
using Library.Excel;

namespace Script
{
    public class ActionExcel : ActionBase
    {
        public class ToXml
        {
            public ToXml()
            {
                ToXml(".xlsx|.xls", ExcelByBase.Data.ImportToDataTable);
            }
        }

        public class ToCsv
        {
            public ToCsv()
            {
                ToCsv(".xlsx|.xls", ExcelByBase.Data.ImportToDataTable);
            }
        }

        public class ToJson
        {
            public ToJson()
            {
                ToJson(".xlsx|.xls", ExcelByBase.Data.ImportToDataTable);
            }
        }

        [Description("读取多个Excel文件分解每一个文件的sheet到一个Excel文件")]
        public class ToExcel
        {
            public ToExcel()
            {
                ToExcel(".xlsx|.xls", ExcelByBase.Data.ImportToDataTable);
            }
        }

        [Description("读取多个Excel文件的多个sheet合并到一个Excel文件")]
        public class ToOneExcel
        {
            public ToOneExcel()
            {
                ToOneExcel(".xlsx|.xls", ExcelByBase.Data.ImportToDataTable);
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public class ToKvExcel
        {
            public ToKvExcel()
            {
                ToKvExcel(".xlsx|.xls", ExcelByBase.Data.ImportToDataTable);
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public class KvExcelTo
        {
            public KvExcelTo()
            {
                KvExcelToFromDataTable(
                    loadAction: (file) => ExcelByBase.Data.ImportToDataTable(file).FirstOrDefault(),
                    saveAction: ExcelByBase.Data.ExportToExcel
                    );
            }
        }
    }
}