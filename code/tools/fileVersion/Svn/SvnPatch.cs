using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace FileVersion
{
    public class SvnPatch : SvnCommon
    {
        public override void Run()
        {
            base.Run();

            var msg = string.Format("请输入起始版本号(输入数字,[{0}-{1}]),然后回车：", lowVersion, highVersion);
            startVersion = SystemConsole.GetInputStr(msg, def: lowVersion.ToString()).AsLong();
            //startVersion = Math.Max(startVersion, lowVersion);
            Console.WriteLine("起始版本号：" + startVersion);

            msg = string.Format("请输入结束版本号(输入数字,[{0}-{1}]),然后回车：", lowVersion, highVersion);
            endVersion = SystemConsole.GetInputStr(msg, def: startVersion.ToString()).AsLong();
            endVersion = Math.Max(endVersion, highVersion);
            Console.WriteLine("结束版本号：" + endVersion);

            if (startVersion == endVersion)
                return;

            Console.WriteLine("\n正在获取版本差异信息...");

            var targetList =
                CmdReadAll(string.Format("svn diff -r {0}:{1} {2} --summarize", startVersion, endVersion, svnUrl))
                    .Select(Uri.UnescapeDataString)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Where(s => !s.EndsWith("/"))
                    //.Where(s => !string.IsNullOrEmpty(s))
                    .Select(p => p.Replace(svnUrl + "/", ""))
                    .ToArray(); //去除文件夹

            Dictionary<string, FileDetailInfo> cache = new Dictionary<string, FileDetailInfo>();
            targetList.ForEach((s, index) =>
            {
                List<string> res = s.Split(' ').Where(s1 => !string.IsNullOrEmpty(s1)).ToList();
                var last = res.Skip(1).Join(" ").Replace("\\", "/").Trim();
                FileDetailInfo svnFileInfo = new FileDetailInfo()
                {
                    is_delete = res.First().Trim() == "D",
                    path = last,
                };
                cache[svnFileInfo.path] = svnFileInfo;
                Console.WriteLine("{0:D5}\t{1}", index, svnFileInfo);
            });
            ExcludeFile(cache);

            string targetDir = string.Format(Name, folder, startVersion, endVersion);
            WriteToTxt(targetDir, cache);
            List<string> del = new List<string>();
            cache.ForEach((s, index) =>
            {
                Console.Clear();
                Console.WriteLine("\n正在导出差异文件...");
                Console.WriteLine("根据项目大小时间长短不定，请耐心等待...");
                Console.WriteLine("正在导出中...{0}", ((float) (++index)/cache.Count).ToString("P"));
                Console.WriteLine("is now: {0}", s.Key);
                Console.WriteLine();
                if (!s.Value.is_delete)
                {
                    string fullPath = Environment.CurrentDirectory.Replace("\\", "/") + "/" + targetDir + "/" + s.Key;
                    DirectoryHelper.CreateDirectory(fullPath);

                    //拉取的文件版本号不会小于所在目录版本号，如若小于，说明文件所在目录曾经被移动过
                    CmdReadAll(string.Format("svn cat -r {0} \"{1}/{2}@{0}\">\"{3}\"", endVersion, svnUrl, s.Key,
                        fullPath));
                    if (File.Exists(fullPath))
                    {
                        var array =
                            CmdReadAll(string.Format("svn log -r {0}:{3} \"{1}/{2}@{0}\" -q -l1 --stop-on-copy",
                                endVersion,
                                svnUrl, s.Key, lowVersion));
                        s.Value.version = array.Skip(1).First().Split(' ').First().Replace("r", "").Trim().AsLong();
                        SetContent(fullPath, s.Value);
                        Console.WriteLine();
                    }
                    else
                    {
                        del.Add(s.Key);
                    }
                }
            });
            foreach (string s in del)
                cache.Remove(s);
            Common(targetDir, cache);
        }
    }
}
