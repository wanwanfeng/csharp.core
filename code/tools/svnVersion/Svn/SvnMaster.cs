using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;
using Library.Helper;

namespace svnVersion
{
    public class SvnMaster : SvnCommon
    {
        public string Name
        {
            get { return "svn-{0}-0-{1}-master"; }
        }

        public string svnVersion { get; private set; }
        public string svnUrl { get; private set; }
        public int highVersion { get; private set; }
        public int targetVersion { get; private set; }
        public string[] targetList { get; private set; }

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

            Console.Write("请输入目标版本号(输入数字)，然后回车：");
            targetVersion = Console.ReadLine().AsInt();
            targetVersion = Math.Min(targetVersion, highVersion);
            Console.WriteLine("目标版本号：" + targetVersion);

            Console.WriteLine("\n正在获取目标版本号文件详细信息...");

            targetList = RunCmd(string.Format("svn list -r {0} {1}@{0} -R -v", targetVersion, svnUrl));
            targetList = targetList.Where(s => !s.EndsWith("/")).ToArray(); //去除文件夹

            int index = 0;
            var cao = new List<List<string>>();
            foreach (string s in targetList)
            {
                List<string> res = s.Split(' ').Where(s1 => !string.IsNullOrEmpty(s1)).ToList();
                var last = res.Skip(6).ToArray().JoinToString(" ").Replace("\\", "/").Trim();
                List<string> list = new List<string>();

                list.Add("A");
                list.Add(res.First().Trim());
                list.Add(res.Skip(2).First().Trim());
                list.Add(Library.Encrypt.MD5.Encrypt(last));
                list.Add(last);

                cao.Add(new List<string>(list));
                Console.WriteLine((++index).ToString().PadLeft(5, '0') + "\t" + list.ToArray().JoinToString(","));
            }

            Console.Write("\n是否导出目标版本号文件（y/n），然后回车：");
            var yes = Console.ReadLine() == "y";
            string targetDir = string.Format(Name, folder, targetVersion);
            if (!yes)
            {
                File.WriteAllLines(targetDir + ".txt", cao.Select(q => q.ToArray().JoinToString(",")).ToArray(),
                    new UTF8Encoding(false));
                return;
            }
            else
            {
                Console.WriteLine("正在导出中...");
                Console.WriteLine("根据项目大时间长短不定，请耐心等待...");

                FileHelper.CreateDirectory(Environment.CurrentDirectory.Replace("\\", "/") + "/" + targetDir);
                targetList = RunCmd(string.Format("svn export -r {0} {1}@{0} {2}", targetVersion, svnUrl, targetDir));

                index = 0;
                foreach (var s in cao)
                {
                    var last = s.Last();
                    Console.WriteLine((++index).ToString().PadLeft(5, '0') + "\t" + last);
                    string fullPath = targetDir + "/" + last;
                    if (File.Exists(fullPath))
                        s.Insert(s.Count - 2, Library.Encrypt.MD5.Encrypt(File.ReadAllBytes(fullPath)));
                }
                File.WriteAllLines(targetDir + ".txt", cao.Select(q => q.ToArray().JoinToString(",")).ToArray(),
                    new UTF8Encoding(false));
            }

            PathToMd5(folder, targetDir, cao);
            MakAESEncrypt(folder, targetDir);
            MakeFolder(folder, targetDir);
            EndCmd();
        }
    }
}
