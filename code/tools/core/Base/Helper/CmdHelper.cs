﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Library.Extensions;

namespace Library.Helper
{
    public abstract class CmdHelper
    {
        private static Process GetProcess(string input)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动
            process.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
            process.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息
            process.StartInfo.RedirectStandardError = true; //重定向标准错误输出
            process.StartInfo.CreateNoWindow = true; //不显示程序窗口

            process.Start(); //启动程序
            process.StandardInput.WriteLine(input + "&exit");
            process.StandardInput.AutoFlush = true;
            return process;
        }


        public virtual string[] CmdReadAll(string input)
        {
            var process = GetProcess(input);
            try
            {
                string output = process.StandardOutput.ReadToEnd();
                var res = output.ToStringArray('\r', '\n').Where(p => !string.IsNullOrEmpty(p)).ToList();

                process.WaitForExit();

                Console.WriteLine(res[2].Replace("&exit", ""));
                return res.Skip(3).ToArray();
            }
            finally
            {
                process.Close();
            }
        }

        public virtual string[] CmdReadLine(string input)
        {
            var process = GetProcess(input);
            try
            {
                var res = new List<string>();
                StreamReader reader = process.StandardOutput;
                while (!reader.EndOfStream)
                {
                    res.Add(reader.ReadLine());
                }

                process.WaitForExit();

                res = res.Where(p => !string.IsNullOrEmpty(p)).ToList();
                Console.WriteLine(res[2].Replace("&exit", ""));
                return res.Skip(3).ToArray();
            }
            finally
            {
                process.Close();
            }
        }
    }
}