using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Excel;
using LitJson;

namespace excel
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                @"请选择：
1：json->csv
2：json->xlsx
3：xlsx->json"
                );
            string s = Console.ReadLine();
            switch (s)
            {
                case "1":
                    ReadjsonToCsv();
                    break;
                case "2":
                    ReadJsonToExcel();
                    break;
                case "3":
                    ReadExcelToJson();
                    break;

                default:
                    break;
            }
            GC.Collect();
            Console.ReadKey();
        }

        #region Json

        /// <summary>
        /// json->csv
        /// </summary>
        private static void ReadjsonToCsv()
        {
            List<string> files;
            if (!CheckPath(out files))
                return;
            foreach (var file in files)
            {
                Console.WriteLine(" is now : " + file);
                List<List<object>> vals = GetJsonDataArray(File.ReadAllText(file));
                List<string> res = new List<string>();
                foreach (List<object> objects in vals)
                {
                    res.Add(string.Join(",", objects.Select(p =>
                    {
                        return "\"" + p + "\"";
                    })));
                }
                File.WriteAllLines(Path.ChangeExtension(file, ".csv"), res, Encoding.UTF8);
            }
        }

        /// <summary>
        /// json->xlsx
        /// </summary>
        /// <param name="isOne"></param>
        private static void ReadJsonToExcel(bool isOne = false)
        {
            List<string> files;
            if (!CheckPath(out files)) return;
            foreach (string file in files)
            {
                Console.WriteLine(" is now : " + file);
                List<List<object>> vals = GetJsonDataArray(File.ReadAllText(file));
                if (isOne)
                {
                    Haha.WriteToExcelOne(file, vals);
                }
                else
                {
                    Haha.WriteToExcel(Path.ChangeExtension(file, ".xlsx"), vals);
                }
            }
            //EditorExcelTools.MS_ExportExcel(Path.ChangeExtension(file, ".xls"), keys, vals);
        }

        private static bool CheckPath(out List<string> files, string exce = ".json")
        {
            string path = (Environment.CurrentDirectory + "/json/").Replace("\\", "/");
            if (!Directory.Exists(path))
            {
                files = new List<string>();
                Console.WriteLine(path + " is not exists !");
                return false;
            }
            files = Directory.GetFiles(path).Where(p => p.EndsWith(exce)).ToList();
            files.Sort();
            return true;
        }

        private static List<List<object>> GetJsonDataArray(string content)
        {
            JsonData[] jsonDatas = JsonMapper.ToObject<JsonData[]>(content.Trim().Trim('\0'));
            //获取key集合
            List<string> keys = new List<string>();
            foreach (JsonData jsonData in jsonDatas)
            {
                foreach (var pair in jsonData.Inst_Object)
                {
                    if (keys.Contains(pair.Key))
                        continue;
                    keys.Add(pair.Key);
                }
            }
            //获取key集合对应的值集合
            var vals = new List<List<object>>();
            foreach (JsonData jsonData in jsonDatas)
            {
                List<object> val = new List<object>();
                vals.Add(val);

                foreach (var key in keys)
                {
                    JsonData value;
                    var str = jsonData.Inst_Object.TryGetValue(key, out value) ? value.ToString() : "";
                    val.Add(str.Replace(":", "::").Replace("\n", "\\n"));
                }
            }
            vals.Insert(0, keys.Select(p => (object) p).ToList());
            return vals;
        }

        #endregion

        #region Excel

        /// <summary>
        /// xlsx->json
        /// </summary>
        private static void ReadExcelToJson()
        {
            List<string> files;
            if (!CheckPath(out files, "xlsx"))
                return;
            foreach (var file in files)
            {
                Console.WriteLine(" is now : " + file);
                List<List<object>> vals = Haha.ReadFromExcel(Path.ChangeExtension(file, ".xlsx"));

                Queue<List<object>> queue = new Queue<List<object>>(vals);
                List<object> keyList = queue.Dequeue();
                List<JsonData> resDatases = new List<JsonData>();
                while (queue.Count != 0)
                {
                    Queue<object> queueVal = new Queue<object>(queue.Dequeue());
                    JsonData jsonData = new JsonData();
                    foreach (object o in keyList)
                    {
                        jsonData[o.ToString()] = queueVal.Dequeue().ToString();
                    }
                    resDatases.Add(jsonData);
                }
                File.WriteAllText(
                    Path.GetDirectoryName(file) + "/json/" + Path.GetFileNameWithoutExtension(file) + ".json",
                    LitJson.JsonMapper.ToJson(resDatases),
                    Encoding.UTF8);
            }
        }

        #endregion
    }
}
