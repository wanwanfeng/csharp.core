using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;
using Library.Helper;

namespace FileVersion
{
    public class SvnMaster : SvnCommon
    {
        public override string Name
        {
            get { return SaveDir + "{0}-{1:D8}-{2:D8}-master"; }
        }

        public long targetVersion { get; private set; }

        public override void Run()
        {
            base.Run();

            Console.Write("请输入目标版本号(输入数字,[{0}-{1}]),然后回车：", lowVersion, highVersion);
            targetVersion = Console.ReadLine().AsInt();
            targetVersion = Math.Max(targetVersion, lowVersion);
            targetVersion = Math.Min(targetVersion, highVersion);
            Console.WriteLine("目标版本号：" + targetVersion);
            Console.WriteLine();
            Console.WriteLine("\n正在获取目标版本号文件详细信息...");

            var targetList = RunCmd(string.Format("svn list -r {0} {1}@{0} -R -v", targetVersion, svnUrl), true);
            targetList = targetList.Where(s => !s.EndsWith("/")).ToArray(); //去除文件夹

            int index = 0;

            Dictionary<string, FileDetailInfo> cache = new Dictionary<string, FileDetailInfo>();
            foreach (string s in targetList)
            {
                List<string> res = s.Split(' ').Where(s1 => !string.IsNullOrEmpty(s1)).ToList();
                var last = res.Skip(6).ToArray().JoinToString(" ").Replace("\\", "/").Trim();
                FileDetailInfo svnFileInfo = new FileDetailInfo()
                {
                    action = "A",
                    version = res.First().Trim(),
                    content_size = res.Skip(2).First().Trim(),
                    path = last,
                };
                cache[svnFileInfo.path] = svnFileInfo;
                Console.WriteLine("{0:D5}\t{1}", ++index, svnFileInfo);
            }
            if (!ExcludeFile(cache)) return;

            Console.Write("\n是否导出目标版本号文件（y/n）：");
            var yes = Console.ReadLine() == "y";
            string targetDir = string.Format(Name, folder, 0, targetVersion);
            DeleteInfo(targetDir);

            //if (yes)
            //{
            //    Console.WriteLine("正在导出中...");
            //    Console.WriteLine("根据项目大时间长短不定，请耐心等待...");
            //    FileHelper.CreateDirectory(Environment.CurrentDirectory.Replace("\\", "/") + "/" + targetDir);
            //    RunCmd(string.Format("svn export -r {0} {1}@{0} {2}", targetVersion, svnUrl, targetDir), true);

            //    index = 0;
            //    foreach (var s in cache)
            //    {
            //        Console.WriteLine((++index).ToString().PadLeft(5, '0') + "\t" + s.Key);
            //        string fullPath = targetDir + "/" + s.Key;
            //        if (File.Exists(fullPath))
            //            SetContent(fullPath, s.Value);
            //    }
            //}

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
                    RunCmd(string.Format("svn cat -r {0} \"{1}/{2}@{0}\">\"{3}\"", targetVersion, svnUrl, s.Key, fullPath));
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
