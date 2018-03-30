﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace FileVersion
{
    public class SvnList : SvnCommon
    {
        private string outDir
        {
            get { return "OutPut"; }
        }

        private string outFile
        {
            get { return "c_resource.json"; }
        }

        public override void Run()
        {
            base.Run();

            if (File.Exists(outFile)) ;
            {
                
            }

            Console.Write("请输入目标版本号(输入数字,[{0}-{1}]),然后回车：", lowVersion, highVersion);
            endVersion = Console.ReadLine().AsInt();
            endVersion = Math.Max(endVersion, lowVersion);
            endVersion = Math.Min(endVersion, highVersion);
            Console.WriteLine("目标版本号：" + endVersion);
            Console.WriteLine();
            Console.WriteLine("\n正在获取目标版本号文件详细信息...");

            var targetList =
                RunCmd(string.Format("svn list -r {0} {1}@{0} -R -v", endVersion, svnUrl), true)
                    .Where(s => !s.EndsWith("/"))
                    .ToArray();//去除文件夹

            Dictionary<string, FileDetailInfo> cache = new Dictionary<string, FileDetailInfo>();
            int index = 0;
            foreach (string s in targetList)
            {
                List<string> res = s.Split(' ').Where(s1 => !string.IsNullOrEmpty(s1)).ToList();
                var last = res.Skip(6).ToArray().JoinToString(" ").Replace("\\", "/").Trim();
                FileDetailInfo svnFileInfo = new FileDetailInfo()
                {
                    is_delete = false,
                    version = res.First().Trim(),
                    content_size = res.Skip(2).First().Trim().AsLong(),
                    path = last,
                };
                cache[svnFileInfo.path] = svnFileInfo;
                Console.WriteLine("{0:D5}\t{1}", ++index, svnFileInfo);
            }
            if (!ExcludeFile(cache)) return;

            Console.Write("\n是否导出目标版本号文件（y/n）：");
            var yes = Console.ReadLine() == "y";
            string targetDir = string.Format(Name, folder, startVersion, endVersion);
            DeleteInfo(targetDir);
            WriteToTxt(targetDir, cache);

            if (yes)
            {
                List<string> del = new List<string>();
                index = 0;
                foreach (var s in cache)
                {
                    Console.Clear();
                    Console.WriteLine("\n正在导出文件...");
                    Console.WriteLine("根据项目大小时间长短不定，请耐心等待...");
                    Console.WriteLine("正在导出中...{0}", ((float) (++index)/cache.Count).ToString("P"));
                    Console.WriteLine("is now: {0}", s.Key);
                    Console.WriteLine();
                    string fullPath = Environment.CurrentDirectory.Replace("\\", "/") + "/" + targetDir + "/" + s.Key;
                    FileHelper.CreateDirectory(fullPath);

                    //拉取的文件版本号不会小于所在目录版本号，如若小于，说明文件所在目录曾经被移动过
                    RunCmd(string.Format("svn cat -r {0} \"{1}/{2}@{0}\">\"{3}\"", endVersion, svnUrl, s.Key, fullPath));
                    //RunCmd(string.Format("svn cat -r {0} \"{1}/{2}@{0}\">\"{3}\"", s.Value.version, svnUrl, s.Key, fullPath));

                    if (File.Exists(fullPath))
                    {
                        SetContent(fullPath, s.Value);
                        Console.WriteLine();
                    }
                    else
                    {
                        del.Add(s.Key);
                    }
                }
                foreach (string s in del)
                    cache.Remove(s);
            }
            Common(targetDir, cache);
        }
    }
}