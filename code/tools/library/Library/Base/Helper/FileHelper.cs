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
            if (path.EndsWith("/"))
            {
                Directory.CreateDirectory(path);
                return;
            }
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        /// <summary>
        /// 获取某目录下文件列表
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="searchOption"></param>
        /// <param name="exceptExtension"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string rootPath, SearchOption searchOption, params string[] exceptExtension)
        {
            var files = Directory.GetFiles(rootPath, "*.*", searchOption)
                .Select(p => p.Replace("\\", "/"))
                .ToList();
            if (exceptExtension.Length == 0) return files;
            var exs = exceptExtension.Select(p => p.StartsWith(".") ? p : "." + p).ToList();
            return files.Where(p => !exs.Contains(Path.GetExtension(p))).ToList();
        }

        /// <summary>
        /// 获取多个目录下文件列表
        /// </summary>
        /// <param name="rootPaths"></param>
        /// <param name="searchOption"></param>
        /// <param name="exceptExtension"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string[] rootPaths, SearchOption searchOption, params string[] exceptExtension)
        {
            var files = rootPaths.SelectMany(p => Directory.GetFiles(p, "*.*", searchOption))
                    .Select(p => p.Replace("\\", "/"))
                    .ToList();
            if (exceptExtension.Length == 0) return files.ToList();
            var exs = exceptExtension.Select(p => p.StartsWith(".") ? p : "." + p).ToList();
            return files.Where(p => !exs.Contains(Path.GetExtension(p))).ToList();
        }
    }
}
