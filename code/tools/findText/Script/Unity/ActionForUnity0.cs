using Library.Helper;
using System;
using System.Collections.Generic;
using System.IO;

namespace findText.Script
{
    /// <summary>
    /// 按行读取
    /// </summary>
    public class ActionForUnity0 : ActionBaseUnity
    {
        protected override string textName
        {
            get { return "Find_Unity_Text"; }
        }

        protected override string exName
        {
            get { return "*.prefab|*.unity"; }
        }

        protected override void OpenRun(string file)
        {
            string[] inputs = File.ReadAllLines(file);

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i].TrimStart().StartsWith("mText:"))
                {
                    var temp = inputs[i].TrimStart().Split(':');
                    var second = temp[1].Trim();
                    if (string.IsNullOrEmpty(second)) continue;
                    if (second.StartsWith("\""))
                    {
                        var xx = StringHelper.Unicode2String(second).Replace(@"\r\n", @"\n");
                        GetJsonValue(xx, file, i.ToString() + "/" + temp[0], xx);
                    }
                    else
                    {
                        GetJsonValue(second, file, i.ToString() + "/" + temp[0], second);
                    }
                }
            }
        }

        public override void Revert()
        {
            var caches = GetFileCaches();
            if (caches.Count == 0) return;

            foreach (var dic in caches)
            {
                var i = 0;
                foreach (KeyValuePair<string, List<List<object>>> valuePair in dic)
                {
                    string temp = valuePair.Key;
                    Console.WriteLine("还原中...请稍后" + ((float)(++i) / dic.Count).ToString("p1") + "\t" + temp);

                    string path = (Path.GetDirectoryName(InputPath) + temp).Replace("\\", "/");
                    ReadAllLines(valuePair, path);
                }
            }

            void ReadAllLines(KeyValuePair<string, List<List<object>>> valuePair, string file)
            {
                bool isSave = false;
                string[] inputs = File.ReadAllLines(file);
            
                foreach (List<object> data in valuePair.Value)
                {
                    string xPath = data[Convert["行号"]].ToString();
                    string oldStr = data[Convert["原文"]].ToString();
                    string newStr = data[Convert["译文"]].ToString();

                    var array = xPath.Split('/');
                    int line = int.Parse(array[0]);
                    if (inputs[line].TrimStart().StartsWith(array[1]))
                    {
                        inputs[line] = inputs[line].Split(':')[0] + newStr;
                    }
                }
                if (isSave)
                {
                    File.WriteAllLines(file, inputs);
                }
            }
        }
    }
}