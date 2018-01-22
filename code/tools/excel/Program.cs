using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Excel;
using LitJson;

namespace excel
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Haha(Environment.CurrentDirectory + "/asasa.xlsx", 1);
            ////new Haha("fa.xlsx", 1);


            //ReadjsonToCsv();
            ReadjsonToExcelS();
            Console.ReadKey();
        }

        private static void ReadjsonToCsv()
        {
            List<string> files;
            if (!CheckPath(out files))
                return;
            foreach (var file in files)
            {
                string content = File.ReadAllText(file).Trim();
                //Console.WriteLine(file + "\n" + content);
                List<string> res = new List<string>();
                JsonData[] jsonDatas = JsonMapper.ToObject<JsonData[]>(content);
                Console.WriteLine(" is now : " + file);

                //获取key集合
                List<string> keys = new List<string>();
                foreach (JsonData jsonData in jsonDatas)
                {
                    foreach (var pair in jsonData.Inst_Object)
                    {
                        if (keys.Contains(pair.Key))
                        {
                            continue;
                        }
                        keys.Add(pair.Key);
                    }
                }
                keys.Sort();
                //获取key集合对应的值集合
                foreach (JsonData jsonData in jsonDatas)
                {
                    var ss = "";

                    foreach (var key in keys)
                    {
                        JsonData value;
                        if (jsonData.Inst_Object.TryGetValue(key, out value))
                        {
                            if (value.ToString().Contains(","))
                            {
                                ss += ",\"" + value + "\"";
                            }
                            else
                            {
                                ss += "," + value;
                            }
                        }
                        else
                        {
                            ss += ",";
                        }
                    }
                    res.Add(ss.Substring(1));
                }
                res.Insert(0, string.Join(",", keys.ToArray()));
                File.WriteAllLines(Path.ChangeExtension(file, ".csv"), res, Encoding.UTF8);
            }
        }

        private static void ReadjsonToExcelS()
        {
            List<string> files;
            if (!CheckPath(out files)) return;


            foreach (string file in files)
            {
                ReadjsonToExcel(file);
            }

            return;

            var tes =
                files.ToLookup((p) => files.IndexOf(p)%5).ToDictionary(p => p.Key, q => new List<string>(q)).ToList();

            foreach (KeyValuePair<int, List<string>> keyValuePair in tes)
            {
                ThreadPoolHaHa(new Queue<string>(keyValuePair.Value));
            }
        }


        private static void ThreadPoolHaHa(Queue<string> queue)
        {
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                if (queue.Count != 0)
                    ReadjsonToExcel(queue.Dequeue());
                if (queue.Count != 0)
                    ThreadPoolHaHa(queue);
            });
        }

        private static void ReadjsonToExcel(string file)
        {
            string content = File.ReadAllText(file).Trim().Trim('\0');
            //Console.WriteLine(file + "\n" + content);
            List<string> res = new List<string>();
            JsonData[] jsonDatas = JsonMapper.ToObject<JsonData[]>(content);
            Console.WriteLine(" is now : " + file);

            //获取key集合
            List<string> keys = new List<string>();
            foreach (JsonData jsonData in jsonDatas)
            {
                foreach (var pair in jsonData.Inst_Object)
                {
                    if (keys.Contains(pair.Key))
                    {
                        continue;
                    }
                    keys.Add(pair.Key);
                }
            }
            keys.Sort();
            //获取key集合对应的值集合
            List<List<object>> vals = new List<List<object>>();
            foreach (JsonData jsonData in jsonDatas)
            {
                List<object> val = new List<object>();
                vals.Add(val);

                foreach (var key in keys)
                {
                    JsonData value;
                    val.Add(jsonData.Inst_Object.TryGetValue(key, out value) ? value.ToString() : "");
                }
            }

            new Haha(Path.ChangeExtension(file, ".xlsx"), keys, vals);

            //EditorExcelTools.MS_ExportExcel(Path.ChangeExtension(file, ".xls"), keys, vals);
        }

        private static bool CheckPath(out List<string> files)
        {
            string path = (Environment.CurrentDirectory + "/json/").Replace("\\", "/");
            if (!Directory.Exists(path))
            {
                files = new List<string>();
                Console.WriteLine(path + " is not exists !");
                return false;
            }
            files = Directory.GetFiles(path).Where(p => p.EndsWith(".json")).ToList();
            files.Sort();
            return true;
        }
    }
}
