﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public override bool HaHa()
        {
            svnVersion = Run("svn --version --quiet").Last();
            Console.WriteLine("SVN版本：" + svnVersion);

            Console.Write("请输入目标目录，然后回车：");
            string folder = Console.ReadLine();
            if (string.IsNullOrEmpty(folder))
                return false;

            string fullFolder = Environment.CurrentDirectory + "/" + folder;
            if (!Directory.Exists(fullFolder))
            {
                Console.WriteLine("目标地址不存在");
                Console.ReadKey();
                Program.isRuning = false;
                return false;
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
                string last = res.Skip(1).ToArray().JoinToString(" ").Replace("\\", "/").Trim();
                last = (last.StartsWith(folder) ? last.Replace(folder + "/", "") : last);
                List<string> list = new List<string>();
                list.Add(res.First().Trim());
                list.Add(Library.Encrypt.MD5.Encrypt(last));
                list.Add(last);
                cao.Add(new List<string>(list));
                Console.WriteLine((++index).ToString().PadLeft(5, '0') + "        " + list.ToArray().JoinToString(","));
            }

            Console.Write("\n正在导出差异文件...");
            Console.WriteLine("正在导出中...");
            Console.WriteLine("根据项目大小时间长短不定，请耐心等待...");
            string targetDir = string.Format("svn-{0}-{2}-{1}-patch", folder, endVersion, startVersion);

            foreach (var s in cao)
            {
                if (s.First() == "D")
                {
                    s.Insert(1, "");
                    s.Insert(2, "");
                    s.Insert(s.Count - 2, "");
                }
                else
                {
                    Console.WriteLine("is now : " + s.Last());
                    string fullPath = Environment.CurrentDirectory.Replace("\\", "/") + "/" + targetDir + "/" + s.Last();
                    FileHelper.CreateDirectory(fullPath);
                    Run(string.Format("svn cat -r {0} \"{1}/{2}@{0}\">\"{3}\"", endVersion, svnUrl, s.Last(), fullPath));
                    if (File.Exists(fullPath))
                    {
                        var array =
                            Run(string.Format("svn log -r {0}:0 \"{1}/{2}@{0}\" -q -l1 --stop-on-copy", endVersion,
                                svnUrl, s.Last()));
                        s.Insert(1, array.Skip(1).First().Split(' ').First().Replace("r", ""));
                        s.Insert(2, new FileInfo(fullPath).Length.ToString());
                        s.Insert(s.Count - 2, Library.Encrypt.MD5.Encrypt(File.ReadAllBytes(fullPath)));
                    }
                }
            }
            File.WriteAllLines(targetDir + ".txt",
                cao.Where(q => q.Count == 6).Select(q => q.ToArray().JoinToString(",")), new UTF8Encoding(false));

            if (!PathToMd5(folder, targetDir, cao)) return false;
            return true;
        }
    }
}
