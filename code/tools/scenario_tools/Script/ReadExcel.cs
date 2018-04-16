﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Excel;
using Library.Helper;
using Library.LitJson;
using LitJson;

namespace scenario_tools
{
    public class ReadExcel
    {
        private string path = "";

        public ReadExcel()
        {
            path = Console.ReadLine() ?? @"D:\Work\yuege\www\assets\res\scenario.xls";
            var dic = new ExcelByReader().ReadFromExcels(path);

            Dictionary<string, List<JsonData>> dicT = new Dictionary<string, List<JsonData>>();

            foreach (KeyValuePair<string, List<List<object>>> keyValuePair in dic)
            {
                JsonData jsonData = ExcelByReader.ConvertListToJson(keyValuePair);

                foreach (JsonData json in jsonData)
                {
                    if (!json.Keys.Contains("file")) continue;
                    var fileName = json["file"].ToString();
                    List<JsonData> jsonDatas;
                    if (dicT.TryGetValue(fileName, out jsonDatas))
                    {
                        jsonDatas.Add(json);
                    }
                    else
                    {
                        jsonDatas = new List<JsonData>() {json};
                        dicT.Add(fileName, jsonDatas);
                    }
                }
            }

            foreach (KeyValuePair<string, List<JsonData>> keyValuePair in dicT)
            {
                var res = new JsonData();
                foreach (JsonData jsonData in keyValuePair.Value)
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
                var fileName = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "/" +
                               keyValuePair.Key + ".json";
                FileHelper.CreateDirectory(fileName);
                File.WriteAllText(fileName, LitJsonHelper.ToJson(res));
            }
        }
    }
}