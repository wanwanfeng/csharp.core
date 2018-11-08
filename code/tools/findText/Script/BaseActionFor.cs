using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library;
using Library.Excel;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace findText
{
    public abstract class BaseActionFor
    {

        protected Regex regex { get; set; }

        public virtual string regexStr
        {
            get
            {
                return
                    // "([\u4E00-\u9FA5]+)|([\u30A0-\u30FF])";
                    "([\u0800-\u4E00]+)|([\u4E00-\u9FA5])|([\u30A0-\u30FF])";
            }
        }

        protected virtual List<JsonData> SetJsonDataArray(ListTable list, bool isReverse = false)
        {
            List<string> first = list.Key;
            if (isReverse)
                list.List.Reverse();

            List<JsonData> jsonDatas = new List<JsonData>();
            foreach (List<object> objects in list.List)
            {
                JsonData data = new JsonData();
                for (int j = 0; j < first.Count; j++)
                {
                    string val = objects[j].ToString();
                    val = val.Replace("::", ":").Replace("\\n", "\n");
                    data[first[j].ToString()] = val;
                }
                jsonDatas.Add(data);
            }
            return jsonDatas;
        }

        public virtual ListTable GetJsonDataArray(string content)
        {
            return JsonHelper.ConvertJsonToListTable(content, str =>
            {
                return str.ToString().Replace(":", "::").Replace("\n", "\\n");
            });
        }

        private void WriteExcel(JsonData resJsonData)
        {
            var vals = GetJsonDataArray(JsonHelper.ToJson(resJsonData));
            if (vals.List.Count == 0)
            {
                Console.WriteLine("未搜索到结果！");
                return;
            }
            Console.WriteLine("正在写入Excel...");
            string outpath = inputPath + ".xlsx";
            ExcelByBase.Data.ExportToExcel(vals, outpath);
            Console.WriteLine("写入完成，正在启动...");
            System.Diagnostics.Process.Start(outpath);
        }

        protected string inputPath = "";
        protected List<string> all = new List<string>();
        private JsonData resJsonData = new JsonData();

        protected virtual string textName
        {
            get { return "Find_Text"; }
        }

        protected virtual string exName
        {
            get { return "*.*"; }
        }

        public void Open(string input)
        {
            if (!Directory.Exists(input))
            {
                Console.WriteLine("文件夹路径不存在!");
            }
            else
            {
                all = DirectoryHelper.GetFiles(inputPath = input, exName.Replace("*", ""), SearchOption.AllDirectories).ToList();
                all.Sort();
                resJsonData = new JsonData();
                regex = new Regex(regexStr);
                OpenRun();
                WriteExcel(resJsonData);
            }
        }

        protected abstract void OpenRun();

        protected string[] GetShowInfo(int i)
        {
            Console.WriteLine("搜索中...请稍后" + ((float) i / all.Count).ToString("f") + "\t" + all[i]);
            return File.ReadAllLines(all[i]);
        }

        protected string GetShowAll(int i)
        {
            Console.WriteLine("搜索中...请稍后" + ((float) i / all.Count).ToString("f") + "\t" + all[i]);
            return File.ReadAllText(all[i]);
        }

        protected void GetJsonValue(string val, int i, string k, string input)
        {
            var dir = Path.GetDirectoryName(inputPath).Replace("\\", "/");
            if (string.IsNullOrEmpty(val.Trim())) return;
            JsonData jsonData = new JsonData();
            jsonData["文件名"] = all[i].Replace(dir, "");
            jsonData["行号"] = k;
            jsonData["原文"] = input.Trim();
            jsonData["译文"] = val.Trim();
            resJsonData.Add(jsonData);
        }

        protected void GetJsonValue(string val, int i, int k, string[] input)
        {
            var dir = Path.GetDirectoryName(inputPath).Replace("\\", "/");
            if (string.IsNullOrEmpty(val.Trim())) return;
            JsonData jsonData = new JsonData();
            jsonData["文件名"] = all[i].Replace(dir, "");
            jsonData["行号"] = k + 1;
            jsonData["原文"] = input[k].Trim();
            jsonData["译文"] = val.Trim();
            resJsonData.Add(jsonData);
        }

        /// <summary>
        /// 文件只读取一次
        /// </summary>
        /// <param name="inputPath"></param>
        public virtual void Revert(string inputPath)
        {
            var excels = ExcelByBase.Data.ImportToDataTable(inputPath, false)
                .Select(ExcelByBase.Data.ConvertToJson)
                .ToList();

            var list = new List<string>();

            foreach (JsonData jsonData in excels)
            {
                var lookup = jsonData.Cast<JsonData>().ToList().ToLookup(p => p["文件名"].ToString(), q => q);
                Dictionary<string, List<JsonData>> cache = lookup.ToDictionary(p => p.Key, q => q.ToList());

                int i = 0;
                foreach (KeyValuePair<string, List<JsonData>> kv in cache)
                {
                    string temp = kv.Key;
                    Console.WriteLine("还原中...请稍后" + ((float)(++i) / cache.Count).ToString("p1") + "\t" + temp);
                    string path = (Path.GetDirectoryName(inputPath) + temp).Replace("\\", "/");

                    if (File.Exists(path))
                    {
                        string[] content = File.ReadAllLines(path);
                        foreach (var data in kv.Value)
                        {
                            int line = data["行号"].ToString().AsInt();
                            string oldStr = data["原文"].ToString();
                            //string oldStr2 = data["需翻译"].ToString();
                            string newStr = data["译文"].ToString();
                            //if (content[line] == oldStr)

                            var linec = content[line - 1];
                            if (linec.Contains(oldStr))
                            {
                                content[line - 1] = linec.Replace(oldStr, newStr);
                            }
                            else
                            {
                                list.Add("替换失败：" + temp + "/" + line + "/" + oldStr + "/" + newStr);
                            }
                        }
                        File.WriteAllLines(path, content);
                    }
                    else
                    {
                        list.Add("不存在的文件：" + path);
                    }
                    File.WriteAllLines(inputPath + ".txt", list.ToArray());
                }
            }
        }
    }
}
