using System;
using Library.Extensions;

namespace SvnVersion
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            do
            {
                Console.Clear();
                CommonBase commonBase = new SvnList();
                if (commonBase.isInstall)
                    commonBase.Run();

            } while (SystemConsole.ContinueY("按任意键继续，e键退出......"));
        }
    }
}
