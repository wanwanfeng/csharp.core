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

        public static ListTable ConvertJsonToList(string content)
        {
            return LitJsonHelper.ConvertJsonToList(content);
        }

        public static JsonData ConvertListToJson(ListTable list)
        {
            Ldebug.Log(" is now sheet: " + list.TableName);
            return LitJsonHelper.ConvertListToJson(list);
        }

        public static ListTable ImportJsonToList(string file)
        {
            return LitJsonHelper.ImportJsonToList(file);
        }

        public static void ExportListToJson(ListTable list, string file)
        {
            JsonData resJsonDatas = ConvertListToJson(list);
            string newPath = Path.ChangeExtension(string.IsNullOrEmpty(file) ? list.FullName : file, ".json");
            FileHelper.CreateDirectory(newPath);
            File.WriteAllText(newPath, JsonMapper.ToJson(resJsonDatas), new UTF8Encoding(false));
        }

        #endregion

        public void HaHa(DataTable dt)
        {
            var headers = GetHeaderList(dt);

            //DataTable dataTable = (DataTable) dt.Clone();
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
            //dataTable.AcceptChanges();
        }
    }
}