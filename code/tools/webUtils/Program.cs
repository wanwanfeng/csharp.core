using System;
using System.Collections.Generic;
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
            //args = new[] { @"D:\Work\mfxy\client\tw\magica", ".js,.html,.css,.jpg,.png,.gif", "2" };
            string projectPath = args.FirstOrDefault();
            string filter = args.Length <= 1 ? ".html,.css,.json,.gif,.png,.jpg" : args[1];
            string mode = args.Length <= 2 ? "" : args[2];
            string exfile = args.Length <= 3 ? "index.html;js/_common/baseConfig.js;js/system/replacement.js" : args[3];


            if (Directory.Exists(projectPath))
            {
                var cache = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories)
                    .Where(p => filter.Contains(Path.GetExtension(p)))
                    .Select(p => p.Replace(projectPath + "\\", "").Replace("\\", "/"))
                    .Except(exfile.Split(';'))
                    .ToList();

                cache.Sort();

                string content = "";
                if (string.IsNullOrEmpty(mode))
                {
                    WritePathToReplacement(projectPath, cache);
                }
                else if (mode == "1")
                {
                    WriteMd5ToReplacement(projectPath, cache);
                }
                else if (mode == "2")
                {
                    var list = cache.ToLookup(Path.GetExtension, q => q)
                        .ToDictionary(p => p.Key, q => new List<string>(q));
                    list.Remove("");
                    var cac = list.Keys.ToList().Select(p => string.Format("js/system/replacement/{0}.js", p.Substring(1))).ToList();
                    list[".js"].RemoveAll(p => cac.Contains(p));
                    cac.Clear();

                    foreach (var pair in list)
                    {
                        var res = "/js/system/replacement/" + pair.Key.Substring(1);
                        WritePathToReplacement(projectPath, pair.Value, res);
                        cac.Add(res.Substring(1) + ".js");
                    }

                    WritePathToReplacement(projectPath, cac);
                }
            }
        }

        private static void WritePathToReplacement(string projectPath, List<string> paths, string pre = "/js/system/replacement")
        {
            var res = pre + ".js";
            paths.Remove(res.Substring(1));
            var list = paths.Select(p => string.Format("\"{0}\":\"{1}\"", p, Md516(File.ReadAllBytes(projectPath +"/"+ p)))).ToList();
            var content = string.Format("define({{{0}}});", string.Join(",", list));
            File.WriteAllText(projectPath + ".txt", content);

            if (!Directory.Exists(Path.GetDirectoryName(projectPath + res)))
                Directory.CreateDirectory(Path.GetDirectoryName(projectPath + res));

            File.WriteAllText(projectPath +  res, content);
        }

        private static void WriteMd5ToReplacement(string projectPath, List<string> paths,
            string pre = "/js/system/replacement")
        {
            var res = pre + ".js";
            paths.Remove(res.Substring(1));
            var list =
                paths.Select(
                    p => string.Format("\"{0}\":\"{1}\"", Md516(p), Md516(File.ReadAllBytes(projectPath + "/" + p))))
                    .ToList();
            var content = string.Format("define({{{0}}});", string.Join(",", list));
            File.WriteAllText(projectPath + ".txt", content);
            File.WriteAllText(projectPath + res, content);
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
