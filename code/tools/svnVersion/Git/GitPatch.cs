using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;
using Library.Helper;

namespace FileVersion
{
    public class GitPatch : GitCommon
    {
        public override string Name
        {
            get { return "{0}-{1:D8}-{2:D8}-patch"; }
        }

        public int startVersion { get; private set; }
        public int endVersion { get; private set; }
        public string[] diffList { get; private set; }

        public override void Run()
        {
            base.Run();

            Console.Write("请输入起始版本号(输入数字,[{0}-{1}]),然后回车：", lowVersion, highVersion);
            startVersion = Console.ReadLine().AsInt();
            startVersion = Math.Max(startVersion, lowVersion);
            Console.WriteLine("起始版本号：" + startVersion);

            Console.Write("请输入结束版本号(输入数字,[{0}-{1}]),然后回车：", lowVersion, highVersion);
            endVersion = Console.ReadLine().AsInt();
            endVersion = Math.Min(endVersion, highVersion);
            endVersion = Math.Max(endVersion, lowVersion);
            Console.WriteLine("结束版本号：" + endVersion);

            if (startVersion == endVersion)
                return;

            Console.WriteLine("\n正在获取版本差异信息...");

            diffList = RunCmd(string.Format("svn diff -r {0}:{1} {2} --summarize", startVersion, endVersion, gitUrl),
                true);
            diffList = diffList.Select(p => p.Replace(gitUrl + "/", "")).Where(s => !s.EndsWith("/")).ToArray(); //去除文件夹

            Dictionary<string, FileDetailInfo> cache = new Dictionary<string, FileDetailInfo>();
            int index = 0;
            foreach (string s in diffList)
            {
                List<string> res = s.Split(' ').Where(s1 => !string.IsNullOrEmpty(s1)).ToList();
                string last = res.Skip(1).ToArray().JoinToString(" ").Replace("\\", "/").Trim();
                FileDetailInfo svnFileInfo = new FileDetailInfo()
                {
                    action = res.First().Trim(),
                    path = last,
                };
                cache[svnFileInfo.path] = svnFileInfo;
                Console.WriteLine("{0:D5}\t{1}", ++index, svnFileInfo);
            }

            ExcludeFile(cache);

            string targetDir = string.Format(Name, folder, startVersion, endVersion);
            List<string> del = new List<string>();
            index = 0;
            foreach (var s in cache)
            {
                Console.Clear();
                Console.WriteLine("\n正在导出差异文件...");
                Console.WriteLine("根据项目大小时间长短不定，请耐心等待...");
                Console.WriteLine("正在导出中...{0}", ((float)(++index) / cache.Count).ToString("P"));
                Console.WriteLine("is now: {0}", s.Key);
                Console.WriteLine();
                if (s.Value.action != "D")
                {
                    string fullPath = Environment.CurrentDirectory.Replace("\\", "/") + "/" + targetDir + "/" + s.Key;
                    FileHelper.CreateDirectory(fullPath);
                    RunCmd(string.Format("svn cat -r {0} \"{1}/{2}@{0}\">\"{3}\"", endVersion, gitUrl, s.Key, fullPath));
                    if (File.Exists(fullPath))
                    {
                        var array =
                            RunCmd(string.Format("svn log -r {0}:{3} \"{1}/{2}@{0}\" -q -l1 --stop-on-copy", endVersion,
                                gitUrl, s.Key, lowVersion));
                        s.Value.version = array.Skip(1).First().Split(' ').First().Replace("r", "").Trim();
                        SetContent(fullPath, s.Value);
                        Console.WriteLine();
                    }
                    else
                    {
                        del.Add(s.Key);
                    }
                }
            }
            foreach (string s in del)
                cache.Remove(s);
            Common(targetDir, cache);
        }
    }
}
