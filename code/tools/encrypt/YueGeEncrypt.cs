using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Extensions;
using Library.Helper;

namespace encrypt
{
    internal class YueGeEncrypt
    {
        public List<string> extensionList = new List<string>()
        {
            ".acb"
        };

        private string cmd;

        public YueGeEncrypt()
        {
            var md5Key = "kEfGhnNmeu4YYuhv";

            var root = SystemConsole.GetInputStr("输入目录:");
            if (Directory.Exists(root))
            {
                var folder = Path.GetFileName(root);
                var files =
                    Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                        .Select(p => p.Replace(root, "").Replace("\\", "/"))
                        .ToList();

                var index = 0;
                var dirRoot = root.Replace(folder, "");
                files.ForEach(file =>
                {
                    var path = folder + file;
                    var dirName = Path.GetDirectoryName(path).Replace("\\", "/");
                    var fileName = Path.GetFileName(path);
                    var newPath = dirName.MD5(md5Key) + "/" + fileName.MD5(md5Key);

                    var outPath = dirRoot + "md5/" + newPath;
                    var p = ((float) (index++)/files.Count).ToString("P") + "\t" + path;
                    FileHelper.CreateDirectory(outPath);
                    if (extensionList.Contains(Path.GetExtension(path)))
                    {
                        Console.WriteLine("路径编码中..." + p);
                        File.Copy(root + file, outPath, true);
                    }
                    else
                    {
                        Console.WriteLine("路径编码并文件加密中..." + p);
                        File.WriteAllBytes(outPath, UtilSecurity.EncryptionBytes(File.ReadAllBytes(root + file)));
                    }
                });
            }
        }
    }
}
