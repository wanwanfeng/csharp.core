using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Extensions;

namespace FileVersion
{
    public class GitCommon : CommonBase
    {
        public override string Name
        {
            get { return SaveDir + "{0}-{1:D8}-{2:D8}"; }
        }

        public string gitUrl { get; protected set; }
        public string gitUserName { get; protected set; }
        public string gitPassword { get; protected set; }

        public override string[] CmdReadAll(string input)
        {
            if (string.IsNullOrEmpty(gitUserName) || string.IsNullOrEmpty(gitPassword))
                return base.CmdReadAll(input);
            string newInput = string.Format("{0} --username {1} --password {2}", input, gitUserName, gitPassword);
            return base.CmdReadAll(newInput);
        }

        public GitCommon()
        {
            gitUrl = Config.IniReadValue("Git", "url", "").Trim();
            gitUserName = Config.IniReadValue("Git", "username", "").Trim();
            gitPassword = Config.IniReadValue("Git", "password", "").Trim();
            Console.WriteLine("url:" + gitUrl);
            Console.WriteLine("username:" + gitUserName);
            Console.WriteLine("password:" + gitPassword);
            Console.WriteLine("--------------------------------------");

            StartCmd();
            softwareVersion = CmdReadAll("git --version").Last();
            isInstall = softwareVersion.StartsWith("git");
            if (isInstall)
                Console.WriteLine("Git版本：" + softwareVersion);
            else
                Console.WriteLine("未安装Git命令行工具，请先安装！");
        }


        protected class ConvertStruct
        {
            public int index;
            public string dateTime;
            public string sha1;

            public override string ToString()
            {
                return string.Format("{0}\t{1},{2}", index, dateTime, sha1);
            }
        }

        protected List<ConvertStruct> CacheConvert = new List<ConvertStruct>();

        public override void Run()
        {
            if (string.IsNullOrEmpty(gitUrl))
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

                gitUrl = CmdReadAll("git remote -v").First().Replace("origin", "").Replace("(fetch)", "").Trim();
                gitUrl += "/" + folder;
            }
            else
            {
                folder = Path.GetFileNameWithoutExtension(gitUrl);
            }

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

            var logs = CmdReadAll("git log --reverse --pretty=format:\"%ad,%H\" --date=format:\"%y-%m-%d-%H-%M-%S\" " + gitUrl);
            Console.WriteLine("");

            int index = 0;
            foreach (var log in logs)
            {
                CacheConvert.Add(new ConvertStruct()
                {
                    index = index,
                    dateTime = log.Split(',').First(),
                    sha1 = log.Split(',').Last()
                });
                ++index;
            }
            CacheConvert.Reverse();

            foreach (var convertStruct in CacheConvert)
            {
                Console.WriteLine(convertStruct);
            }

            highVersion = CacheConvert.First().index;
            lowVersion = CacheConvert.Last().index;

            Console.WriteLine("");
            Console.WriteLine("最高版本号：{0}", highVersion);
            Console.WriteLine("最低版本号：{0}", lowVersion);
            Console.WriteLine("");
        }
    }
}
