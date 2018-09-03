using System.Collections.Generic;
using System.IO;
using DataTable = Library.Excel.DataTable;
using Library.Excel;
using Library.LitJson;
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
                ToKvExcel(".json", file => new List<DataTable>() {ExcelByBase.Json.ImportToDataTable(file)});
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public class KvExcelTo
        {
            public KvExcelTo()
            {
                KvExcelTo(new ListTableModel()
                {
                    loadAction = ExcelByBase.Json.ImportToListTable,
                    saveAction = ExcelByBase.Data.ExportToJson,
                    isCustomAction = (fullpath, list) =>
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

                        JsonData jsonData = LitJsonHelper.ToObject(File.ReadAllText(fullpath).Trim().Trim('\0'));
                        LitJsonHelper.RevertDictionaryToJson(jsonData, dictionary);
                        return LitJsonHelper.ToJson(jsonData,(p)=> p.Replace("＠", "@"), true);
                    }
                });
            }
        }
    }
}