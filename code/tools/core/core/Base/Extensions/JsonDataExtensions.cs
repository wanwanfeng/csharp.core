using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PLitJson
{
    public static partial class JsonDataExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="validate">错误校验输出</param>
        /// <returns></returns>
        public static string ToJson(this JsonData jsonData, int indentLevel = 0, bool validate = true)
        {
            if (indentLevel == 0 && validate)
                return jsonData.ToJson();

            StringWriter sw = new StringWriter();
            JsonWriter jsonWriter = new JsonWriter(sw)
            {
                PrettyPrint = true,
                IndentValue = indentLevel,
                Validate = validate,
            };
            jsonData.ToJson(jsonWriter);
            return sw.ToString().Trim('\r', '\n');
        }

        /// <summary>
        /// 移除key
        /// </summary>
        /// <param name="json"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static JsonData Remove(this JsonData json, string key)
        {
            if (json.IsObject)
            {
                JsonData jsonData = new JsonData();
                foreach (var item in json.Keys)
                {
                    if (item == key) continue;
                    jsonData[item] = json[item];
                }
                return jsonData;
            }
            throw new Exception("this json is not dict !");
        }
    }
}