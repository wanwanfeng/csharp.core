using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;
using Library.Helper;

namespace FileVersion
{
    public class GitMaster : GitCommon
    {
        public override void Run()
        {
            base.Run();

            var beforeTip = string.Format("请输入目标版本号(输入数字,[{0}-{1}]),然后回车：", lowVersion, highVersion);
            endVersion = SystemConsole.GetInputStr(beforeTip, def: lowVersion.ToString()).AsInt();
            endVersion = Math.Min(endVersion, highVersion);
            Console.WriteLine("目标版本号：" + endVersion);
            Console.WriteLine();

            //获取当前目标版本号的list列表
            Dictionary<string, FileDetailInfo> cache = GetListCache(endVersion);

            var targetDir = ExportFile(cache);
            Common(targetDir, cache);
        }

        private string ExportFile(Dictionary<string, FileDetailInfo> cache)
        {
            var yes = SystemConsole.ContinueY("是否导出目标版本号文件（y/n）：");
            string targetDir = string.Format(Name, folder, startVersion, endVersion);
            DeleteInfo(targetDir);
            WriteToTxt(targetDir, cache);

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

            //if (yes)
            //{
            //    List<string> del = new List<string>();
            //    cache.ForEach((s, index) =>
            //    {
            //        Console.Clear();
            //        Console.WriteLine("\n正在导出文件...");
            //        Console.WriteLine("根据项目大小时间长短不定，请耐心等待...");
            //        Console.WriteLine("正在导出中...{0}", ((float)(++index) / cache.Count).ToString("P"));
            //        Console.WriteLine("is now: {0}", s.Key);
            //        Console.WriteLine();
            //        string fullPath = Environment.CurrentDirectory.Replace("\\", "/") + "/" + targetDir + "/" + s.Key;
            //        FileHelper.CreateDirectory(fullPath);

            //        //拉取的文件版本号不会小于所在目录版本号，如若小于，说明文件所在目录曾经被移动过
            //        CmdReadAll(string.Format("svn cat -r {0} \"{1}/{2}@{0}\">\"{3}\"", endVersion, svnUrl, s.Key,
            //            fullPath));
            //        //RunCmd(string.Format("svn cat -r {0} \"{1}/{2}@{0}\">\"{3}\"", s.Value.version, svnUrl, s.Key, fullPath));

            //        if (File.Exists(fullPath))
            //        {
            //            SetContent(fullPath, s.Value);
            //            Console.WriteLine();
            //        }
            //        else
            //        {
            //            del.Add(s.Key);
            //        }
            //    });
            //    foreach (string s in del)
            //        cache.Remove(s);
            //}
            return targetDir;
        }

        private Dictionary<string, FileDetailInfo> GetListCache(long version)
        {
            Console.WriteLine("\n正在获取目标版本号{0}文件详细信息...", version);

            Dictionary<string, FileDetailInfo> cache = new Dictionary<string, FileDetailInfo>();

            CmdReadAll(string.Format("git ls-tree -r -l master", "", ""))
                .Where(s => !s.EndsWith("/"))
                .ToArray()
                .ForEach((s, index) =>
                {
                    List<string> res = s.Split(' ').Where(s1 => !string.IsNullOrEmpty(s1)).ToList();
                    var last = res.Skip(6).Join(" ").Replace("\\", "/").Trim();
                    FileDetailInfo svnFileInfo = new FileDetailInfo()
                    {
                        is_delete = false,
                        version = res.First().Trim().AsLong(),
                        content_size = res.Skip(2).First().Trim().AsLong(),
                        revision = version,
                        path = last,
                    };
                    cache[svnFileInfo.path] = svnFileInfo;
                    Console.WriteLine("{0:D5}\t{1}", index, svnFileInfo);
                });
            return cache;
        }
    }
}
