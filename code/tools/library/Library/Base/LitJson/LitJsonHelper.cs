using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace LitJson
{
    public partial class JsonData
    {
        public int ToInt()
        {
            switch (type)
            {
                case JsonType.Int:
                    return inst_int;
                case JsonType.Double:
                    return Convert.ToInt32(inst_double);
                case JsonType.Long:
                    return Convert.ToInt32(inst_long);
                case JsonType.String:
                    return Convert.ToInt32(inst_string);
                case JsonType.Boolean:
                    return Convert.ToInt32(inst_boolean);
            }
            throw new Exception("JsonData ToInt error");
        }

        public long ToLong()
        {
            switch (type)
            {
                case JsonType.Int:
                    return Convert.ToInt64(inst_int);
                case JsonType.Double:
                    return Convert.ToInt64(inst_double);
                case JsonType.Long:
                    return inst_long;
                case JsonType.String:
                    return Convert.ToInt64(inst_string);
                case JsonType.Boolean:
                    return Convert.ToInt64(inst_boolean);
            }
            throw new Exception("JsonData ToLong error");
        }

        public double ToDouble()
        {
            switch (type)
            {
                case JsonType.Int:
                    return Convert.ToDouble(inst_int);
                case JsonType.Double:
                    return inst_double;
                case JsonType.Long:
                    return Convert.ToDouble(inst_long);
                case JsonType.String:
                    return Convert.ToDouble(inst_string);
                case JsonType.Boolean:
                    return Convert.ToDouble(inst_boolean);
            }
            throw new Exception("JsonData ToDouble error");
        }

        public bool ToBoolean()
        {
            switch (type)
            {
                case JsonType.Int:
                    return Convert.ToBoolean(inst_int);
                case JsonType.Double:
                    return Convert.ToBoolean(inst_double);
                case JsonType.Long:
                    return Convert.ToBoolean(inst_long);
                case JsonType.String:
                    return Convert.ToBoolean(inst_string);
                case JsonType.Boolean:
                    return inst_boolean;
            }
            throw new Exception("JsonData ToBoolean error");
        }

        public JsonData Last()
        {
            if (IsArray)
                return inst_array.LastOrDefault();
            if (IsObject)
                return inst_object.Values.LastOrDefault();
            return this;
        }

        public JsonData First()
        {
            if (IsArray)
                return inst_array.FirstOrDefault();
            if (IsObject)
                return inst_object.Values.First();
            return this;
        }

        public void Remove(JsonData remove)
        {
            if (IsArray)
                inst_array.Remove(remove);
        }

        public void Remove(string key)
        {
            if (!IsObject) return;
            if (Count == 0) return;
            inst_object.Remove(key);
        }

        public void RemoveAt(int index)
        {
            if (!IsArray) return;
            if (Count == 0) return;
            if (Count <= index) return;
            inst_array.RemoveAt(index);
        }

        public void Remove(params string[] keys)
        {
            if (!IsObject) return;
            if (Count == 0) return;
            if (keys.Length == 0) return;
            if (!Keys.Except(keys).Any()) return;
            keys.ToList().ForEach(p => { inst_object.Remove(p); });
        }

        public void RemoveAt(params int[] indexs)
        {
            if (!IsArray) return;
            if (Count == 0) return;
            if (indexs.Length == 0) return;
            indexs = indexs.Where(p => p < Count).ToArray();
            if (indexs.Length == 0) return;
            for (int i = indexs.Length - 1; i >= 0; i--)
                inst_array.RemoveAt(i);
        }
    }
}

namespace Library.LitJson
{
    public class LitJsonHelper : IJsonHelper
    {
        static LitJsonHelper()
        {
            JsonMapper.RegisterImporter<double, float>(input => (float) input);
            //JsonMapper.RegisterImporter<int, long>(input => (long)input);
            JsonMapper.RegisterExporter<float>((v, w) =>
            {
                w.Write(v);
            });
            //JsonMapper.RegisterExporter<long>((v, w) =>
            //{
            //    w.Write(v);
            //});
        }

        public T ToObject<T>(string res)
        {
            return JsonMapper.ToObject<T>(res);
        }

        public JsonData ToObject(string res)
        {
            return JsonMapper.ToObject(res);
        }

        public string ToJson<T>(T t)
        {
            return JsonMapper.ToJson(t);
        }

        public ListTable ImportJsonToListTable(string file, Func<object, object> func = null)
        {
            if (!File.Exists(file))
                Ldebug.Log("文件不存在!");
            string content = File.ReadAllText(file);
            ListTable listTable = ConvertJsonToListTable(content, func);
            listTable.TableName = Path.GetFileName(file);
            listTable.FullName = file;
            return listTable;
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
                return new ListTable {IsArray = true, Key = keys, List = list};
            }
            if (data.IsObject)
            {
                Dictionary<string, JsonData> cache = ConvertJsonToDictionary(data);
                var list = cache.Select(p => new List<object>()
                    {
                        p.Key,
                        func == null ? p.Value : func(p.Value)
                    }).ToList();
                return new ListTable { IsArray = false, Key = new List<string> { "key", "value" }, List = list };
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

            foreach (List<object> objects in list.List)
            {
                Queue<object> queueVal = new Queue<object>(objects);
                JsonData jsonData = new JsonData();
                foreach (string o in list.Key)
                {
                    var val = queueVal.Dequeue();
                    if (val is JsonData)
                    {
                        jsonData[o] = (JsonData) val;
                        continue;
                    }
                    jsonData[o] = new JsonData(val);
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
            }
            else if (data.IsObject)
            {
                foreach (string jsonKey in data.Keys)
                {
                    GetKeyValue(cache, data[jsonKey], root + "/" + jsonKey);
                }
            }
            else
            {
                cache.Add(root, data);
            }
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
    }
}
