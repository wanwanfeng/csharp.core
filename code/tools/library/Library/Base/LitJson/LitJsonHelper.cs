using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace Library.LitJson
{
    public class LitJsonHelper
    {
        static LitJsonHelper()
        {
            JsonMapper.RegisterImporter<double, float>(input => (float)input);
            JsonMapper.RegisterImporter<int, long>(input => (long)input);
            JsonMapper.RegisterExporter<float>((v, w) =>
            {
                w.Write(v);
            });
            JsonMapper.RegisterExporter<long>((v, w) =>
            {
                w.Write(v);
            });
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

        public static List<List<object>> ConvertJsonToList(string content, Func<string, string> func = null)
        {
            JsonData[] jsonDatas = JsonMapper.ToObject<JsonData[]>(content.Trim().Trim('\0'));

            //获取key集合
            List<string> keys = new List<string>();
            foreach (JsonData jsonData in jsonDatas)
                keys.AddRange(jsonData.Keys);
            keys = keys.Distinct().ToList();

            //获取key集合对应的值集合
            var vals = new List<List<object>>() { keys.Select(p => (object)p).ToList() };
            foreach (JsonData jsonData in jsonDatas)
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
    }
}
