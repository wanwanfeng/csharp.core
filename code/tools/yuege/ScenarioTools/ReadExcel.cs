using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Excel;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace scenario_tools
{
    public class ReadExcel
    {
        public ReadExcel()
        {
            var filePath = SystemConsole.GetInputStr("请输入剧情文件（.xls|.xlsx）：",
                def: @"D:\Work\yuege\www\assets\res\scenario.xls");

            var cache = ExcelByBase.Data.ImportToDataTable(filePath, false).Select(p => (JsonData) p).Select(
                data =>
                {
                    var dic = data.Cast<JsonData>()
                        .GroupBy(p => p["file"].ToString())
                        .ToDictionary(p => p.Key, q => q.ToList());
                    dic.Remove("file");
                    return dic;
                }).ToList();

            foreach (Dictionary<string, List<JsonData>> dic in cache)
            {
                var index = 0;
                foreach (KeyValuePair<string, List<JsonData>> pair in dic)
                {
                    var res = new JsonData();
                    Console.WriteLine("剧情文件({1})：{0}", pair.Key, (++index)/(float) cache.Count);
                    foreach (JsonData jsonData in pair.Value)
                    {
                        var xx = new JsonData();
                        var keys = jsonData.Keys.Where(p => !p.Contains("_zh_cn"));
                        foreach (var key in keys)
                        {
                            if (key == "" || key == "file") continue;
                            if (jsonData.Keys.Contains(key + "_zh_cn"))
                                xx[key] = jsonData[key + "_zh_cn"];
                            else
                                xx[key] = jsonData[key];
                        }
                        res.Add(xx);
                    }

                    var fileName = Path.ChangeExtension(filePath, "").Trim('.') + "/" + pair.Key + ".json";
                    FileHelper.CreateDirectory(fileName);
                    File.WriteAllText(fileName, JsonHelper.ToJson(res));
                }
            }
        }
    }
}