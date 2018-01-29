using System;
using System.Collections.Generic;
using System.IO;
using svnVersion;

namespace svnVersion
{
    internal class Program
    {
        public static bool Runing = true;

        private static void Main(string[] args)
        {
            CmdHelp cmdHelp = null;
            var haha = Console.ReadLine();
            switch (haha)
            {
                case "master":
                case "m":
                    cmdHelp = new SvnMaster();
                    break;
                case "patch":
                case "p":
                    cmdHelp = new SvnPatch();
                    break;
            }

            if (cmdHelp != null)
            {
                while (Runing)
                {
                    cmdHelp.HaHa();
                }
            }
        }
    }
}
