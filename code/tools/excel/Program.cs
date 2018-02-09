using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Excel;
using Library.Helper;
using LitJson;

namespace excel
{
    public enum CaoType
    {
        JsonToCsv = 1,
        JsonToExcel = 2,
        JsonToOneExcel = 3,
        XlsxToJson = 4,
    }


    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("----------命令索引----------");
            foreach (var value in Enum.GetValues(typeof(CaoType)))
            {
                Console.WriteLine("\t" + (int) value + "：" + value);
            }
            Console.WriteLine("----------------------------");
            Console.Write("请选择，然后回车：");
            string s = Console.ReadLine();
            if (s != null)
            {
                CaoType caoType = (CaoType) Enum.Parse(typeof (CaoType), s);
                switch (caoType)
                {
                    case CaoType.JsonToCsv:
                        ReadjsonToCsv();
                        break;
                    case CaoType.JsonToExcel:
                        ReadJsonToExcel();
                        break;
                    case CaoType.JsonToOneExcel:
                        ReadJsonToOneExcel();
                        break;
                    case CaoType.XlsxToJson:
                        ReadExcelToJson();
                        break;
                    default:
                        Console.Write("不存在的命令！");
                        break;
                }
            }
            GC.Collect();
            Console.Write("请按任意键退出...");
            Console.ReadKey();
        }

        #region Json->

        /// <summary>
        /// json->csv
        /// </summary>
        private static void ReadjsonToCsv()
        {
            List<string> files;
            if (!CheckPath(out files))
                return;
            foreach (var file in files)
            {
                Console.WriteLine(" is now : " + file);
                List<List<object>> vals = GetJsonDataArray(File.ReadAllText(file));
                List<string> res = new List<string>();
                foreach (List<object> objects in vals)
                {
                    res.Add(string.Join(",", objects.Select(p => "\"" + p + "\"").ToArray()));
                }
                File.WriteAllLines(Path.ChangeExtension(file, ".csv"), res.ToArray(), Encoding.UTF8);
            }
        }

        /// <summary>
        /// json->xlsx
        /// </summary>
        private static void ReadJsonToExcel()
        {
            List<string> files;
            if (!CheckPath(out files)) return;
            foreach (string file in files)
            {
                Console.WriteLine(" is now : " + file);
                List<List<object>> vals = GetJsonDataArray(File.ReadAllText(file));
                //StreamExportExcel.WriteToExcel(Path.ChangeExtension(file, ".xls"), vals);
                OfficeWorkbooks.WriteToExcel(Path.ChangeExtension(file, ".xlsx"), vals);
            }
        }

        /// <summary>
        /// / json->xlsx
        /// </summary>
        private static void ReadJsonToOneExcel()
        {
            List<string> files;
            if (!CheckPath(out files)) return;
            Dictionary<string, List<List<object>>> dic = new Dictionary<string, List<List<object>>>();
            foreach (string file in files)
            {
                Console.WriteLine(" is now : " + file);
                dic[file] = GetJsonDataArray(File.ReadAllText(file));
            }
            OfficeWorkbooks.WriteToExcelOne(dic);
        }

        private static bool CheckPath(out List<string> files, string exce = ".json")
        {
            //OpenFileDialog fd = new OpenFileDialog();
            //fd.Filter = "EXCEL文件(*.xls)|*.xls|EXCEL文件(*.xlsx)|*.xlsx";

            //if (fd.ShowDialog() == DialogResult.OK)
            //{
            //    //这里面就可以对选择的文件进行处理了
            //}

            string path = (Environment.CurrentDirectory + "/json/").Replace("\\", "/");
            if (!Directory.Exists(path))
            {
                files = new List<string>();
                Console.WriteLine(path + " is not exists !");
                return false;
            }
            files = Directory.GetFiles(path).Where(p => p.EndsWith(exce)).ToList();
            files.Sort();
            return true;
        }

        public static List<List<object>> GetJsonDataArray(string content)
        {
            JsonData[] jsonDatas = JsonMapper.ToObject<JsonData[]>(content.Trim().Trim('\0'));
            //获取key集合
            List<string> keys = new List<string>();
            foreach (JsonData jsonData in jsonDatas)
            {
                foreach (var pair in jsonData.Inst_Object)
                {
                    if (keys.Contains(pair.Key))
                        continue;
                    keys.Add(pair.Key);
                }
            }
            //获取key集合对应的值集合
            var vals = new List<List<object>>();
            foreach (JsonData jsonData in jsonDatas)
            {
                List<object> val = new List<object>();
                vals.Add(val);

                foreach (var key in keys)
                {
                    JsonData value;
                    var str = jsonData.Inst_Object.TryGetValue(key, out value) ? value.ToString() : "";
                    val.Add(str.Replace(":", "::").Replace("\n", "\\n"));
                }
            }
            vals.Insert(0, keys.Select(p => (object) p).ToList());
            return vals;
        }

        #endregion

        #region Excel->

        /// <summary>
        /// xlsx->json
        /// </summary>
        private static void ReadExcelToJson()
        {
            List<string> files;
            if (!CheckPath(out files, "xlsx"))
                return;
            foreach (var file in files)
            {
                Console.WriteLine(" is now : " + file);
                Dictionary<string, List<List<object>>> vals = OfficeWorkbooks.ReadFromExcel(Path.ChangeExtension(file, ".xlsx"));

                //Dictionary<string, List<List<object>>> vals = EditorExcelTools.CacheDictionary.ReadFromExcel(Path.ChangeExtension(file, ".xlsx"));
                foreach (KeyValuePair<string, List<List<object>>> keyValuePair in vals)
                {
                    Console.WriteLine(" is now sheet: " + keyValuePair.Key);
                    Queue<List<object>> queue = new Queue<List<object>>(keyValuePair.Value);
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
                    string newPath = Path.ChangeExtension(file, ".json");
                    if (vals.Count > 1)
                    {
                        newPath = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) +
                                         "\\" + keyValuePair.Key + ".json";
                        FileHelper.CreateDirectory(newPath);
                    }
                    File.WriteAllText(newPath, JsonMapper.ToJson(resJsonDatas), new UTF8Encoding(false));
                }
            }
        }

        #endregion
    }
}
