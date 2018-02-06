using System;

namespace FileVersion
{
    internal class Program
    {
        private static bool isRuning = true;

        private static void Main(string[] args)
        {
            string mes =
                @"1,SvnMaster [输入sm]
2,SvnPatch [输入sp]
3,SvnUpdate [输入sl]
";
//1,GitMaster [输入gm]
//2,GitPatch [输入gp]
//3,GitUpdate [输入gl]
//";
            Console.WriteLine(mes);
            Console.Write("请输入选择，然后回车：");
            CommonBase commonBase = null;
            var haha = Console.ReadLine();
            switch (haha)
            {
                case "sm":
                    commonBase = new SvnMaster();
                    break;
                case "sp":
                    commonBase = new SvnPatch();
                    break;
                case "sl":
                    commonBase = new SvnUpdate();
                    break;
                case "gm":
                    commonBase = new GitMaster();
                    break;
                case "gp":
                    commonBase = new GitPatch();
                    break;
                case "gl":
                    commonBase = new GitUpdate();
                    break;
            }

            if (commonBase == null) return;

            while (isRuning)
            {
                if (commonBase.isInstall)
                    commonBase.Run();
                Console.WriteLine("按y键继续,按其余键退出......");
                isRuning = Console.ReadLine() == "y";
            }
        }
    }
}
