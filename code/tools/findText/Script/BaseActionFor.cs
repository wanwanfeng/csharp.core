using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using findText.Script;
using Library.Excel;
using Library.Extensions;
using Library.Helper;
using Library.LitJson;
using LitJson;

namespace findText
{
    public enum ConvertType
    {
        [TypeValue(typeof (ActionForCpp))] cpp,
        [TypeValue(typeof (ActionForCSharp))] csharp,
        [TypeValue(typeof (ActionForPhp))] php,
        [TypeValue(typeof (ActionForJava))] java,
        [TypeValue(typeof (ActionForJavaScript))] javascript,
        [TypeValue(typeof (ActionForHtml))] html,
    }

    public abstract class BaseActionFor
    {

        protected static Regex regex = new Regex(
            //"\"([\u4E00-\u9FA5]+)|([\u4E00-\u9FA5]+.*\")|(\".*[\u30A0-\u30FF]+)|([\u30A0-\u30FF])\""
            "([\u4E00-\u9FA5]+)|([\u4E00-\u9FA5]')|([\u30A0-\u30FF])|([\u30A0-\u30FF])"
            //regexStr = "/u0800-/u4e00"
            );

        private JsonData SetJsonDataArray(List<List<object>> content, bool isReverse = false)
        {
            JsonData jsonDatas = new JsonData();
            jsonDatas.SetJsonType(JsonType.Array);
            List<object> first = content.First();
            content.Remove(first);
            if (isReverse)
                content.Reverse();
            foreach (List<object> objects in content)
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

        private List<List<object>> GetJsonDataArray(string content)
        {
            return LitJsonHelper.ConvertJsonToList(content, str =>
            {
                return str.Replace(":", "::").Replace("\n", "\\n");
            });
        }

        private void WriteExcel(string fileName, JsonData resJsonData)
        {
            Console.WriteLine("正在写入Excel...");
            string outpath = inputPath + ".xls";
            List<List<object>> vals = GetJsonDataArray(JsonMapper.ToJson(resJsonData));
            new ExcelByNpoi().WriteToExcel(outpath, vals);
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

        protected virtual string[] exName
        {
            get { return new string[0]; }
        }

        public void Open(string input)
        {
            inputPath = input;

            if (!Directory.Exists(inputPath))
            {
                Console.WriteLine("文件夹路径不存在!");
            }
            else
            {
                var res =
                    Directory.GetFiles(inputPath, "*", SearchOption.AllDirectories)
                        .Select(p => p.Replace("\\", "/"))
                        .ToArray();
                foreach (var s in exName)
                {
                    all.AddRange(res.Where(p =>
                    {
                        var xx = Path.GetExtension(p);
                        return !string.IsNullOrEmpty(xx) && xx == s;
                    }));
                }
                if (all.Count == 0) return;
                all.Sort();
                OpenRun();
                WriteExcel(textName, resJsonData);
            }
        }

        protected abstract void OpenRun();

        protected string[] GetShowInfo(int i)
        {
            Console.WriteLine("搜索中...请稍后" + ((float) i/all.Count).ToString("f") + "\t" + all[i]);
            return File.ReadAllLines(all[i]);
        }

        protected void GetJsonValue(string val, int i, int k, string[] input)
        {
            if (string.IsNullOrEmpty(val.Trim())) return;
            JsonData jsonData = new JsonData();
            jsonData["文件名"] = all[i];
            jsonData["行号"] = k + 1;
            jsonData["原文"] = input[k].Trim();
            jsonData["译文"] = val.Trim();
            resJsonData.Add(jsonData);
        }

        public virtual void Revert(string inputPath)
        {
            if (!File.Exists(inputPath))
            {
                Console.WriteLine("file is not exist!");
            }
            else
            {
                Dictionary<string, List<List<object>>> dic = new ExcelByReader().ReadFromExcels(inputPath);

                foreach (KeyValuePair<string, List<List<object>>> pair in dic)
                {
                    JsonData jsonData = SetJsonDataArray(pair.Value, true);

                    var i = 0;
                    foreach (JsonData data in jsonData)
                    {
                        string temp = data["文件名"].ToString();
                        Console.WriteLine("还原中...请稍后" + ((float)i / all.Count).ToString("f") + "\t" + temp);

                        string path = (inputPath + temp).Replace("\\", "/");
                        string[] content = File.ReadAllLines(path);
                        int line = data["行号"].ToString().AsInt();
                        string oldStr = data["原文"].ToString();
                        //string oldStr2 = data["需翻译"].ToString();
                        string newStr = data["译文"].ToString();
                        //if (content[line] == oldStr)

                        var linec = content[line - 1];
                        content[line - 1] = linec.Replace(oldStr, newStr);
                        File.WriteAllLines(path, content);
                    }

                    //foreach (KeyValuePair<string, JsonData> data in jsonData.Inst_Object)
                    //{
                    //    string temp = data.Value["文件名"].ToString();
                    //    string[] content = File.ReadAllLines(temp);
                    //    int line = data.Value["行号"].ToString().AsInt() + 1;
                    //    string oldStr = data.Value["原文"].ToString();
                    //    string oldStr2 = data.Value["需翻译"].ToString();
                    //    string newStr = data.Value["译文"].ToString();
                    //    if (content[line] == oldStr)
                    //        content[line] = content[line].Replace(oldStr2, newStr);
                    //    File.WriteAllLines(temp, content);
                    //}
                }
            }
        }
    }
}
