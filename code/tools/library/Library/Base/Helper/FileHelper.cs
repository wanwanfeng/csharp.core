using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Library.Helper
{
    public class FileHelper
    {
        /// <summary>
        /// 创建父级目录
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectory(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        /// <summary>
        /// 获取某目录下文件列表
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="searchOption"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string rootPath, SearchOption searchOption, params string[] exclude)
        {
            var files = Directory.GetFiles(rootPath, "*.*", searchOption)
                .Select(p => p.Replace("\\", "/"))
                .ToList();
            if (exclude.Length == 0) return files;
            var excludeList = new List<string>();
            foreach (var s in exclude)
                excludeList.AddRange(files.Where(p => p.ToLower().EndsWith(s.ToLower())));
            return files.Except(excludeList).ToList();
        }

        /// <summary>
        /// 获取多个目录下文件列表
        /// </summary>
        /// <param name="rootPaths"></param>
        /// <param name="searchOption"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string[] rootPaths, SearchOption searchOption, params string[] exclude)
        {
            var files = rootPaths.SelectMany(p => Directory.GetFiles(p, "*.*", searchOption))
                    .Select(p => p.Replace("\\", "/"))
                    .ToList();
            if (exclude.Length == 0) return files.ToList();
            var excludeList = new List<string>();
            foreach (var s in exclude)
                excludeList.AddRange(files.Where(p => p.ToLower().EndsWith(s.ToLower())));
            return files.Except(excludeList).ToList();
        }
    }
}
