﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Excel;

namespace Script
{
    public class ActionJson : ActionBase
    {
        public class ToXml
        {
            public ToXml()
            {
                ToXml(".json", file => new List<DataTable>() { ExcelByBase.ConvertJsonToDataTableByPath(file) });
            }
        }

        public class ToCsv 
        {
            public ToCsv()
            {
                ToCsv(".json", file => new List<DataTable>() {ExcelByBase.ConvertJsonToDataTableByPath(file)});
            }
        }

        public class ToExcel
        {
            public ToExcel()
            {
                ToExcel(".json", file => new List<DataTable>() { ExcelByBase.ConvertJsonToDataTableByPath(file) });
            }
        }

        public class ToOneExcel
        {
            public ToOneExcel()
            {
                ToOneExcel(".json", file => new List<DataTable>() {ExcelByBase.ConvertJsonToDataTableByPath(file)});
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public class ToKvExcel
        {
            public ToKvExcel()
            {
                ToKvExcel(".json", ExcelByBase.ConvertJsonToDataTableByPath);
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public class KvExcelTo
        {
            public KvExcelTo()
            {
                KvExcelTo(ExcelByBase.ConvertDataTableToJsonByPath);
            }
        }
    }
}