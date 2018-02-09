using System;
using System.IO;
using System.Linq;
using Library.Extensions;

namespace FileVersion
{
    public class SvnCommon : CommonBase
    {
        public string svnUrl { get; protected set; }

        public SvnCommon()
        {
            StartCmd();
            softwareVersion = RunCmd("svn --version --quiet").Last();
            isInstall = softwareVersion.Replace(".", "").AsInt() != 0;
            if (isInstall)
                Console.WriteLine("SVN版本：" + softwareVersion);
            else
                Console.WriteLine("未安装Svn命令行工具，请先安装！");
        }

        public override void Run()
        {
            bool yes = false;
            while (yes == false)
            {
                Console.Write("请输入目标目录，然后回车：");
                folder = Console.ReadLine();
                if (folder != null && !Directory.Exists(folder))
                {
                    Console.WriteLine("未输入目录或不存在!");
                    //Console.Write("\n是否将本目录作为目标目录（y/n）：");
                    //yes = Console.ReadLine() == "y";
                }
                else
                {
                    break;
                }
            }


            try
            {
                svnUrl = RunCmd("svn info --show-item url").Last();
                svnUrl += "/" + folder;
                Console.WriteLine("库地址：" + svnUrl);

                Console.WriteLine("");
                highVersion = RunCmd("svn info --show-item last-changed-revision " + folder).Last().AsInt();
                Console.WriteLine("最高版本号：" + highVersion);

                var logs =
                    RunCmd(string.Format("svn log -r 0:{0} \"{1}@{0}\" -q -l1 --stop-on-copy", highVersion, svnUrl));
                lowVersion = logs.Skip(1).First().Split('|').First().Replace("r", "").Trim().AsInt();
                Console.WriteLine("最低版本号：" + lowVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine("请判断远程库连接是否正确！" + e.Message);
            }
            Console.WriteLine("");
        }
    }
}
