using System;
using System.IO;
using System.Linq;
using System.Reflection;
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

            } while (SystemExtensions.ContinueY("按y键继续,按其余键退出......"));
        }
    }
}
