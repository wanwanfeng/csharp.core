using System;
using System.Collections.Generic;
using System.IO;
using Library.LitJson;
using LitJson;

namespace Library.Helper
{
    public interface IJsonHelper
    {
        T ToObject<T>(string res);
        JsonData ToObject(string res);
        string ToJson<T>(T t, int indentLevel = 0, bool validate = true);
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="isUnicode">需要转义</param>
        /// <param name="indentLevel">缩进</param>
        /// <param name="validate">错误校验输出</param>
        /// <returns></returns>
        public static string ToJson<T>(T t, bool isUnicode = false, int indentLevel = 0, bool validate = true)
        {
            var value = Helper.ToJson(t, indentLevel, validate);
            return (isUnicode ? value : StringHelper.Unicode2String(value)).Replace(@"\r\n", @"\n");
        }

        public static JsonData ConvertListTableToJson(ListTable list)
        {
            return Helper.ConvertListTableToJson(list);
        }

        public static JsonData RevertDictionaryToJson(JsonData data, Dictionary<string, JsonData> vals)
        {
            return Helper.RevertDictionaryToJson(data, vals);
        }

        public static ListTable ConvertJsonToListTable(string content, Func<object, object> func = null)
        {
            return Helper.ConvertJsonToListTable(content, func);
        }

        public static ListTable ImportJsonToListTable(string file, Func<object, object> func = null)
        {
            if (!File.Exists(file))
                Ldebug.Log("文件不存在!");
            string content = File.ReadAllText(file);
            ListTable listTable = ConvertJsonToListTable(content, func);
            listTable.TableName = Path.GetFileName(file);
            listTable.FullName = file;
            return listTable;
        }

        public static T ImportJson<T>(string file)
        {
            if (!File.Exists(file))
                Ldebug.Log("文件不存在!");
            string content = File.ReadAllText(file);
            return ToObject<T>(content.Trim('\0'));
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
        /// <summary>
        /// key列表
        /// </summary>
        public List<string> Columns = new List<string>();
        /// <summary>
        /// 值列表
        /// </summary>
        public List<List<object>> Rows = new List<List<object>>();

        public object this[int hang, int lie]
        {
            get { return Rows[hang][lie]; }
        }

        public object this[int hang, string key]
        {
            get { return Rows[hang][Columns.IndexOf(key)]; }
        }
    }
}