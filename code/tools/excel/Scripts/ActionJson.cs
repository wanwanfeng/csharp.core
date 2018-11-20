using System.Collections.Generic;
using System.IO;
using DataTable = Library.Excel.DataTable;
using Library.Excel;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace Script
{
    public class ActionJson : ActionBase
    {
        public class ToXml
        {
            public ToXml()
            {
                ToXml(".json", file => new List<DataTable>() {ExcelByBase.Json.ImportToDataTable(file)});
            }
        }

        public class ToCsv
        {
            public ToCsv()
            {
                ToCsv(".json", file => new List<DataTable>() {ExcelByBase.Json.ImportToDataTable(file)});
            }
        }

        public class ToExcel
        {
            public ToExcel()
            {
                ToExcel(".json", file => new List<DataTable>() {ExcelByBase.Json.ImportToDataTable(file)});
            }
        }

        public class ToOneExcel
        {
            public ToOneExcel()
            {
                ToOneExcel(".json", file => new List<DataTable>() {ExcelByBase.Json.ImportToDataTable(file)});
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public class ToKvExcel
        {
            public ToKvExcel()
            {
                ToKvExcelAll(".json", file => new List<DataTable>() {ExcelByBase.Json.ImportToDataTable(file)});
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public class KvExcelTo
        {
            public KvExcelTo()
            {
                var isIndent = SystemConsole.GetInputStr("json文件是否进行格式化？(true:false)").AsBool(true);
                KvExcelToFromListTable(
                    loadAction: ExcelByBase.Json.ImportToListTable,
                    saveAction: (table, s) => { ExcelByBase.Data.ExportToJson(table, s, isIndent); },
                    isCustomAction: (fullpath, list) =>
                    {
                        Dictionary<string, JsonData> dictionary = new Dictionary<string, JsonData>();
                        foreach (List<object> objects in list)
                        {
                            object id = objects[1];
                            string key = objects[2].ToString();
                            object value = objects[3];
                            string value_zh_cn = objects[4].ToString();

                            dictionary[id.ToString()] = value_zh_cn;
                        }

                        JsonData jsonData = JsonHelper.ToObject(File.ReadAllText(fullpath).Trim().Trim('\0'));
                        JsonHelper.RevertDictionaryToJson(jsonData, dictionary);
                        return isIndent ? JsonHelper.ToJson(jsonData, indentLevel: 2) : JsonHelper.ToJson(jsonData);
                    }
                    );
            }
        }
    }
}