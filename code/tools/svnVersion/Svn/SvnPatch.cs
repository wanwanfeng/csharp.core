using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace svnVersion
{
    public class SvnPatch : CmdHelp
    {
        public string svnVersion { get; private set; }
        public string svnUrl { get; private set; }
        public int highVersion { get; private set; }
        public int startVersion { get; private set; }
        public int endVersion { get; private set; }
        public string[] diffList { get; private set; }

        public override void HaHa()
        {
            svnVersion = Run("svn --version --quiet").Last();
            Console.WriteLine("SVN版本：" + svnVersion);

            Console.Write("请输入目标目录，然后回车：");
            string folder = Console.ReadLine();
            string fullFolder = Environment.CurrentDirectory + "/" + folder;
            if (!Directory.Exists(fullFolder))
            {
                Console.WriteLine("目标地址不存在");
                Console.ReadKey();
                Program.Runing = false;
                return;
            }

            svnUrl = Run("svn info --show-item url").Last();
            svnUrl += "/" + folder;
            Console.WriteLine("库地址：" + svnUrl);

            Console.WriteLine("");
            highVersion = Run("svn info --show-item last-changed-revision").Last().AsInt();
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

            diffList = Run(string.Format("svn diff -r {0}:{1} --summarize", startVersion, endVersion));
            diffList = diffList.Where(s => !s.EndsWith("/")).ToArray(); //去除文件夹

            int index = 0;
            var cao = new List<List<string>>();
            foreach (string s in diffList)
            {
                List<string> res = s.Split(' ').Where(s1 => !string.IsNullOrEmpty(s1)).ToList();
                List<string> list = new List<string> {res.First()};
                res.RemoveAt(0);
                list.Add(res.ToArray().JoinToString(" ").Replace("\\", "/"));
                cao.Add(new List<string>(list));
                Console.WriteLine((++index).ToString().PadLeft(5, '0') + "        " + list.ToArray().JoinToString(","));
            }

            Console.Write("\n正在导出差异文件...");
            Console.WriteLine("正在导出中...");
            Console.WriteLine("根据项目大时间长短不定，请耐心等待...");
            string targetDir = string.Format("/svn-{0}-{2}-{1}-patch", folder, endVersion, startVersion);
            FileHelper.CreateDirectory(Environment.CurrentDirectory + targetDir);
            foreach (var s in cao)
            {
                Run(string.Format("svn cat -r {0} {1}/{2}/{3}@{0}", endVersion, svnUrl, targetDir, s.Last()));
            }

            Console.ReadKey();
        }
    }
}
