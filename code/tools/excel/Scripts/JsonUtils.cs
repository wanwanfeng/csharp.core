﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;
using Library.Helper;
using PLitJson;

namespace Script
{
    public class BaseCheck : BaseSystemConsole
    {
        public void Check(string selectExtension, Action<string> callAction)
        {
            var res = new ConcurrentBag<string>();
            Action<string> action = file =>
            {
                Console.WriteLine(" is now : " + file);
                try
                {
					callAction?.Invoke(file);
				}
                catch (Exception)
                {
                    res.Add(file);
                }
            };

            //Parallel.ForEach(CheckPath(selectExtension), action);//并行操作
            CheckPath(selectExtension).AsParallel().ForAll(action); //并行操作
            //CheckPath(".json").ForEach(action);//线性操作

            WriteError(res.Select(p => p.Replace("/", "\\")));
        }
    }

    public class CheckJson : BaseCheck
    {
        public CheckJson()
        {
            Check(".json|.txt", file =>
            {
                JsonData json = JsonHelper.ToObject(File.ReadAllText(file).Trim('\0').Trim());
                JsonHelper.ToJson(json, false);
            });
        }
    }

    public class IndentJson : BaseCheck
    {
        public IndentJson()
        {
            Check(".json|.txt", file =>
            {
                JsonData json = JsonHelper.ToObject(File.ReadAllText(file).Trim('\0').Trim());
                string jsonStr = JsonHelper.ToJson(json, indentLevel: 2);
                File.WriteAllText(file, jsonStr, new UTF8Encoding(false));
            });
        }
    }

    public class CancelIndentJson : BaseCheck
    {
        public CancelIndentJson()
        {
            Check(".json|.txt", file =>
            {
                JsonData json = JsonHelper.ToObject(File.ReadAllText(file).Trim('\0').Trim());
                string jsonStr = JsonHelper.ToJson(json, isUnicode: false);
                File.WriteAllText(file, jsonStr, new UTF8Encoding(false));
            });
        }
    }
}
