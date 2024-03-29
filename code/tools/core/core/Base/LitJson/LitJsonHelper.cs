﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;
using PLitJson;

namespace Library.LitJson
{
    public partial class LitJsonHelper : IJsonHelper
    {
        static LitJsonHelper()
        {
            JsonMapper.RegisterImporter<double, float>(input => (float) input);
            JsonMapper.RegisterExporter<float>((v, w) =>
            {
                w.Write(v);
            });
            StaticConstructor();
        }

        static partial void StaticConstructor();

        public T ToObject<T>(string res) => JsonMapper.ToObject<T>(res);

        public T ToObject<T>(TextReader reader) => JsonMapper.ToObject<T>(reader);

        public JsonData ToObject(string res) => JsonMapper.ToObject(res);

        public JsonData ToObject(TextReader reader) => JsonMapper.ToObject(reader);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="validate">错误校验输出</param>
        /// <returns></returns>
        public string ToJson<T>(T t, int indentLevel = 0, bool validate = true)
        {
            if (indentLevel == 0 && validate)
                return JsonMapper.ToJson(t);

            StringWriter sw = new StringWriter();
            JsonWriter jsonWriter = new JsonWriter(sw)
            {
                PrettyPrint = true,
                IndentValue = indentLevel,
                Validate = validate,
            };
            JsonMapper.ToJson(t, jsonWriter);
            return sw.ToString().Trim('\r', '\n');
        }

        /// <summary>
        ///  同种结构JsonArray转为List数组,非同种结构JsonObject转为数组
        /// </summary>
        /// <param name="content"></param>
        /// <param name="func"></param>
        /// <returns>返回的是两种不同的List</returns>
        public ListTable ConvertJsonToListTable(string content, Func<object, object> func = null)
        {
            if (string.IsNullOrEmpty(content))
                return new ListTable();

            JsonData data = JsonMapper.ToObject(content.Trim().Trim('\0'));
            if (data.IsArray)
            {
                //获取key集合
                List<string> keys = new List<string>();
                foreach (JsonData jsonData in data)
                    keys.AddRange(jsonData.Keys);
                keys = keys.Distinct().ToList();

                //获取key集合对应的值集合
                var list = new List<List<object>>();
                foreach (JsonData jsonData in data)
                {
                    List<object> val = new List<object>();
                    foreach (var key in keys)
                    {
                        var str = jsonData.Keys.Contains(key) ? jsonData[key] : null;
                        val.Add(func == null ? str : func(str));
                    }
                    list.Add(val);
                }
                return new ListTable {Columns = keys, Rows = list};
            }
            if (data.IsObject)
            {
                Dictionary<string, JsonData> cache = ConvertJsonToDictionary(data);
                var list = cache.Select(p => new List<object>()
                {
                    p.Key,
                    func == null ? p.Value : func(p.Value)
                }).ToList();
                return new ListTable {Columns = new List<string> {"key", "value"}, Rows = list};
            }
            return new ListTable();
        }

        /// <summary>
        ///  同种结构转为Array
        /// </summary>
        /// <returns></returns>
        public JsonData ConvertListTableToJson(ListTable list)
        {
            JsonData resJsonDatas = new JsonData();
            resJsonDatas.SetJsonType(JsonType.Array);

            foreach (List<object> objects in list.Rows)
            {
                Queue<object> queueVal = new Queue<object>(objects);
                JsonData jsonData = new JsonData();
                foreach (string o in list.Columns)
                {
                    var val = queueVal.Dequeue();
                    jsonData[o] = (val is JsonData) ? (JsonData) val : new JsonData(val);
                }

                resJsonDatas.Add(jsonData);
            }

            return resJsonDatas;
        }

        /// <summary>
        /// 非同种结构转为对象
        /// 叶子结点相对于根节点的路径为key
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Dictionary<string, JsonData> ConvertJsonToDictionary(JsonData data)
        {
            if (!data.IsObject) return new Dictionary<string, JsonData>();
            Dictionary<string, JsonData> cache = new Dictionary<string, JsonData>();
            GetKeyValue(cache, data);
            return cache;
        }

