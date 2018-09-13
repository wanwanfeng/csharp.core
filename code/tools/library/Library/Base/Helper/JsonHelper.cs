using System;
using System.Collections.Generic;
using Library.LitJson;
using JsonData = LitJson.JsonData;

namespace Library.Helper
{
    public interface IJsonHelper
    {
        T ToObject<T>(string res);
        JsonData ToObject(string res);
        string ToJson<T>(T t);
        ListTable ImportJsonToListTable(string file, Func<object, object> func = null);
        ListTable ConvertJsonToListTable(string content, Func<object, object> func = null);
        JsonData ConvertListTableToJson(ListTable list);
        JsonData RevertDictionaryToJson(JsonData data, Dictionary<string, JsonData> vals);
    }

    public class JsonHelper
    {
        static JsonHelper()
        {
            SetJsonHelper(new LitJsonHelper());
        }

        private static IJsonHelper Helper { get; set; }

        public static void SetJsonHelper(IJsonHelper jsonHelper)
        {
            Helper = jsonHelper;
        }

        public static T ToObject<T>(string res)
        {
            return Helper.ToObject<T>(res);
        }

        public static JsonData ToObject(string res)
        {
            return Helper.ToObject(res);
        }

        public static string ToJson<T>(T t, bool isUnicode = false)
        {
            var value = Helper.ToJson(t);
            return isUnicode ? value : StringHelper.Unicode2String(value);
        }

        public static ListTable ImportJsonToListTable(string file, Func<object, object> func = null)
        {
            return Helper.ImportJsonToListTable(file, func);
        }

        public static ListTable ConvertJsonToListTable(string content, Func<object, object> func = null)
        {
            return Helper.ConvertJsonToListTable(content, func);
        }

        public static JsonData ConvertListTableToJson(ListTable list)
        {
            return Helper.ConvertListTableToJson(list);
        }

        public static JsonData RevertDictionaryToJson(JsonData data, Dictionary<string, JsonData> vals)
        {
            return Helper.RevertDictionaryToJson(data, vals);
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

        public object this[int hang, int lie]
        {
            get { return List[hang][lie]; }
        }

        public object this[int hang, string key]
        {
            get { return List[hang][Key.IndexOf(key)]; }
        }

    }
}