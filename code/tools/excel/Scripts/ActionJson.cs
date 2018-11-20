using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Excel;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace Script
{
    public class ActionJson : ActionBase
    {
        public override string selectExtension
        {
            get { return ".json"; }
        }

        public override Func<string, IEnumerable<DataTable>> import
        {
            get { return file => new[] {ExcelByBase.Json.ImportToDataTable(file)}; }
        }

        public class ToXml : ActionJson
        {
            public ToXml()
            {
                ToXml();
            }
        }

        public class ToCsv : ActionJson
        {
            public ToCsv()
            {
                ToCsv();
            }
        }

        public class ToExcel : ActionJson
        {
            public ToExcel()
            {
                ToExcel();
            }
        }

        public class ToOneExcel : ActionJson
        {
            public ToOneExcel()
            {
                ToOneExcel();
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public class ToKvExcel : ActionJson
        {
            public ToKvExcel()
            {
                ToKvExcelAll();
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public sealed class KvExcelTo : ActionJson
        {
            public KvExcelTo()
            {
                var isIndent = SystemConsole.GetInputStr("json文件是否进行格式化？(true:false)").AsBool(true);
                export = (table, s) => { ExcelByBase.Data.ExportToJson(table, s, isIndent); };
                KvExcelTo(
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