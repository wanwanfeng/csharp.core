using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Helper
{
    public class DirectoryHelper
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
        public static List<string> GetFiles(string rootPath, SearchOption searchOption = SearchOption.AllDirectories,
            params string[] exceptExtension)
        {
            return Directory.GetFiles(rootPath, "*.*", searchOption)
                .Select(p => p.Replace("\\", "/"))
                .Where(p => exceptExtension.Contains(Path.GetExtension(p)))
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="exceptExtension">.png|.jpg|.bmp|.psd|.tga|.tif|.dds</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string rootPath, string exceptExtension,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            return Directory.GetFiles(rootPath, "*.*", searchOption)
                .Select(p => p.Replace("\\", "/"))
                .Where(p => exceptExtension.Split('|').Contains(Path.GetExtension(p)))
                .ToList();
        }

        /// <summary>
        /// 获取多个目录下文件列表
        /// </summary>
        /// <param name="rootPaths"></param>
        /// <param name="searchOption"></param>
        /// <param name="exceptExtension"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string[] rootPaths, SearchOption searchOption = SearchOption.AllDirectories,
            params string[] exceptExtension)
        {
            return rootPaths.SelectMany(p => Directory.GetFiles(p, "*.*", searchOption))
                .Select(p => p.Replace("\\", "/"))
                .Where(p => exceptExtension.Contains(Path.GetExtension(p)))
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPaths"></param>
        /// <param name="exceptExtension">.png|.jpg|.bmp|.psd|.tga|.tif|.dds</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string[] rootPaths, string exceptExtension,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            return rootPaths.SelectMany(p => Directory.GetFiles(p, "*.*", searchOption))
                .Select(p => p.Replace("\\", "/"))
                .Where(p => exceptExtension.Split('|').Contains(Path.GetExtension(p)))
                .ToList();
        }
    }
}