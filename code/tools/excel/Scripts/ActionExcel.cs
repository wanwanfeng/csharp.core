using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Excel;
using Library.Helper;
using Library.LitJson;

namespace Script
{
    public class ActionExcel : ActionBase
    {
        public class ToCsv : ActionExcel
        {
            public ToCsv()
            {
                List<string> files = CheckPath(".xlsx|.xls");
                if (files.Count == 0) return;
                files.ForEach(file =>
                {
                    Console.WriteLine(" is now : " + file);
                    var dts = ExcelByNpoi.ExcelToDataTable(file);
                    dts.ForEach(p =>
                    {
                        ExcelByBase.ConvertDataTableToCsv(p, Path.ChangeExtension(file, ".csv"));
                    });
                });
            }
        }

        public class ToJson : ActionExcel
        {
            public ToJson()
            {
                List<string> files = CheckPath(".xlsx|.xls");
                if (files.Count == 0) return;
                ExcelByNpoi excel = new ExcelByNpoi();
                files.ForEach(file =>
                {
                    Console.WriteLine(" is now : " + file);
                    var vals = excel.ReadFromExcels(file);

                    if (vals.Count == 1)
                    {
                        ExcelByBase.ConvertListToJsonFile(vals.First(), file);
                        return;
                    }

                    foreach (KeyValuePair<string, List<List<object>>> pair in vals)
                    {
                        string newPath = file.Replace(Path.GetExtension(file), "\\" + pair.Key.Replace("$", ""));
                        FileHelper.CreateDirectory(newPath);
                        ExcelByBase.ConvertListToJsonFile(pair, newPath);
                    }
                });
            }
        }

        public class ToOneExcel : ActionExcel
        {
            public ToOneExcel()
            {
                List<string> files = CheckPath(".xlsx|.xls", SelectType.Folder);
                if (files.Count == 0) return;

                var dic = new Dictionary<string, List<List<object>>>();
                files.ForEach(file =>
                {
                    var vals = new ExcelByNpoi().ReadFromExcels(file);

                    if (vals.Count == 1)
                    {
                        dic[file] =
                            ExcelByBase.ConvertJsonToList(
                                LitJsonHelper.ToJson(ExcelByBase.ConvertListToJson(vals.First())));
                        return;
                    }

                    foreach (KeyValuePair<string, List<List<object>>> pair in vals)
                    {
                        dic[file + "/" + pair.Key] =
                            ExcelByBase.ConvertJsonToList(LitJsonHelper.ToJson(ExcelByBase.ConvertListToJson(pair)));
                    }

                });
                new ExcelByNpoi().WriteToOneExcel(InputPath + ".xlsx", dic);
            }
        }
    }
}