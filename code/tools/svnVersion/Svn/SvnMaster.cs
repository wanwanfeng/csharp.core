using System;
using System.IO;
using System.Threading;

namespace svnVersion
{
    public class SvnMaster
    {
        public string svnVersion { get; private set; }
        public string svnUrl { get; private set; }

        public SvnMaster()
        {
            svnVersion = CmdHelp.Run("svn --version --quiet");
            //Thread.Sleep(100);
            Console.WriteLine("SVN版本：" + svnVersion);
            Console.WriteLine("请输入目标目录，然后回车：");
            string folder = Console.ReadLine();
            //if (!File.Exists(folder))
            //{
            //    Console.WriteLine("目标地址不存在");
            //    Console.ReadKey();
            //    return;
            //}
            svnUrl = CmdHelp.Run("svn info --show-item url") + "/" + folder;
            Console.WriteLine("库地址：" + svnUrl);
        }
    }
}
