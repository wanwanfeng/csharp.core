using Library.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace findText.Script
{
    /// <summary>
    /// 文件按---分割分块读取
    /// </summary>
    public class ActionForUnity2 : ActionBaseUnity
    {
        protected override string textName
        {
            get { return "Find_Unity_Text"; }
        }

        protected override string exName
        {
            get { return "*.prefab|*.unity"; }
        }


        public class StructInfo
        {
            public List<string> lines;
            public object yamlObject;
        }

        protected Dictionary<string, StructInfo> CacheYamlObject(string[] inputs)
        {
            Dictionary<string, List<string>> contents = new Dictionary<string, List<string>>();
            var curKey = "";
            foreach (var line in inputs)
            {
                if (line.StartsWith("---"))
                {
                    curKey = line;
                    contents[curKey] = new List<string>();
                    continue;
                }
                contents[curKey].Add(line);
            }

            Dictionary<string, StructInfo> cache = new Dictionary<string, StructInfo>();

            foreach (var item in contents)
            {
                string input = string.Join("\n", item.Value.ToArray());
                var r = new StringReader(input);

                //var deserializer = new Deserializer();
                var deserializer = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();

                var yamlObject = (Dictionary<object, object>)deserializer.Deserialize(r);
                cache[item.Key] = new StructInfo()
                {
                    lines = item.Value,
                    yamlObject = yamlObject
                };
            }

            return cache;
        }


        protected override void OpenRun(string file)
        {
            string[] inputs = File.ReadAllLines(file);

            var cache = CacheYamlObject(inputs.Skip(2).ToArray());
            foreach (var pair in cache)
            {
                var yamlObject = (Dictionary<object, object>)pair.Value.yamlObject;
                foreach (var item in yamlObject)
                {
                    if (item.Key.ToString() == "MonoBehaviour")
                    {
                        foreach (var value in (Dictionary<object, object>)item.Value)
                        {
                            if (value.Key.ToString() == "mText")
                            {
                                var key = pair.Key + "/" + item.Key + "/" + value.Key;
                                Console.WriteLine(key + ":" + value.Value);
                                if (value.Value == null) continue;
                                GetJsonValue(value.Value.ToString(), file, key, value.Value.ToString());
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
                string[] inputs = File.ReadAllLines(file);
                var cache = CacheYamlObject(inputs.Skip(2).ToArray());

                Dictionary<string, bool> cacheSave = new Dictionary<string, bool>();

                foreach (List<object> data in valuePair.Value)
                {
                    string xPath = data[Convert["行号"]].ToString();
                    string oldStr = data[Convert["原文"]].ToString();
                    string newStr = data[Convert["译文"]].ToString();

                    Queue<string> queues = new Queue<string>(xPath.Split('/'));

                    var key = queues.Peek();
                    cacheSave[key] = false; 
                    if (cache.ContainsKey(key))
                    {
                        var yamlObject = (Dictionary<object, object>)cache[queues.Dequeue()].yamlObject;
                        if (yamlObject.ContainsKey(queues.Peek()))
                        {
                            var dic = (Dictionary<object, object>)yamlObject[queues.Dequeue()];
                            if (dic.ContainsKey(queues.Peek()))
                            {
                                dic[queues.Dequeue()] = string.Format("\"{0}\"", StringHelper.StringToUnicode(newStr));
                                cacheSave[key] = true;
                            }
                        }
                    }
                }
                if (cacheSave.Values.Contains(true))
                {
                    List<string> results = inputs.Take(2).ToList();

                    //var serializer = new Serializer();
                    //var serializer = new SerializerBuilder().Build();

                    var serializer = new SerializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();
                    foreach (var pair in cache)
                    {
                        results.Add(pair.Key);
                        if (cacheSave.ContainsKey(pair.Key) && cacheSave[pair.Key])
                        {
                            var str = serializer.Serialize(pair.Value.yamlObject);
                            results.AddRange(str.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
                        }
                        else
                        {
                            results.AddRange(pair.Value.lines);
                        }
                    }

                    File.WriteAllLines(file, results.ToArray());
                }
            }
        }
    }
}
