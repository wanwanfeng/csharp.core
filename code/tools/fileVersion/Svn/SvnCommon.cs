using System;
using System.IO;
using System.Linq;
using Library;
using Library.Extensions;

namespace FileVersion
{
    public class SvnCommon : CommonBase
    {
        public override string Name
        {
            get { return SaveDir + "{0}-{1:D8}-{2:D8}"; }
        }

        public string svnUrl { get; protected set; }
        public string svnUserName { get; protected set; }
        public string svnPassword { get; protected set; }

        public override string[] CmdReadAll(string input)
        {
            if (string.IsNullOrEmpty(svnUserName) || string.IsNullOrEmpty(svnPassword))
                return base.CmdReadAll(input);
            string newInput = string.Format("{0} --username {1} --password {2}", input, svnUserName, svnPassword);
            return base.CmdReadAll(newInput);
        }

        public SvnCommon()
        {
            svnUrl = Config.IniReadValue("Svn", "url", "").Trim();
            svnUserName = Config.IniReadValue("Svn", "username", "").Trim();
            svnPassword = Config.IniReadValue("Svn", "password", "").Trim();
            Console.WriteLine("url:" + svnUrl);
            Console.WriteLine("username:" + svnUserName);
            Console.WriteLine("password:" + svnPassword);
            Console.WriteLine("--------------------------------------");

            StartCmd();
            softwareVersion = CmdReadAll("svn --version --quiet").Last();
            isInstall = softwareVersion.Replace(".", "").AsInt() != 0;
            if (isInstall)
                Console.WriteLine("SVN版本：" + softwareVersion);
            else
                Console.WriteLine("未安装Svn命令行工具，请先安装！");
        }

        public override void Run()
        {
            try
            {
                //应用在某版本库实例中
                if (string.IsNullOrEmpty(svnUrl))
                {
                    bool yes = false;
                    while (yes == false)
                    {
                        folder = SystemConsole.GetInputStr("请输入目标目录，然后回车：");
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
                    svnUrl = CmdReadAll("svn info --show-item url").Last();
                    svnUrl += "/" + folder;
                }
                else
                {
                    folder = Path.GetFileNameWithoutExtension(svnUrl);
                }

                Console.WriteLine("库地址：" + svnUrl);

                Console.WriteLine("");
                highVersion = CmdReadAll("svn info --show-item last-changed-revision " + svnUrl).Last().AsInt();
                Console.WriteLine("最高版本号：" + highVersion);

                var logs =
                    CmdReadAll(string.Format("svn log -r 0:{0} \"{1}@{0}\" -q -l1 --stop-on-copy", highVersion, svnUrl));
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
