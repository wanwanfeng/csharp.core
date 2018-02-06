using System;
using System.IO;
using System.Linq;

namespace FileVersion
{
    public class GitCommon : CommonBase
    {
        public string gitVersion { get; protected set; }
        public string gitUrl { get; protected set; }

        public GitCommon()
        {
            StartCmd();
            gitVersion = RunCmd("git --version").Last();
            isInstall = gitVersion.StartsWith("git");
            if (isInstall)
                Console.WriteLine("Git版本：" + gitVersion);
            else
                Console.WriteLine("未安装Git命令行工具，请先安装！");
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

            gitUrl = RunCmd("git remote -v").First().Replace("origin", "").Replace("(fetch)","").Trim();
            gitUrl += "/" + folder;
            Console.WriteLine("库地址：" + gitUrl);

            Console.WriteLine("");
            //highVersion = RunCmd("git rev-parse HEAD " + folder).Last().AsInt();
            //Console.WriteLine("最高版本号：" + highVersion);

            var logs = RunCmd("git log --pretty --oneline " + folder, true);
            var highsha_1 = logs.First().Split(' ').First();
            var lowsha_1 = logs.Last().Split(' ').First();



            Console.WriteLine("最高版本号：" + highVersion);
            Console.WriteLine("最低版本号：" + lowVersion);

            Console.WriteLine("");
        }
    }
}
