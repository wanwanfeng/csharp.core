using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Excel;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace Script
{
    /// <summary>
    /// 比较Excel找出差异并导出到文件
    /// </summary>
    public class CompareExcel
    {
        public CompareExcel()
        {
            var dir1 = SystemConsole.GetInputStr("请拖入目标文件(.xls|xlsx):", "您选择的文件：");
            var dir2 = SystemConsole.GetInputStr("请拖入比较文件(.xls|xlsx):", "您选择的文件：");

            var listTables =
                ExcelByBase.Data.ImportToDataTable(dir1, false)
                    .Select(p => (ListTable) p)
                    .ToDictionary(p => p.TableName);
            var listTables2 =
                ExcelByBase.Data.ImportToDataTable(dir2, false)
                    .Select(p => (ListTable) p)
                    .ToDictionary(p => p.TableName);

            foreach (var keyValuePair in listTables)
            {
                ListTable lst;
                if (listTables2.TryGetValue(keyValuePair.Key, out lst))
                {
                    var intersectList = keyValuePair.Value.Rows.AsParallel().AsOrdered()
                        .Where((objects, i) =>
                        {
                            Console.WriteLine(i);
                            return lst.Rows.Any(objects.SequenceEqual);
                        }).ToList();
                    var exceptList = keyValuePair.Value.Rows.Except(intersectList);

                    //var exceptList = keyValuePair.Value.List.Except(lst.List).ToList();
                    //var intersectList = keyValuePair.Value.List.Intersect(lst.List).ToList();
                    // ReSharper disable once PossibleNullReferenceException
                    var newName = Path.ChangeExtension(dir1, "").Trim('.') + "-" + Path.GetFileNameWithoutExtension(dir2);
                    keyValuePair.Value.Rows = exceptList.ToList();
                    ExcelByBase.Data.ExportToExcel(keyValuePair.Value, string.Format("{0}-except.xlsx", newName));
                    keyValuePair.Value.Rows = intersectList.ToList();
                    ExcelByBase.Data.ExportToExcel(keyValuePair.Value, string.Format("{0}-intersect.xlsx", newName));
                }
                else
                {
                    Console.WriteLine(dir2 + " 不存在的sheet：" + keyValuePair.Key);
                }
            }
            Console.WriteLine("完毕");
        }
    }

    public class CompareJson
    {
        public CompareJson()
        {
            var dir1 = SystemConsole.GetInputStr("请拖入目标文件(.json):", "您选择的文件：");
            var dir2 = SystemConsole.GetInputStr("请拖入比较文件(.json):", "您选择的文件：");

            Func<JsonData, string> getKey = p => p["Columns0"].ToString() + "@" + p["Columns1"].ToString() + "@" + p["Columns2"].ToString();

            var jsondatas = JsonHelper.ImportJson<JsonData[]>(dir1);

            var listTables = JsonHelper.ImportJson<JsonData[]>(dir1).Select(p =>
            {
                p["key"] = getKey(p);
                return p;
            }).ToDictionary(p => p["key"].ToString());
            var listTables2 = JsonHelper.ImportJson<JsonData[]>(dir2).Select(p =>
            {
                p["key"] = getKey(p);
                return p;
            }).ToDictionary(p => p["key"].ToString());

            var intersectList = listTables.Keys.Intersect(listTables2.Keys).ToDictionary(p => p, p => listTables[p]);
            var exceptList = listTables.Keys.Except(listTables2.Keys).ToDictionary(p => p, p => listTables[p]);

            foreach (KeyValuePair<string, JsonData> data in listTables)
            {
                JsonData json;
                if (intersectList.TryGetValue(data.Key, out json))
                {
                    data.Value["Columns4"] = json["Columns4"];
                }
            }


            var res = listTables.Select(p =>
            {
                p.Value.Remove("key");
                return p.Value;
            }).ToArray();
            File.WriteAllText("intersectList.json", JsonHelper.ToJson(res, indentLevel: 2));
        }
    }

}