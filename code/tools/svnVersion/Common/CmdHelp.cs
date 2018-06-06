﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;

namespace SvnVersion
{
    public abstract class CmdHelp
    {
        protected System.Diagnostics.Process process;

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


        protected void StartCmd()
        {
            process = new System.Diagnostics.Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动
            process.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
            process.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息
            process.StartInfo.RedirectStandardError = true; //重定向标准错误输出
            process.StartInfo.CreateNoWindow = true; //不显示程序窗口
        }

        //向cmd窗口发送输入信息
        protected void EndCmd()
        {
            if (process == null || process.HasExited) return;
            process.Start(); //启动程序
            process.StandardInput.WriteLine("exit");
            process.StandardInput.AutoFlush = true;
            process.WaitForExit(); //等待程序执行完退出进程
            process.Close();
        }


        public virtual string[] CmdReadAll(string input)
        {
            process.Start(); //启动程序
            process.StandardInput.WriteLine(input + "&exit");
            process.StandardInput.AutoFlush = true;

            string output = process.StandardOutput.ReadToEnd();
            var res = new List<string>(output.AsStringArray('\r', '\n'));

            process.WaitForExit();

            res = res.Where(p => !string.IsNullOrEmpty(p)).ToList();
            Console.WriteLine(res[2].Replace("&exit", ""));
            res = res.Skip(3).ToList();
            return res.ToArray();
        }

        public virtual string[] CmdReadLine(string input)
        {
            process.Start(); //启动程序
            process.StandardInput.WriteLine(input + "&exit");
            process.StandardInput.AutoFlush = true;

            var res = new List<string>();
            StreamReader reader = process.StandardOutput;
            while (!reader.EndOfStream)
            {
                res.Add(reader.ReadLine());
            }

            process.WaitForExit();

            res = res.Where(p => !string.IsNullOrEmpty(p)).ToList();
            Console.WriteLine(res[2].Replace("&exit", ""));
            res = res.Skip(3).ToList();
            return res.ToArray();
        }


        public void PrintPercentage(int finishedCount, int totalCount)
        {
            decimal finishedPercentage = Convert.ToDecimal(finishedCount) / Convert.ToDecimal(totalCount);
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine((finishedPercentage * 100).ToString("f1") + "%");
        } 

    }
}