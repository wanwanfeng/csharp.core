using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Helper
{
    public class FileHelper : DirectoryHelper
    {
        /// <summary>
        /// 路径为key获取dic嵌套值
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object Path2DictionaryGetValue(Dictionary<string, object> dic, string path)
        {
            var temp = dic;
            var array = path.Replace("\\", "/").TrimStart('/').Split('/').ToList();
            array.GetRange(0, array.Count - 1).ForEach(k =>
            {
                if (temp.ContainsKey(k))
                    temp = temp[k] as Dictionary<string, object>;
                else
                    temp = null;
            });
            return temp != null && temp.ContainsKey(array.Last()) ? temp[array.Last()] : null;
        }

        /// <summary>
        /// 路径转换为dic嵌套
        /// </summary>
        /// <param name="path"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Dictionary<string, object> Path2Dictionary(string path, Func<string, string, object> func = null)
        {
            return
                String2Dictionary(
                    Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Select(p => p.Replace(path, "")), func);
        }

        /// <summary>
        /// 路径列表转换为dic嵌套
        /// </summary>
        /// <param name="path"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Dictionary<string, object> String2Dictionary(IEnumerable<string> path,
            Func<string, string, object> func = null)
        {
            Dictionary<string, object> cache = new Dictionary<string, object>();
            path.Select(p => p.Replace("\\", "/").TrimStart('/'))
                .ToList()
                .ForEach(p =>
                {
                    var temp = cache;
                    var array = p.Split('/').ToList();
                    array.GetRange(0, array.Count - 1).ForEach(k =>
                    {
                        if (!temp.ContainsKey(k))
                            temp[k] = new Dictionary<string, object>();
                        temp = temp[k] as Dictionary<string, object>;
                    });
                    temp[array.Last()] = func == null ? null : func.Invoke(array.Last(), p);
                });
            return cache;
        }

        public static Dictionary<string, object> Path2List(string path, Func<string, object> func = null)
        {
            return
                String2List(
                    Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Select(p => p.Replace(path, "")), func);
        }

        public static Dictionary<string, object> String2List(IEnumerable<string> path, Func<string, object> func = null)
        {
            var cache = new Dictionary<string, object>();
            path.Select(p => p.Replace("\\", "/").TrimStart('/'))
                .ToList()
                .ForEach(p =>
                {
                    cache[p] = func == null ? null : func.Invoke(p);
                });
            return cache;
        }
    }
}
