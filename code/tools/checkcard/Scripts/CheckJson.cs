using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace checkcard.Scripts
{
    public class CheckJson : BaseSystemConsole
    {
        public CheckJson()
        {
            Read();
        }

        public void Read()
        {
            var res = new List<string>();
            Action<string> action = file =>
            {
                Console.WriteLine(" is now : " + file);
                try
                {
                    JsonData json = JsonHelper.ToObject(File.ReadAllText(file).Trim('\0').Trim());
                    string jsonStr = JsonHelper.ToJson(json, false);
                    //File.WriteAllText(file, jsonStr, new UTF8Encoding(true));
                }
                catch (Exception)
                {
                    res.Add(file);
                }
            };
            //Parallel.ForEach(CheckPath(".json", SelectType.Folder), action);//并行操作
            CheckPath(".json", SelectType.Folder).AsParallel().ForAll(action);//并行操作
            //CheckPath(".json", SelectType.Folder).ForEach(action);//线性操作

            File.WriteAllLines(InputPath + ".txt", res.Select(p => p.Replace("/", "\\")).ToArray());
        }
    }
}