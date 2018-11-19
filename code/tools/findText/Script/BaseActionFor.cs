using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Library;
using Library.Excel;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace findText
{
    public abstract class BaseActionFor : BaseSystemConsole
    {
        [Description("https://www.cnblogs.com/csguo/p/7401874.html")]
        public enum MyEnum
        {
            [Description("日文"), StringValue("([\u0800-\u4E00])")] 日文 = 1,
            [Description("日文平假名"), StringValue("([\u3040-\u309F])")] 日文平假名,
            [Description("日文片假名"), StringValue("([\u30A0-\u30FF])")] 日文片假名,
            [Description("日文片假名语音扩展"), StringValue("([\u31F0-\u31FF])")] 日文片假名语音扩展,
            [Description("中文"), StringValue("([\u4E00-\u9FA5])")] 中文 = 10
        }


        protected Regex regex { get; set; }

        public virtual string regexStr { get; set;
            //get
            //{
            //    return
            //        // "([\u4E00-\u9FA5]+)|([\u30A0-\u30FF])";
            //        "([\u0800-\u4E00]+)|([\u4E00-\u9FA5])|([\u30A0-\u30FF])";
            //}
            //set { }
        }

        public virtual ListTable GetJsonDataArray(string content)
        {
            return JsonHelper.ConvertJsonToListTable(content);
            return JsonHelper.ConvertJsonToListTable(content, str =>
            {
                return str.ToString().Replace(":", "::").Replace("\n", "\\n");
            });
        }

        private JsonData resJsonData = new JsonData();

        protected virtual string textName
        {
            get { return "Find_Text"; }
        }

        protected virtual string exName
        {
            get { return "*.*"; }
        }

        public void Open()
        {
            var cache = AttributeHelper.GetCacheStringValue<MyEnum>();
            var str = string.Join("|", cache.Values);
            {
                SystemConsole.Run(config: new Dictionary<string, Action>()
                {
                    {
                        "匹配中文与日文",
                        () =>
                        {
                            regexStr = str;
                            GetValue();
                        }
                    },
                    {
                        "只匹配中文", () =>
                        {
                            regexStr = cache[MyEnum.中文];
                            GetValue();
                        }
                    },
                    {
                        "只匹配日文", () =>
                        {
                            regexStr = string.Join("|", cache.Where(p => p.Key < MyEnum.中文).Select(p => p.Value));
                            GetValue();
                        }
                    }
                });
            }
        }

        private void GetValue()
        {
            resJsonData = new JsonData();
            regex = new Regex(regexStr);
            CheckPath(exName.Replace("*.", "."), SelectType.Folder).ForEach((file, i, count) =>
            {
                Console.WriteLine("搜索中...请稍后" + ((float) i/count).ToString("p") + "\t" + file);
                OpenRun(file);
            });

            var vals = GetJsonDataArray(JsonHelper.ToJson(resJsonData));
            if (vals.List.Count == 0)
            {
                Console.WriteLine("未搜索到结果！");
                return;
            }
            Console.WriteLine("正在写入Excel...");
            string outpath = string.Format("{0}[{1}].xlsx", InputPath, exName.Replace("*", "").Replace("*", ""));
            ExcelByBase.Data.ExportToExcel(vals, outpath);
            Console.WriteLine("写入完成，正在启动...");
            System.Diagnostics.Process.Start(outpath);
        }

        protected abstract void OpenRun(string file);

        protected void GetJsonValue(string val, string file, string k, string input)
        {
            var dir = Path.GetDirectoryName(InputPath).Replace("\\", "/");
            if (string.IsNullOrEmpty(val.Trim())) return;
            JsonData jsonData = new JsonData();
            jsonData["文件名"] = file.Replace(dir, "");
            jsonData["行号"] = k;
            jsonData["原文"] = input.Trim();
            jsonData["译文"] = val.Trim();
            resJsonData.Add(jsonData);
        }

        protected void GetJsonValue(string val, string file, int k, string[] input)
        {
            var dir = Path.GetDirectoryName(InputPath).Replace("\\", "/");
            if (string.IsNullOrEmpty(val.Trim())) return;
            JsonData jsonData = new JsonData();
            jsonData["文件名"] = file.Replace(dir, "");
            jsonData["行号"] = k + 1;
            jsonData["原文"] = input[k].Trim();
            jsonData["译文"] = val.Trim();
            resJsonData.Add(jsonData);
        }

        public readonly Dictionary<string, int> Convert = new[]
        {
            "文件名",
            "行号",
            "原文",
            "译文"
        }.Select((p, i) => new {k = p, index = i}).ToDictionary(p => p.k, p => p.index);


        /// <summary>
        /// 文件只读取一次
        /// </summary>
        public virtual void Revert()
        {
            var list = new List<string>();
            var caches = CheckPath(".xlsx", SelectType.File).AsParallel().SelectMany(file =>
            {
                Console.WriteLine(" from : " + file);
                return ExcelByBase.Data.ImportToDataTable(file);
            }).Select(table =>
            {
                var cache =
                    ExcelByBase.Data.ConvertToListTable(table)
                        .List
                        .ToLookup(p => p.First())
                        .ToDictionary(p => p.Key.ToString(), q => q.ToList());
                cache.Remove("文件名");
                return new {dic = cache, file = table.FullName};
            }).ToList();

            foreach (var cach in caches)
            {
                int i = 0;
                foreach (KeyValuePair<string, List<List<object>>> pair in cach.dic)
                {
                    string temp = pair.Key;
                    Console.WriteLine("还原中...请稍后" + ((float)(++i) / cach.dic.Count).ToString("p1") + "\t" + temp);
                    string path = (Path.GetDirectoryName(InputPath) + temp).Replace("\\", "/");

                    if (File.Exists(path))
                    {
                        string[] content = File.ReadAllLines(path);
                        foreach (var data in pair.Value)
                        {
                            int line = data[Convert["行号"]].ToString().AsInt();
                            string oldStr = data[Convert["原文"]].ToString();
                            //string oldStr2 = data[convert["需翻译"]].ToString();
                            string newStr = data[Convert["译文"]].ToString();
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
                        File.WriteAllLines(path, content, new UTF8Encoding(false));
                    }
                    else
                    {
                        list.Add("不存在的文件：" + path);
                    }
                }
            }

            File.WriteAllLines(InputPath + ".txt", list.ToArray());
        }
    }
}
