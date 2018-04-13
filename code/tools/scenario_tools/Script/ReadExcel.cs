using System;
using System.Collections.Generic;
using System.IO;
using Library.Excel;
using Library.Helper;
using Library.LitJson;
using LitJson;

namespace scenario_tools
{
    public class ReadExcel
    {
        public ReadExcel()
        {
            var path = Console.ReadLine() ?? @"D:\Work\yuege\www\assets\res\scenario.xls";
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
                    foreach (var key in jsonData.Keys)
                    {
                        if (key != "")
                            xx[key] = jsonData[key];
                    }
                    res.Add(xx);
                }
                var fileName = @"\res\scenario\" + keyValuePair.Key;
                FileHelper.CreateDirectory(fileName);
                File.WriteAllText(fileName, LitJsonHelper.ToJson(res));
            }
        }
    }
}