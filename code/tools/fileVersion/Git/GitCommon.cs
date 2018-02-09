﻿using System;
using System.IO;
using System.Linq;
using Library.Extensions;

namespace FileVersion
{
    public class GitCommon : CommonBase
    {
        public string gitUrl { get; protected set; }

        public GitCommon()
        {
            StartCmd();
            softwareVersion = RunCmd("git --version").Last();
            isInstall = softwareVersion.StartsWith("git");
            if (isInstall)
                Console.WriteLine("Git版本：" + softwareVersion);
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

            gitUrl = RunCmd("git remote -v").First().Replace("origin", "").Replace("(fetch)", "").Trim();
            gitUrl += "/" + folder;
            Console.WriteLine("库地址：" + gitUrl);

            Console.WriteLine("");
            //highVersion = RunCmd("git rev-parse HEAD " + folder).Last().AsInt();
            //Console.WriteLine("最高版本号：" + highVersion);

            //var logs = RunCmd("git log --pretty --oneline " + folder, true);
            //var highsha_1 = logs.First().Split(' ').First();
            //var lowsha_1 = logs.Last().Split(' ').First();

            //var logs = RunCmd("git rev-list --all " + folder, true);
            //var highsha_1 = logs.First();
            //var lowsha_1 = logs.Last();

            //https://git-scm.com/book/zh/v1/Git-%E5%9F%BA%E7%A1%80-%E6%9F%A5%E7%9C%8B%E6%8F%90%E4%BA%A4%E5%8E%86%E5%8F%B2

            var logs = RunCmd("git log --pretty=format:\"%ad,%H\" --date=format:\"%y%m%d%H%M%S\" " + folder, true);
            highVersion = logs.First().Split(',').First().AsLong();
            lowVersion = logs.Last().Split(',').First().AsLong();
            Console.WriteLine("");
            int index = 0;
            foreach (var log in logs)
            {
                Console.WriteLine(++index + "\t" + log);
            }
            Console.WriteLine("");
            Console.WriteLine("最高版本号：{0}", logs.First());
            Console.WriteLine("最低版本号：{0}", logs.Last());
            Console.WriteLine("");
        }
    }
}
