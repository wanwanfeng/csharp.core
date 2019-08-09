using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;

namespace Library.Helper
{
    public partial class FileHelper : DirectoryHelper
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
            path.ToList().ForEach(p =>
            {
                p = p.Replace("\\", "/").TrimStart('/');
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
            return path.Select(p => p.Replace("\\", "/").TrimStart('/'))
                .ToDictionary(p => p, q => func == null ? null : func.Invoke(q));
        }

        /// <summary>
        /// 读取每一行
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadAllLines(string path, Encoding encoding = null)
        {
            if (!File.Exists(path))
                throw new Exception("file not exist!");
            using (var fileStream = File.OpenRead(path))
            {
                using (var reader = new StreamReader(fileStream, encoding ?? Encoding.UTF8))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
            }
        }

        /// <summary>
        /// 写入每一行
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static IEnumerable<string> WriteAllLines(string path, IEnumerable<string> content,
            Encoding encoding = null)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                using (var write = new StreamWriter(fileStream, encoding ?? Encoding.UTF8))
                {
                    foreach (string line in content)
                    {
                        write.WriteLine(line);
                        yield return line;
                    }
                }
            }
        }

        public static void FileMerge(string[] paths, string outFile)
        {
            outFile = outFile.TrimStart('\\').TrimStart('/');
            CreateDirectory(outFile);
            using (var sw = File.Open(outFile, FileMode.Append))
            {
                foreach (var path in paths)
                {
                    using (var sr = File.Open(path.TrimStart('\\').TrimStart('/'), FileMode.Open))
                    {
                        sr.CopyTo(sw);
                    }
                }
            }
        }
    }

    public partial class FileHelper
    {
        private class FileStream : System.IO.FileStream
        {
            byte[] keyArray = Encoding.UTF8.GetBytes("fdhgfhyut6yik768iujhf523w5656786olkou8i9089");

            public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) : base(path, mode, access, share, bufferSize, useAsync)
            {
            }
            public FileStream(string path, FileMode mode) : base(path, mode)
            {
            }
            public override int Read(byte[] array, int offset, int count)
            {
                var index = base.Read(array, offset, count);
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] ^= keyArray[i % keyArray.Length];
                }
                return index;
            }
            public override void Write(byte[] array, int offset, int count)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] ^= keyArray[i % keyArray.Length];
                }
                base.Write(array, offset, count);
            }
        }

        /// <summary>
        /// 文件读取异或加密文件
        /// </summary>
        /// <param name="sourceFileName">源路径</param>
        public static byte[] ReadAllBytes(string sourceFileName)
        {
            if (!File.Exists(sourceFileName)) return null;

            using (var sr = new FileStream(sourceFileName, FileMode.Open))
            {
                using (var sw = new MemoryStream())
                {
                    byte[] bytes = new byte[4096];
                    int len;
                    while ((len = sr.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        sw.Write(bytes, 0, len);
                    }
                    return sw.ToArray();
                }
            }
        }

        /// <summary>
        /// 文件读取异或加密文件
        /// </summary>
        /// <param name="sourceFileName">源路径</param>
        public static string ReadAllText(string sourceFileName)
        {
            if (!File.Exists(sourceFileName)) return "";
            byte[] content = ReadAllBytes(sourceFileName);
            return content == null ? "" : Encoding.UTF8.GetString(content, 0, content.Length);
        }

        /// <summary>
        /// 文件读取异或加密文件
        /// </summary>
        /// <param name="sourceFileName">源路径</param>
        public static string[] ReadAllLines(string sourceFileName)
        {
            if (!File.Exists(sourceFileName)) return null;
            return ReadAllText(sourceFileName).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 文件写入异或加密文件
        /// </summary>
        /// <param name="sourceFileName">源路径</param>
        public static void WriteAllBytes(string destFileName, byte[] bytes)
        {
            using (var sw = new FileStream(destFileName, FileMode.OpenOrCreate))
            {
                sw.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// 文件写入异或加密文件
        /// </summary>
        /// <param name="sourceFileName">源路径</param>
        public static void WriteAllText(string destFileName, string content)
        {
            WriteAllBytes(destFileName, Encoding.UTF8.GetBytes(content));
        }

        /// <summary>
        /// 文件写入异或加密文件
        /// </summary>
        /// <param name="sourceFileName">源路径</param>
        public static void WriteAllLines(string destFileName, string[] contents)
        {
            WriteAllText(destFileName, string.Join("\n", contents));
        }
    }
}
