using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace findText.Script
{
    /// <summary>
    /// 使用YamlStream读取
    /// </summary>
    public class ActionForUnity : ActionBaseUnity
    {
        protected override string textName
        {
            get { return "Find_Unity_Text"; }
        }

        protected override string exName
        {
            //get { return "*.prefab|*.unity"; }
            get { return "*.prefab"; }
        }

        protected override void OpenRun(string file)
        {
            string[] inputs = File.ReadAllLines(file);

            var input = new StreamReader(file, Encoding.UTF8);
            var yaml = new YamlStream();
            yaml.Load(input);

            for (int i = 0; i < yaml.Documents.Count; i++)
            {
                var mapping = (YamlMappingNode)yaml.Documents[i].RootNode;
                foreach (var pair in mapping.Children)
                {
                    if (pair.Key.ToString() == "MonoBehaviour")
                    {
                        var entrys = (YamlMappingNode)pair.Value;

                        foreach (var item in entrys.Children)
                        {
                            if (item.Key.ToString() == "mText")
                            {
                                var key = "--- !u!114 &" + mapping.Anchor + "/" + pair.Key + "/" + item.Key;
                                Console.WriteLine(key + ":" + item.Value);
                                if (item.Value == null) continue;
                                GetJsonValue(item.Value.ToString(), file, key, item.Value.ToString());
                            }
                        }
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
                var input = new StreamReader(file, Encoding.UTF8);
                var yaml = new YamlStream();
                yaml.Load(input);

                bool isSave = false;

                foreach (List<object> data in valuePair.Value)
                {
                    string xPath = data[Convert["行号"]].ToString();
                    string oldStr = data[Convert["原文"]].ToString();
                    string newStr = data[Convert["译文"]].ToString();

                    for (int i = 0; i < yaml.Documents.Count; i++)
                    {
                        Queue<string> queues = new Queue<string>(xPath.Split('/'));
                        var mapping = (YamlMappingNode)yaml.Documents[i].RootNode;
                        if ("--- !u!114 &" + mapping.Anchor == queues.Peek())
                        {
                            queues.Dequeue();
                            if (mapping.Children.ContainsKey(queues.Peek()))
                            {
                                var entrys = (YamlMappingNode)mapping.Children[queues.Dequeue()];
                                if (entrys.Children.ContainsKey(queues.Peek()))
                                {
                                    entrys.Children[queues.Dequeue()] = newStr;
                                    isSave = true;
                                }
                            }
                        }
                    }
                }
                input.Close();
                if (isSave)
                    yaml.Save(File.CreateText(file));
            }
        }
    }
}