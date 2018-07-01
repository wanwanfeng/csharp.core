using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace webUtils
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string projectPath = args.FirstOrDefault();
            string filter = args.Length <= 1 ? ".js,.html,.css" : args[1];
            if (Directory.Exists(projectPath))
            {
                var list =
                    Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories)
                        .Where(p => filter.Contains(Path.GetExtension(p)))
                        .ToDictionary(p => p.Replace(projectPath + "\\", "").Replace("\\", "/"),
                            q => Md516(File.ReadAllBytes(q)))
                        .Select(p => string.Format("\"{0}\":\"{1}\"", p.Key, p.Value.ToLower()))
                        .ToList();

                var content = string.Format("var fileTimeStamp = {{{0}}};", string.Join(",", list.ToArray()));
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

        public static string Md516(byte[] input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(input);
            (md5 as IDisposable).Dispose();
            return BitConverter.ToString(data, 4, 8).Replace("-", "");
        }
    }
}
