using Library.Helper;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Library
{
    [InitializeOnLoad]
    public class CommandLine
    {
        static CommandLine()
        {
            foreach (var item in Environment.GetCommandLineArgs().Where(p => p.StartsWith("--")).Select(p => p.Split('=')))
            {
                var key = item.FirstOrDefault().TrimStart('-').TrimStart('-');
                var value = item.LastOrDefault();
                Environment.SetEnvironmentVariable(key.ToLower(), string.IsNullOrEmpty(value) ? null : value);
            }
            if (Environment.GetEnvironmentVariables() is Hashtable hashtable)
            {
                var content = JsonHelper.ToJson(hashtable, indentLevel: 2);
                Debug.LogFormat("[CommandLine]\n{0}", content);
                File.WriteAllText("CommandLine.json", content);
            }
        }
    }
}