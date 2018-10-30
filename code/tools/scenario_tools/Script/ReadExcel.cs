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
            var filePath = SystemConsole.GetInputStr("请输入剧情文件（.xls|.xlsx）：", def: @"D:\Work\yuege\www\assets\res\scenario.xls");
            var listJson = ExcelByBase.Data.ImportToDataTable(filePath, false).Select(ExcelByBase.Data.ConvertToJson).ToList();

            foreach (JsonData data in listJson)
            {
                var lookup = data.Cast<JsonData>().ToLookup(p => p["file"].ToString(), q => q);
                Dictionary<string, List<JsonData>> cache = lookup.ToDictionary(p => p.Key, q => q.ToList());

                var index = 0;
                foreach (KeyValuePair<string, List<JsonData>> pair in cache)
                {
                    var res = new JsonData();
                    Console.WriteLine("剧情文件({1})：{0}", pair.Key, (++index) / (float) cache.Count);
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