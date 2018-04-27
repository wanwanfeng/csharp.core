using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var root = SystemExtensions.GetInputStr("输入目录:");
            if (Directory.Exists(root))
            {
                var folder = Path.GetFileName(root);
                var files =
                    Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                        .Select(p => p.Replace(root, "").Replace("\\", "/"))
                        .ToArray();

                var index = 0;
                var dirRoot = root.Replace(folder, "");
                foreach (string file in files)
                {
                    var path = folder + file;
                    var dirName = Path.GetDirectoryName(path).Replace("\\", "/");
                    var fileName = Path.GetFileName(path);
                    var newPath = new Library.MD5().Encrypt(dirName + md5Key) + "/" +
                                  new Library.MD5().Encrypt(fileName + md5Key);

                    var outPath = dirRoot + "md5/" + newPath;
                    var p = ((float) (index++)/files.Length).ToString("P") + "\t" + path;
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
                }
            }
            Console.WriteLine("按任意键结束!");
            Console.ReadKey();
        }
    }
}
