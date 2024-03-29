﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace encrypt
{
    internal class YueGeDencrypt
    {

        public List<string> extensionList = new List<string>()
        {
            ".acb"
        };

        private string Md5Key = "kEfGhnNmeu4YYuhv";

        private string cmd;

        public YueGeDencrypt()
        {
            Dictionary<string, string> cache = GetCache(Md5Key);

            Console.WriteLine("-----------------------------");
            Console.WriteLine("");

            string root = SystemConsole.GetInputStr("输入加密的目标目录:");
            if (Directory.Exists(root))
            {
                var folder = Path.GetFileName(root);
                root += root.EndsWith("\\") ? "" : "\\";
                var files =
                    Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                        .Select(p => p.Replace(root, "").Replace("\\", "/"))
                        .Where(p => Path.GetFileNameWithoutExtension(p).Length == 32)
                        .ToList();

                var dirRoot = root.Replace(folder, "");
                files.ForEach((file, i, count) =>
                {
                    var path = folder + "/" + file;
                    var newPath = "";
                    if (cache.TryGetValue(path, out newPath))
                    {
                        var outPath = dirRoot + newPath;
                        FileHelper.CreateDirectory(outPath);
                        var p = ((float)i / count).ToString("P") + "\t" + path;
                        if (extensionList.Contains(Path.GetExtension(path)))
                        {
                            Console.WriteLine("路径解码中..." + p);
                            File.Copy(root + file, outPath, true);
                        }
                        else
                        {
                            Console.WriteLine("路径解码并文件解密中..." + p);
                            File.WriteAllBytes(outPath, UtilSecurity.DecryptionBytes(File.ReadAllBytes(root + file)));
                        }
                    }
                    else
                    {
                        Console.WriteLine("不存在的资源文件！");
                    }
                });
            }
        }

        private Dictionary<string, string> GetCache(string md5Key)
        {
            Dictionary<string, string> cache = new Dictionary<string, string>();

            string root = SystemConsole.GetInputStr("输入原始目录获取md5对应关系:");

            var folder = Path.GetFileName(root);
            if (folder != "app")
            {
                Console.WriteLine("文件夹选择错误，必须以/app结尾");
                return cache;
            }

            root += root.EndsWith("\\") ? "" : "\\";
            var files =
                Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                    .Select(p => p.Replace(root, "").Replace("\\", "/"))
                    .Where(p => Path.GetFileNameWithoutExtension(p).Length != 32)
                    .ToList();

            files.ForEach((file, i, count) =>
            {
                Console.WriteLine("路径关系获取中..." + ((float)i / count).ToString("P") + "\t" + file);
                var path = file;
                var dirName = Path.GetDirectoryName(path).Replace("\\", "/");
                var fileName = Path.GetFileName(path);
                var newPath = dirName.MD5(md5Key) + "/" + fileName.MD5(md5Key);
                cache[newPath] = file;
            });

            return cache;
        }
    }

    internal class YueGeDencrypt2
    {

        public List<string> extensionList = new List<string>()
        {
            ".acb"
        };

        private string Md5Key = "kEfGhnNmeu4YYuhv";

        private string cmd;

        public YueGeDencrypt2()
        {
            Dictionary<string, string> cache = null; // GetCache(Md5Key);

            Console.WriteLine("-----------------------------");
            Console.WriteLine("");

            string root = SystemConsole.GetInputStr("输入目标目录:");
            if (Directory.Exists(root))
            {
                var folder = Path.GetFileName(root);
                root += root.EndsWith("\\") ? "" : "\\";
                var files =
                    Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                        .Select(p => p.Replace(root, "").Replace("\\", "/"))
                        .Where(p => Path.GetFileNameWithoutExtension(p).Length == 32)
                        .ToList();

                files.ForEach(file =>
                {
                    File.WriteAllBytes(file + ".temp", UtilSecurity.DecryptionBytes(File.ReadAllBytes(root + file)));
                });

                return;

                var dirRoot = root.Replace(folder, "");
                files.ForEach((file, i, count) =>
                {
                    var path = folder + "/" + file;
                    var newPath = "";
                    if (cache.TryGetValue(path, out newPath))
                    {
                        var outPath = dirRoot + newPath;
                        FileHelper.CreateDirectory(outPath);
                        var p = ((float) i/count).ToString("P") + "\t" + path;
                        if (extensionList.Contains(Path.GetExtension(path)))
                        {
                            Console.WriteLine("路径解码中..." + p);
                            File.Copy(root + file, outPath, true);
                        }
                        else
                        {
                            Console.WriteLine("路径解码并文件解密中..." + p);
                            File.WriteAllBytes(outPath, UtilSecurity.DecryptionBytes(File.ReadAllBytes(root + file)));
                        }
                    }
                    else
                    {
                        Console.WriteLine("不存在的资源文件！");
                    }
                });
            }
        }

        private Dictionary<string, string> GetCache(string md5Key)
        {
            Dictionary<string, string> cache = new Dictionary<string, string>();

            string root = SystemConsole.GetInputStr("输入原始目录获取md5对应关系:");

            var folder = Path.GetFileName(root);
            if (folder != "app")
            {
                Console.WriteLine("文件夹选择错误，必须以/app结尾");
                return cache;
            }

            root += root.EndsWith("\\") ? "" : "\\";
            var files =
                Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                    .Select(p => p.Replace(root, "").Replace("\\", "/"))
                    .Where(p => Path.GetFileNameWithoutExtension(p).Length != 32)
                    .ToList();


            files.ForEach((file, i, count) =>
            {
                Console.WriteLine("路径关系获取中..." + ((float) i/count).ToString("P") + "\t" + file);
                var path = file;
                var dirName = Path.GetDirectoryName(path).Replace("\\", "/");
                var fileName = Path.GetFileName(path);
                var newPath = dirName.MD5(md5Key) + "/" + fileName.MD5(md5Key);
                cache[newPath] = file;
            });

            return cache;
        }
    }
}
