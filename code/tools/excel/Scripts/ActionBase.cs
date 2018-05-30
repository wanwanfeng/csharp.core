using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;

namespace Script
{
    public class ActionBase
    {
        public static string InputPath { get; set; }


        public static List<string> CheckPath(string exce, bool isOnlydir = false)
        {
            List<string> files = new List<string>();

            string path = SystemExtensions.GetInputStr(isOnlydir ? "请拖入文件夹：" : "请拖入文件夹或文件：", "您选择的文件夹或文件：");
            if (string.IsNullOrEmpty(path))
                return files;

            if (Directory.Exists(path))
            {
                files = Directory.GetFiles(path).ToList();
            }
            else if (File.Exists(path))
            {
                files.Add(path);
            }
            else
            {
                Console.WriteLine("path is not valid!");
                return files;
            }

            InputPath = path;
            var exs = exce.AsStringArray(',', '|').Select(p => p.StartsWith(".") ? p : "." + p).ToList();
            files = files.Where(p => exs.Contains(Path.GetExtension(p))).ToList();
            files.Sort();
            return files;
        }
    }
}