using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace webScss
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //args = new[] { @"base.scss", @"D:\Work\mfxy\client\tw\magica" };
            //args = new[] { @"D:\Work\mfxy\client\tw\magica\sass\_common\base.scss", @"D:\Work\mfxy\client\tw\magica" };
            //File.WriteAllText("ssss.txt", string.Join(",", args) + "," + Environment.CurrentDirectory);

            string filePath = args.FirstOrDefault();
            if (filePath == null) return;
            if (!File.Exists(filePath)) return;
            {
                try
                {
                    filePath = new FileInfo(filePath).FullName;
                    string outPath = filePath.Replace("\\", "/").Replace("/sass/", "/css/").Replace("/scss/", "/css/");
                    var directoryName = new FileInfo(outPath).DirectoryName;
                    if (directoryName != null && !Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);
                    string cmdpath = @"c:\Ruby24-x64\bin\sass.bat";
                    string paramss = string.Format("--no-cache \"{0}\" \"{1}\"", filePath,
                        Path.ChangeExtension(outPath, ".css"));
                    StartCmd(cmdpath, paramss);
                    var map = Path.ChangeExtension(outPath, ".css.map");
                    if (File.Exists(map))
                        File.Delete(map);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
            string projectPath = args.Skip(1).FirstOrDefault();
            if (string.IsNullOrEmpty(projectPath)) return;
            if (!Directory.Exists(projectPath)) return;
                webUtils.Program.Main(new[] { projectPath });
        }

        protected static void StartCmd(string path,string param)
        {
           var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = param;
            process.StartInfo.CreateNoWindow = true; //不显示程序窗口
            process.Start(); //启动程序
            process.WaitForExit(); //等待程序执行完退出进程
            process.Close();
        }
    }
}
