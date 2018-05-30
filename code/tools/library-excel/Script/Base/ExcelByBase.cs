using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Library.Helper;
using Library.LitJson;
using LitJson;

namespace Library.Excel
{
    public abstract partial class ExcelByBase
    {
        #region  Convert List<List<object>> and Json

        public static List<List<object>> ConvertJsonToListByPath(string file)
        {
            return LitJsonHelper.ConvertJsonToListByPath(file);
        }

        public static List<List<object>> ConvertJsonToList(string content)
        {
            return LitJsonHelper.ConvertJsonToList(content);
        }

        public static JsonData ConvertListToJson(KeyValuePair<string, List<List<object>>> keyValuePair)
        {
            Ldebug.Log(" is now sheet: " + keyValuePair.Key);
            return LitJsonHelper.ConvertListToJson(keyValuePair.Value);
        }

        public static void ConvertListToJsonFile(KeyValuePair<string, List<List<object>>> keyValuePair, string file)
        {
            JsonData resJsonDatas = ConvertListToJson(keyValuePair);
            string newPath = Path.ChangeExtension(string.IsNullOrEmpty(file) ? keyValuePair.Key : file, ".json");
            FileHelper.CreateDirectory(newPath);
            File.WriteAllText(newPath, JsonMapper.ToJson(resJsonDatas), new UTF8Encoding(false));
        }

        #endregion

        public void HaHa(DataTable dt)
        {
            var headers = GetHeaderList(dt);

            DataTable dataTable = dt.Clone();
            //dataTable.DefaultView.ToTable()
            string regex = "";

            foreach (string header in headers)
            {
                //List<object> vals = new List<object>();
                //foreach (DataRow dr in dt.Rows)
                //{
                //    vals.Add(dr[header]);
                //}
                //var content = string.Join(",", vals.Select(p => p.ToString()).ToArray());
                //if (Regex.IsMatch(content, regex))
                //{

                //}

                foreach (DataRow dr in dt.Rows)
                {
                    var content = dr[header].ToString();
                    if (Regex.IsMatch(content, regex))
                    {
                        dr.Delete();
                        break;
                    }
                }
            }
            dataTable.AcceptChanges();
        }
    }
}