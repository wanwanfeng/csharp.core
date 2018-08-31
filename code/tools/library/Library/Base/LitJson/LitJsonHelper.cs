using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Extensions;
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
    }
}

namespace Library
{
    [Serializable]
    public class ListTable
    {
        public string TableName = "";
        public string FullName = "";
        public bool IsArray = true;
        public List<string> Key = new List<string>();
        public List<List<object>> List = new List<List<object>>();
    }
}

namespace Library.LitJson
{
    public class LitJsonHelper
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

        public static T ToObject<T>(string res)
        {
            return JsonMapper.ToObject<T>(res);
        }

        public static JsonData ToObject(string res)
        {
            return JsonMapper.ToObject(res);
        }

        public static string ToJson<T>(T t, bool unescape = false)
        {
            //不转义中文字符
            return unescape ? Regex.Unescape(JsonMapper.ToJson(t)) : JsonMapper.ToJson(t);
        }

        public static ListTable ImportJsonToListTable(string file, Func<string, string> func = null)
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
        public static ListTable ConvertJsonToListTable(string content, Func<string, string> func = null)
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
                        var str = jsonData.Keys.Contains(key) ? jsonData[key].ToString() : "";
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
                        func == null ? p.Value : func(p.Value.ToString())
                    }).ToList();
                return new ListTable { IsArray = false, Key = new List<string> { "key", "value" }, List = list };
            }
            return new ListTable();
        }

        /// <summary>
        ///  同种结构转为Array
        /// </summary>
        /// <returns></returns>
        public static JsonData ConvertListTableToJson(ListTable list)
        {
            if (list.IsArray)
            {
                JsonData resJsonDatas = new JsonData();
                resJsonDatas.SetJsonType(JsonType.Array);

                foreach (List<object> objects in list.List)
                {
                    Queue<object> queueVal = new Queue<object>(objects);
                    JsonData jsonData = new JsonData();
                    foreach (string o in list.Key)
                        jsonData[o] = queueVal.Dequeue().ToString();
                    resJsonDatas.Add(jsonData);
                }

                return resJsonDatas;
            }
            else
            {
                //todo
                JsonData resJsonDatas = new JsonData();
                resJsonDatas.SetJsonType(JsonType.Array);

                foreach (List<object> objects in list.List)
                {
                    Queue<object> queueVal = new Queue<object>(objects);
                    JsonData jsonData = new JsonData();
                    foreach (string o in list.Key)
                        jsonData[o] = queueVal.Dequeue().ToString();
                    resJsonDatas.Add(jsonData);
                }

                return resJsonDatas;
            }
        }

        /// <summary>
        /// 非同种结构转为对象
        /// 叶子结点相对于根节点的路径为key
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, JsonData> ConvertJsonToDictionary(JsonData data)
        {
            if (!data.IsObject) return new Dictionary<string, JsonData>();
            Dictionary<string, JsonData> cache = new Dictionary<string, JsonData>();
            GetKeyValue(cache, data);
            return cache;
        }

        private static void GetKeyValue(Dictionary<string, JsonData> cache, JsonData data, string root = "")
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
        public static JsonData RevertDictionaryToJson(JsonData data, Dictionary<string, JsonData> vals)
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
        /// data值再覆盖(数组索引作为key)
        /// </summary>
        /// <param name="data">修改前的data</param>
        /// <param name="vals"></param>
        /// <returns>返回修改后的data</returns>
        public static JsonData RevertDictionaryToJson2(JsonData data, Dictionary<string, JsonData> vals)
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
                            temp = new JsonData();
                    }
                    else
                    {
                        if (temp.IsObject && temp.ContainsKey(key))
                            temp = temp[key];
                        else if (temp.IsArray)
                            temp = temp[key.AsInt()];
                        else
                            temp = new JsonData();
                    }
                } while (keys.Count != 0);
            }
            return data;
        }
    }
}
