using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace Script
{
    public class ActionBase
    {
        public enum SelectType
        {
            [Description("请拖入文件：")] File,
            [Description("请拖入文件夹：")] Folder,
            [Description("请拖入文件夹或文件：")] All
        }

        public IDictionary<SelectType, string> CacheSelect;

        public ActionBase()
        {
            CacheSelect = AttributeHelper.GetCacheDescription<SelectType>();
        }

        public static string InputPath { get; set; }


        public List<string> CheckPath(string exce, SelectType selectType = SelectType.All)
        {
            List<string> files = new List<string>();

            string path = SystemExtensions.GetInputStr(CacheSelect[selectType], "您选择的文件夹或文件：");
            if (string.IsNullOrEmpty(path))
                return files;

            switch (selectType)
            {
                case SelectType.File:
                    if (File.Exists(path))
                    {
                        files.Add(path);
                    }
                    break;
                case SelectType.Folder:
                case SelectType.All:
                    if (Directory.Exists(path))
                    {
                        files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();
                    }
                    break;
                default:
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