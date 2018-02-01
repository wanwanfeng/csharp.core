using System;

namespace SvnVersion
{
    internal class Program
    {
        private static bool isRuning = true;

        private static void Main(string[] args)
        {
            string mes =
                @"1,SvnMaster [输入master或m]
2,SvnPatch [输入patch或p]
3,SvnUpdate [输入list或l]";
            Console.WriteLine(mes);
            Console.Write("请输入选择，然后回车：");
            SvnCommon svnCommon = null;
            var haha = Console.ReadLine();
            switch (haha)
            {
                case "master":
                case "m":
                    svnCommon = new SvnMaster();
                    break;
                case "patch":
                case "p":
                    svnCommon = new SvnPatch();
                    break;
                case "list":
                case "l":
                    svnCommon = new SvnUpdate();
                    break;
            }

            if (svnCommon == null) return;

            while (isRuning)
            {
                svnCommon.Run();
                Console.WriteLine("按y键继续,按其余键退出......");
                isRuning = Console.ReadLine() == "y";
            }
        }
    }
}
