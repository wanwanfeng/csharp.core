using Library.Excel;
using Library.Extensions;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;

namespace Script
{
    public class ActionExcel : ActionBase
    {
        public override string selectExtension
        {
            get { return ".xlsx|.xlsm|.xls"; }
        }

        public override Func<string, IEnumerable<DataTable>> import
        {
            get
            {
                if (firstIsKey)
                {
                    //自定义的输出（首行为key）
                    return (file) => ExcelUtils.ImportFromExcel(file, false);
                }
                //未经过滤原样输出
                return (file) => ExcelUtils.ImportFromExcel(file);
            }
        }

        public override Action<DataTable, string> export
        {
            get { return (dt, file) => { ExcelUtils.ExportToExcel(dt, file); }; }
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

        public class FixExcel : ActionExcel
        {
            public FixExcel()
            {
                ToCommon((dt, file) => {
                    var path = Path.ChangeExtension(InputPath, "").TrimEnd('.');

                    var list = dt.ToListTable();
                    foreach (List<object> objects in list.Rows)
                    {
                        var temp = path + objects[0].ToString();
                        Console.WriteLine("is now:" + temp);
                        var json = JsonHelper.ToObject(File.ReadAllText(temp).Trim().Trim('\0'));
                        object id = objects[1];
                        string key = objects[2].ToString();
                        string value = objects[3].ToString();
                        string value_zh_cn = objects[4].ToString();
                        objects[3] = JsonHelper.ReadValueByKeyPath(json, id.ToString());
                    }
                    var newdt =  list.ToDataTable();

                    ExcelUtils.ExportToExcel(newdt, file);
                });
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