        private void GetKeyValue(Dictionary<string, JsonData> cache, JsonData data, string root = "")
        {
            if (data == null) return;
            if (data.IsArray)
            {
                int index = 0;
                foreach (JsonData json in data)
                {
                    GetKeyValue(cache, json, root + "/" + (index++));
                }
                return;
            }
            if (data.IsObject)
            {
                foreach (string jsonKey in data.Keys)
                {
                    GetKeyValue(cache, data[jsonKey], root + "/" + jsonKey);
                }
                return;
            }
            cache.Add(root, data);
        }

        /// <summary>
        /// data值再覆盖(数组索引作为key)
        /// </summary>
        /// <param name="data">修改前的data</param>
        /// <param name="vals"></param>
        /// <returns>返回修改后的data</returns>
        public JsonData RevertDictionaryToJson(JsonData data, Dictionary<string, JsonData> vals)
        {
            foreach (KeyValuePair<string, JsonData> keyValuePair in vals)
            {
                Queue<string> keys = new Queue<string>(keyValuePair.Key.Trim('/').Split('/'));
                JsonData temp = data;
                do
                {
                    var key = keys.Dequeue();
                    if (keys.Count == 0)
                    {
                        if (temp.IsObject && temp.ContainsKey(key))
                            temp[key] = keyValuePair.Value;
                        else if (temp.IsArray)
                            temp[key.AsInt()] = keyValuePair.Value;
                        else
                        {

                        }
                    }
                    else
                    {
                        if (temp.IsObject && temp.ContainsKey(key))
                            temp = temp[key];
                        else if (temp.IsArray)
                            temp = temp[key.AsInt()];
                        else
                        {

                        }
                    }
                } while (keys.Count != 0);
            }
            return data;
        }

        /// <summary>
        /// 按路径读取data值(数组索引作为key)
        /// </summary>
        /// <param name="data">原始data</param>
        /// <param name="vals"></param>
        public Dictionary<string, object> ReadJsonByPathToDictionary(JsonData data, Dictionary<string, JsonData> vals)
        {
            Dictionary<string, object> cache = new Dictionary<string, object>();
            foreach (KeyValuePair<string, JsonData> keyValuePair in vals)
            {
                Queue<string> keys = new Queue<string>(keyValuePair.Key.Trim('/').Split('/'));
                JsonData temp = data;
                do
                {
                    var key = keys.Dequeue();
                    if (keys.Count == 0)
                    {
                        if (temp.IsObject && temp.ContainsKey(key))
                            cache[keyValuePair.Key] = temp[key]; //= keyValuePair.Value;
                        else if (temp.IsArray)
                            cache[keyValuePair.Key] = temp[key.AsInt()]; // = keyValuePair.Value;
                        else
                        {

                        }
                    }
                    else
                    {
                        if (temp.IsObject && temp.ContainsKey(key))
                            temp = temp[key];
                        else if (temp.IsArray)
                            temp = temp[key.AsInt()];
                        else
                        {

                        }
                    }
                } while (keys.Count != 0);
            }
            return cache;
        }

        public object ReadValueByKeyPath(JsonData data, string keyPath)
        {
            Queue<string> keys = new Queue<string>(keyPath.Trim('/').Split('/'));
            object obj = null;
            JsonData temp = data;
            do
            {
                var key = keys.Dequeue();
                if (keys.Count == 0)
                {
                    if (temp.IsObject && temp.ContainsKey(key))
                        obj = temp[key];
                    else if (temp.IsArray)
                        obj = temp[key.AsInt()];
                    else
                    {

                    }
                }
                else
                {
                    if (temp.IsObject && temp.ContainsKey(key))
                        temp = temp[key];
                    else if (temp.IsArray)
                        temp = temp[key.AsInt()];
                    else
                    {

                    }
                }
            } while (keys.Count != 0);
            return obj;
        }
    }
}
