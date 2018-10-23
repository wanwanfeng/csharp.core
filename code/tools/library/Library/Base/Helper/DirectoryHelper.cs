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
        /// <param name="selectExtensionArray">.png|.jpg|.bmp|.psd|.tga|.tif|.dds</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string rootPath, string[] selectExtensionArray,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            return Directory.GetFiles(rootPath, "*.*", searchOption)
                .Where(p => selectExtensionArray.Contains(Path.GetExtension(p)))
                .Select(p => p.Replace("\\", "/"))
                .ToList();
        }

        /// <summary>
        /// 获取某目录下文件列表
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="selectExtension">.png|.jpg|.bmp|.psd|.tga|.tif|.dds</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string rootPath, string selectExtension,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            var selectArray = selectExtension.Split('|');
            return Directory.GetFiles(rootPath, "*.*", searchOption)
                .Where(p => selectArray.Contains(Path.GetExtension(p)))
                .Select(p => p.Replace("\\", "/"))
                .ToList();
        }

        /// <summary>
        /// 获取多个目录下文件列表
        /// </summary>
        /// <param name="rootPaths"></param>
        /// <param name="selectExtensionArray">.png|.jpg|.bmp|.psd|.tga|.tif|.dds</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string[] rootPaths, string[] selectExtensionArray,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            return rootPaths.SelectMany(p => Directory.GetFiles(p, "*.*", searchOption))
                .Where(p => selectExtensionArray.Contains(Path.GetExtension(p)))
                .Select(p => p.Replace("\\", "/"))
                .ToList();
        }

        /// <summary>
        /// 获取多个目录下文件列表
        /// </summary>
        /// <param name="rootPaths"></param>
        /// <param name="selectExtension">.png|.jpg|.bmp|.psd|.tga|.tif|.dds</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string[] rootPaths, string selectExtension,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            var selectArray = selectExtension.Split('|');
            return rootPaths.SelectMany(p => Directory.GetFiles(p, "*.*", searchOption))
                .Where(p => selectArray.Contains(Path.GetExtension(p)))
                .Select(p => p.Replace("\\", "/"))
                .ToList();
        }
    }
}