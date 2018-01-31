using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;
using Library.Helper;

namespace svnVersion
{
    public class SvnPatch : SvnCommon
    {
        public string Name
        {
            get { return "svn-{0}-{1}-{2}-patch"; }
        }

        public string svnVersion { get; private set; }
        public string svnUrl { get; private set; }
        public int highVersion { get; private set; }
        public int startVersion { get; private set; }
        public int endVersion { get; private set; }
        public string[] diffList { get; private set; }

        public override void Run()
        {
            StartCmd();
            svnVersion = RunCmd("svn --version --quiet").Last();
            Console.WriteLine("SVN版本：" + svnVersion);

            Console.Write("请输入目标目录，然后回车：");
            string folder = Console.ReadLine();
            if (folder != null && !Directory.Exists(folder))
            {
                Console.WriteLine("目标地址不存在");
                return;
            }

            svnUrl = RunCmd("svn info --show-item url").Last();
            svnUrl += "/" + folder;
            Console.WriteLine("库地址：" + svnUrl);

            Console.WriteLine("");
            highVersion = RunCmd("svn info --show-item last-changed-revision").Last().AsInt();
            Console.WriteLine("最高版本号：" + highVersion);

            Console.Write("请输入起始版本号(输入数字)，然后回车：");
            startVersion = Console.ReadLine().AsInt();
            startVersion = Math.Min(startVersion, highVersion);
            Console.WriteLine("起始版本号：" + startVersion);

            Console.Write("请输入结束版本号(输入数字)，然后回车：");
            endVersion = Console.ReadLine().AsInt();
            endVersion = Math.Min(endVersion, highVersion);
            Console.WriteLine("结束版本号：" + endVersion);

            Console.WriteLine("\n正在获取版本差异信息...");

            diffList = RunCmd(string.Format("svn diff -r {0}:{1} --summarize", startVersion, endVersion));
            diffList = diffList.Where(s => !s.EndsWith("/")).ToArray(); //去除文件夹

            Dictionary<string, SvnFileInfo> cache = new Dictionary<string, SvnFileInfo>();
            int index = 0;
            foreach (string s in diffList)
            {
                List<string> res = s.Split(' ').Where(s1 => !string.IsNullOrEmpty(s1)).ToList();
                string last = res.Skip(1).ToArray().JoinToString(" ").Replace("\\", "/").Trim();
                last = (last.StartsWith(folder) ? last.Replace(folder + "/", "") : last);
                SvnFileInfo svnFileInfo = new SvnFileInfo()
                {
                    action = res.First().Trim(),
                    path = last,
                };
                cache[svnFileInfo.path] = svnFileInfo;
                Console.WriteLine((++index).ToString().PadLeft(5, '0') + "\t" + svnFileInfo);
            }

            Console.Write("\n正在导出差异文件...");
            Console.WriteLine("正在导出中...");
            Console.WriteLine("根据项目大小时间长短不定，请耐心等待...");
            string targetDir = string.Format(Name, folder, startVersion, endVersion);

            foreach (var s in cache)
            {
                if (s.Value.action != "D")
                {
                    Console.WriteLine("is now : " + s.Key);
                    string fullPath = Environment.CurrentDirectory.Replace("\\", "/") + "/" + targetDir + "/" + s.Key;
                    FileHelper.CreateDirectory(fullPath);
                    RunCmd(string.Format("svn cat -r {0} \"{1}/{2}@{0}\">\"{3}\"", endVersion, svnUrl, s.Key, fullPath));
                    if (File.Exists(fullPath))
                    {
                        var array =
                            RunCmd(string.Format("svn log -r {0}:0 \"{1}/{2}@{0}\" -q -l1 --stop-on-copy", endVersion,
                                svnUrl, s.Key));
                        s.Value.version = array.Skip(1).First().Split(' ').First().Replace("r", "");
                        s.Value.content_size = new FileInfo(fullPath).Length.ToString();
                        s.Value.content_md5 = Library.Encrypt.MD5.Encrypt(File.ReadAllBytes(fullPath));
                    }
                }
            }
            File.WriteAllLines(targetDir + ".txt", cache.Select(q => q.Value.ToString()).ToArray(),
                new UTF8Encoding(false));
            PathToMd5(folder, targetDir, cache);
            MakAESEncrypt(folder, targetDir, cache);
            MakeFolder(folder, targetDir);
            EndCmd();
        }
    }
}
