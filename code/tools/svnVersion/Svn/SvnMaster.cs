using System;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace svnVersion
{
    public class SvnMaster : CmdHelp
    {
        public string svnVersion { get; private set; }
        public string svnUrl { get; private set; }
        public int highVersion { get; private set; }
        public int targetVersion { get; private set; }
        public string[] targetList { get; private set; }

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

            Console.Write("请输入目标版本号(输入数字)，然后回车：");
            targetVersion = Console.ReadLine().AsInt();
            targetVersion = Math.Min(targetVersion, highVersion);
            Console.WriteLine("目标版本号：" + targetVersion);

            Console.WriteLine("\n正在获取目标版本号文件详细信息...");

            targetList = Run(string.Format("svn list -r {0} {1}@{0} -R -v", targetVersion, svnUrl));
            targetList = targetList.Where(s => !s.EndsWith("/")).ToArray();//去除文件夹

            int index = 0;
            foreach (string s in targetList)
            {
                Console.WriteLine((++index).ToString().PadLeft(5, '0') + "        " +
                                  s.Split(' ').Where(value => !string.IsNullOrEmpty(value)).ToArray().JoinToString(","));
            }

            Console.Write("\n是否导出目标版本号文件（y/n），然后回车：");
            var yes = Console.ReadLine() == "y";
            if (yes)
            {
                Console.WriteLine("正在导出中...");
                Console.WriteLine("根据项目大时间长短不定，请耐心等待...");
                string targetDir = Environment.CurrentDirectory + string.Format("/svn-{0}-0-{1}-master", folder, targetVersion);
                FileHelper.CreateDirectory(targetDir);
                targetList = Run(string.Format("svn export -r {0} {1}@{0} {2}", targetVersion, svnUrl, targetDir));
            }


            Console.ReadKey();
        }
    }
}
