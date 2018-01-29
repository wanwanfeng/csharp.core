using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Helper;

namespace svnVersion
{
    public abstract class CmdHelp
    {
        protected System.Diagnostics.Process p = new System.Diagnostics.Process();

        //public string Run(string input)
        //{
        //    p.StartInfo.FileName = "cmd.exe";
        //    p.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动
        //    p.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
        //    p.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息
        //    p.StartInfo.RedirectStandardError = true; //重定向标准错误输出
        //    p.StartInfo.CreateNoWindow = true; //不显示程序窗口
        //    p.Start(); //启动程序

        //    //向cmd窗口发送输入信息
        //    p.StandardInput.WriteLine(input + "&exit");

        //    p.StandardInput.AutoFlush = true;
        //    //p.StandardInput.WriteLine("exit");
        //    //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
        //    //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

        //    //获取cmd窗口的输出信息
        //    string output = p.StandardOutput.ReadToEnd();

        //    //StreamReader reader = p.StandardOutput;
        //    //string line=reader.ReadLine();
        //    //while (!reader.EndOfStream)
        //    //{
        //    //    str += line + "  ";
        //    //    line = reader.ReadLine();
        //    //}

        //    p.WaitForExit(); //等待程序执行完退出进程
        //    p.Close();

        //    return output;
        //}


        protected CmdHelp()
        {
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true; //重定向标准错误输出
            p.StartInfo.CreateNoWindow = true; //不显示程序窗口
        }

        //向cmd窗口发送输入信息
        ~CmdHelp()
        {
            Console.ReadKey();
            p.Start(); //启动程序
            p.StandardInput.WriteLine("exit");
            p.StandardInput.AutoFlush = true;
            p.WaitForExit(); //等待程序执行完退出进程
            p.Close();
        }

        public string[] Run(string input)
        {
            p.Start(); //启动程序
            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(input + "&exit");
            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令
            p.WaitForExit();
            //获取cmd窗口的输出信息
            StreamReader reader = p.StandardOutput;
            List<string> res = new List<string>() {reader.ReadLine()};
            while (!reader.EndOfStream)
            {
                res.Add(reader.ReadLine());
            }
            Console.WriteLine("");
            Console.WriteLine(res[3].Replace("&exit",""));
            return res.Skip(4).ToArray();
        }


        public virtual bool HaHa()
        {
            return true;
        }

        protected bool PathToMd5(string folder, string targetDir, List<List<string>> cao)
        {
            if (folder == null) return false;
            Console.Write("\n是否将路径MD5化（y/n），然后回车：");
            bool yes = Console.ReadLine() == "y";
            if (!yes) return false;
            var targetMd5Dir = targetDir.Replace(folder, Library.Encrypt.MD5.Encrypt(folder));
            foreach (var s in cao)
            {
                string fullPath = targetDir + "/" + s.Last();
                string targetFullPath = targetMd5Dir + "/" + Library.Encrypt.MD5.Encrypt(s.Last());
                if (!File.Exists(fullPath)) continue;
                FileHelper.CreateDirectory(targetFullPath);
                File.Copy(fullPath, targetFullPath, true);
            }
            if (File.Exists(targetDir + ".txt"))
            {
                File.Move(targetDir + ".txt", targetMd5Dir + ".txt");
            }
            return true;
        }
    }
}
