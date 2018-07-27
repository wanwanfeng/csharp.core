using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static string ToJson<T>(T t)
        {
            return JsonMapper.ToJson(t);
        }

        public static List<List<object>> ConvertJsonToListByPath(string file, Func<string, string> func = null)
        {
            if (!File.Exists(file))
                Ldebug.Log("文件不存在!");
            string content = File.ReadAllText(file);
            return ConvertJsonToList(content, func);
        }

        /// <summary>
        /// 同种结构转为List数组
        /// </summary>
        /// <param name="content"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<List<object>> ConvertJsonToList(string content, Func<string, string> func = null)
        {
            if (string.IsNullOrEmpty(content))
                return new List<List<object>>();
            return ConvertJsonToList(JsonMapper.ToObject(content.Trim().Trim('\0')), func);
            //return ConvertJsonToDictionary((content.Trim().Trim('\0')));
        }

        /// <summary>
        /// 同种结构转为List数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<List<object>> ConvertJsonToList(JsonData data, Func<string, string> func = null)
        {
            if (!data.IsArray) return new List<List<object>>();
            //获取key集合
            List<string> keys = new List<string>();
            foreach (JsonData jsonData in data)
                keys.AddRange(jsonData.Keys);
            keys = keys.Distinct().ToList();

            //获取key集合对应的值集合
            var vals = new List<List<object>>() {keys.Select(p => (object) p).ToList()};
            foreach (JsonData jsonData in data)
            {
                List<object> val = new List<object>();
                foreach (var key in keys)
                {
                    var str = jsonData.Keys.Contains(key) ? jsonData[key].ToString() : "";
                    val.Add(func == null ? str : func(str));
                }
                vals.Add(val);
            }
            return vals;
        }

        public static JsonData ConvertListToJson(List<List<object>> vals)
        {
            Queue<List<object>> queue = new Queue<List<object>>(vals);
            List<object> keyList = queue.Dequeue();
            JsonData resJsonDatas = new JsonData();
            resJsonDatas.SetJsonType(JsonType.Array);
            while (queue.Count != 0)
            {
                Queue<object> queueVal = new Queue<object>(queue.Dequeue());
                JsonData jsonData = new JsonData();
                foreach (object o in keyList)
                    jsonData[o.ToString()] = queueVal.Dequeue().ToString();
                resJsonDatas.Add(jsonData);
            }
            return resJsonDatas;
        }

        /// <summary>
        /// 非同种结构转为对象
        /// </summary>
        /// <param name="content"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<List<object>> ConvertJsonToDictionary(string content, Func<string, string> func = null)
        {
            if (string.IsNullOrEmpty(content))
                return new List<List<object>>();

            JsonData data = JsonMapper.ToObject(content.Trim().Trim('\0'));
            Dictionary<string, JsonData> cache = ConvertJsonToDictionary(data);
            var list = cache.Select(p => new List<object>()
                {
                    p.Key,
                    func == null ? p.Value : func(p.Value.ToString())
                })
                .ToList();
            list.Insert(0, new List<object>() {"key", "value"});
            return list;
        }

        /// <summary>
        /// 非同种结构转为对象
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

        public static JsonData ConvertDictionaryToJson(Dictionary<string, JsonData> vals)
        {
            JsonData data = new JsonData();
            data.SetJsonType(JsonType.Object);
            foreach (KeyValuePair<string, JsonData> keyValuePair in vals)
            {
                Queue<string> keys = new Queue<string>(keyValuePair.Key.Trim('/').Split('/'));
                do
                {
                    SetValue(data, keys, keyValuePair.Value);
                } while (keys.Count != 0);
            }
            return data;
        }

        private static void SetValue(JsonData data, Queue<string> keys, JsonData value)
        {
            string key = keys.Dequeue();
            if (data.ContainsKey(key))
            {
                SetValue(data[key], keys, value);
            }
            else
            {
                if (keys.Count == 0)
                    data[key] = value;
                else
                    SetValue(data[key] = new JsonData(), keys, value);
            }
        }
    }
}
