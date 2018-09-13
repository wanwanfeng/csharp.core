using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            List<string> files = CheckPath(".json", SelectType.Folder);
            if (files.Count == 0) return;
            var res = new List<string>();
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                try
                {
                    JsonData json = JsonHelper.ToObject(File.ReadAllText(file).Trim('\0').Trim());
                    string jsonStr = JsonHelper.ToJson(json, false);
                    File.WriteAllText(file, jsonStr, new UTF8Encoding(true));
                }
                catch (Exception)
                {
                    res.Add(file);
                }
            });

            File.WriteAllLines(InputPath + ".txt", res.Select(p => p.Replace("/", "\\")).ToArray());
        }
    }
}