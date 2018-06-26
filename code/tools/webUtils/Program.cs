using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webUtils
{
    class Program
    {
        static void Main(string[] args)
        {
            string projectPath = args.FirstOrDefault();
            if (Directory.Exists(projectPath))
            {
                var list =
                    Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories)
                        .Where(p => ".js|.html".Contains(Path.GetExtension(p)))
                        .ToDictionary(p => p.Replace(projectPath + "\\", "").Replace("\\", "/"),
                            q => Library.Encrypt.MD516(File.ReadAllText(q)))
                        .Select(p => string.Format("\"{0}\":\"{1}\"", p.Key, p.Value.ToLower()))
                        .ToList();

                var content = string.Format("var fileTimeStamp = {{{0}}}", string.Join(",", list.ToArray()));
                File.WriteAllText(projectPath + ".txt", content);

                string[] re = File.ReadAllLines(projectPath +  "/js/system/replacement.js");
                re[0] = content;
                File.WriteAllLines(projectPath +  "/js/system/replacement.js",re);
            }
        }
    }
}
