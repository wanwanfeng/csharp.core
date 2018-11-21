using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;

namespace fontConvert
{
    public abstract class BaseActionBy
    {
        protected string inputPath = "";
        protected List<string> all = new List<string>();

        public void Open(string input, string ex)
        {
            inputPath = input;

            if (!Directory.Exists(inputPath))
            {
                Console.WriteLine("文件夹路径不存在!");
            }
            else
            {
                var exName = ex.SplitString('|', ',').Select(p => p.StartsWith(".") ? p : "." + p).ToList();
                all =
                    Directory.GetFiles(inputPath, "*", SearchOption.AllDirectories)
                        .Where(p => exName.Contains(Path.GetExtension(p)))
                        .Select(p => p.Replace("\\", "/"))
                        .ToList();
                all.Sort();
                OpenRun();
            }
        }

        protected virtual void OpenRun()
        {
            for (int i = 0; i < all.Count; i++)
            {
                Console.WriteLine("替换中...请稍后" + ((float) i/all.Count).ToString("p") + "\t" + all[i]);

                string[] input = File.ReadAllLines(all[i]);

                bool isSave = false;
                for (int j = 0; j < input.Length; j++)
                {
                    string value = input[j];
                    input[j] = OpenRunLine(value);
                    isSave = value == input[j];
                }
                if (isSave)
                {
                    File.WriteAllLines(all[i], input);
                }
            }
        }

        protected virtual string OpenRunLine(string value)
        {
            return value;
        }
    }
}
