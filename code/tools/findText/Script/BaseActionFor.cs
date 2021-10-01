using System;
using System.Collections.Generic;
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
    public abstract class BaseActionFor : BaseSystemExcel
    {
		protected Regex regex { get; private set; }

		protected bool CheckMatches(string val)
		{
			return CheckMatches(regex, val);
		}

        public virtual string regexStr { get; set; }

        public virtual ListTable GetJsonDataArray(string content)
        {
            return JsonHelper.ConvertJsonToListTable(content);
            return JsonHelper.ConvertJsonToListTable(content, str =>
            {
                return str.ToString().Replace(":", "::").Replace("\n", "\\n");
            });
        }

        private JsonData resJsonData = new JsonData();

        protected virtual string textName => "Find_Text";

        protected virtual string exName => "*.*";

		public string selectExtension => Environment.GetEnvironmentVariable(GetType().Name + ".SelectExtension") ?? exName;

        public void Open()
		{

			var cache = AttributeHelper.GetCacheStringValue<RegexLanguaheEnum>();
			SystemConsole.Run(config: new Dictionary<string, Action>()
			{
				["匹配中文与日文"] = () =>
				{
					regexStr = string.Join("|", cache.Where(p => p.Key >= RegexLanguaheEnum.日文 && p.Key <= RegexLanguaheEnum.中文).Select(p => p.Value));
					GetValue();
				},

				["只匹配中文"] = () =>
				{
					regexStr = cache[RegexLanguaheEnum.中文];
					GetValue();
				},

				["只匹配日文"] = () =>
				{
					regexStr = string.Join("|", cache.Where(p => p.Key < RegexLanguaheEnum.中文).Select(p => p.Value));
					GetValue();
				},

				["只匹配韩文"] = () =>
				{
					regexStr = cache[RegexLanguaheEnum.韩文];
					GetValue();
				}
			});
		}

        protected virtual string ToJson(object json)
        {
            return JsonHelper.ToJson(json);
        }

        private void GetValue()
        {
            resJsonData = new JsonData();
            regex = new Regex(regexStr);
			CheckPath(selectExtension, SelectType.All)
				.Where(p=>!p.Contains("/Assets/WFS/fbschema/MasterTool/MasterTool/MasterTool"))
				.ToList()
				.ForEach(OpenRun, "搜索中...请稍后");

			if (resJsonData.IsArray && resJsonData.Count == 0)
			{
				Console.WriteLine("未搜索到结果！");
				return;
			}

			string outpath = string.Format("{0}({1}).json", InputPath, selectExtension.Replace("*", "").Replace("|", ""));
			File.WriteAllText(outpath, JsonHelper.ToJson(resJsonData, indentLevel: 2));

			var vals = GetJsonDataArray(File.ReadAllText(outpath));
            if (vals.Rows.Count == 0)
            {
                Console.WriteLine("未搜索到结果！");
                return;
            }
			outpath = Path.ChangeExtension(outpath, ".xlsx");
			Console.WriteLine("正在写入Excel...");
            ExcelUtils.ExportToExcel(vals.ToDataTable(), outpath);
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
            jsonData["译文"] = "";
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
            jsonData["译文"] = "";
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
            var caches = GetFileCaches();
            if (caches.Count == 0) return;

            var list = new List<string>();
            foreach (var dic in caches)
            {
                int i = 0;
                foreach (KeyValuePair<string, List<List<object>>> pair in dic)
                {
                    string temp = pair.Key;
                    Console.WriteLine("还原中...请稍后" + ((float) (++i)/dic.Count).ToString("p1") + "\t" + temp);
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
