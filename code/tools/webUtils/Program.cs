using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace webUtils
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new[] { @"E:\Git\HelloHtml\HBuilderProjects\HelloHBuilder", ".js,.html,.css,.jpg,.png,.gif", "1" };
            string projectPath = args.FirstOrDefault();
            string filter = args.Length <= 1 ? ".js,.html,.css" : args[1];
            string mode = args.Length <= 2 ? "" : args[2];
            if (Directory.Exists(projectPath))
            {
                var cache = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories)
                    .Where(p => filter.Contains(Path.GetExtension(p)))
                    .ToDictionary(p => p.Replace(projectPath + "\\", "").Replace("\\", "/"),
                        q => Md516(File.ReadAllBytes(q)));

                string content = "";
                if (string.IsNullOrEmpty(mode))
                {
                    var list = cache.Select(p => string.Format("\"{0}\":\"{1}\"", p.Key, p.Value)).ToList();
                    content = string.Format("var fileTimeStamp = {{{0}}};", string.Join(",", list.ToArray()));
                }
                else if (mode == "1")
                {
                    var list =
                        cache.ToDictionary(p => Md516(p.Key), q => q.Value)
                            .Select(p => string.Format("\"{0}\":\"{1}\"", p.Key, p.Value))
                            .ToList();
                    content = string.Format("var fileTimeStamp = {{{0}}};", string.Join(",", list.ToArray()));
                }
                else if (mode == "2")
                {
                    //var list =
                    //    cache.Select(p => string.Format("{0}/{1}", p.Key, p.Value))
                    //        .Select(p => p.Split('/').ToList())
                    //        .ToLookup(p => p.Count)
                    //        .ToDictionary(p => p.Key, q => q.ToList());
                    //content = string.Format("var fileTimeStamp = {{{0}}};", string.Join(",", list.ToArray()));
                }

                File.WriteAllText(projectPath + ".txt", content);

                var res = projectPath + "/js/system/replacement.js";
                if (File.Exists(res))
                {
                    string[] re = File.ReadAllLines(res);
                    re[0] = content;
                    File.WriteAllLines(res, re);
                }
            }
        }

        public static string Md516(string input)
        {
            return Md516(Encoding.UTF8.GetBytes(input));
        }

        public static string Md516(byte[] input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(input);
            (md5 as IDisposable).Dispose();
            return BitConverter.ToString(data, 4, 8).Replace("-", "").ToLower();
        }
    }
}
