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
            if (path.EndsWith("/") || path.EndsWith("\\"))
            {
                Directory.CreateDirectory(path);
                return;
            }
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private static IEnumerable<string> PredicateFiles(IEnumerable<string> files, string[] selectExtensions)
        {
            return (selectExtensions == null || selectExtensions.Length == 0
                ? files
                : files.Where(p => selectExtensions.Contains(Path.GetExtension(p))))
                .Select(p => p.Replace("\\", "/"));
        }

        /// <summary>
        /// 获取某目录下文件列表
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="selectExtensions">.png|.jpg|.bmp|.psd|.tga|.tif|.dds</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFiles(string rootPath, string[] selectExtensions,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            return PredicateFiles(Directory.GetFiles(rootPath, "*.*", searchOption), selectExtensions);
        }

        /// <summary>
        /// 获取某目录下文件列表
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="selectExtension">.png|.jpg|.bmp|.psd|.tga|.tif|.dds</param>
        /// <param name="searchOption"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFiles(string rootPath, string selectExtension,
            SearchOption searchOption = SearchOption.AllDirectories, params char[] separator)
        {
            return PredicateFiles(Directory.GetFiles(rootPath, "*.*", searchOption),
                selectExtension.Split(separator.Length == 0 ? new[] {'|'} : separator));
        }

        /// <summary>
        /// 获取多个目录下文件列表
        /// </summary>
        /// <param name="rootPaths"></param>
        /// <param name="selectExtensions">.png|.jpg|.bmp|.psd|.tga|.tif|.dds</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFiles(string[] rootPaths, string[] selectExtensions,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            return PredicateFiles(rootPaths.SelectMany(p => Directory.GetFiles(p, "*.*", searchOption)),
                selectExtensions);
        }

        /// <summary>
        /// 获取多个目录下文件列表
        /// </summary>
        /// <param name="rootPaths"></param>
        /// <param name="selectExtension">.png|.jpg|.bmp|.psd|.tga|.tif|.dds</param>
        /// <param name="searchOption"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFiles(string[] rootPaths, string selectExtension,
            SearchOption searchOption = SearchOption.AllDirectories, params char[] separator)
        {
            return PredicateFiles(rootPaths.SelectMany(p => Directory.GetFiles(p, "*.*", searchOption)),
                selectExtension.Split(separator.Length == 0 ? new[] {'|'} : separator));
        }
    }
}