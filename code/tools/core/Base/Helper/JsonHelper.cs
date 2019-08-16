using System;
using System.Collections.Generic;
using System.IO;
using Library.Helper;
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
        Dictionary<string, object> ReadJsonByPathToDictionary(JsonData data, Dictionary<string, JsonData> vals);
        object ReadValueByKeyPath(JsonData data, string keyPath);
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

        public static Dictionary<string, object> ReadJsonByPathToDictionary(JsonData data, Dictionary<string, JsonData> vals)
        {
            return Helper.ReadJsonByPathToDictionary(data, vals);
        }

        public static object ReadValueByKeyPath(JsonData data, string keyPath)
        {
            return Helper.ReadValueByKeyPath(data, keyPath);
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
    public partial class ListTable
    {
        public string TableName = "";
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


        #region JsonData ListTable 显示转换（需强制）


        public static explicit operator JsonData(ListTable lt)
        {
            return JsonHelper.ConvertListTableToJson(lt);
        }

        public static explicit operator ListTable(JsonData jsonData)
        {
            return JsonHelper.ConvertJsonToListTable(JsonHelper.ToJson(jsonData));
        }

        #endregion

    }
}