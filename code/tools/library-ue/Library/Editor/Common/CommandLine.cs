using Library.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Library
{
    [InitializeOnLoad]
    public class CommandLine
    {
        public static CommandLine Args { get; private set; }

        static CommandLine()
        {
            var lookup = Environment.GetCommandLineArgs()
                .Where(p => p.StartsWith("--"))
                .Select(p => p.Split('='))
                .Select(p => new { k = p.FirstOrDefault().ToLower(), v = p.LastOrDefault() })
                .GroupBy(p => p.k, p => p.v)
                .ToDictionary(p => p.Key, p => p.FirstOrDefault());

            Debug.LogFormat("[CommandLine]\n{0}", Args = new CommandLine(lookup));
        }
        private Dictionary<string, string> dict = new Dictionary<string, string>();
        private CommandLine(Dictionary<string, string> lookup)
        {
            dict = lookup;
        }
        public bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }
        public string this[string key, string defaultValue = ""]
        {
            get
            {
                if (ContainsKey(key))
                    return dict[key];
                return defaultValue;
            }
        }

        public int this[string key, int defaultValue = 0]
        {
            get
            {
                if (ContainsKey(key))
                {
                    var val = 0;
                    if (int.TryParse(dict[key], out val))
                    {
                        defaultValue = val;
                    }
                }
                return defaultValue;
            }
        }

        public bool this[string key, bool defaultValue = false]
        {
            get
            {
                if (ContainsKey(key))
                {
                    var val = false;
                    if (bool.TryParse(dict[key], out val))
                    {
                        defaultValue = val;
                    }
                }
                return defaultValue;
            }
        }

        public string Check(string key)
        {
            if (ContainsKey(key))
                return dict[key];
            throw new Exception("this key is not define!");
        }

        public override string ToString()
        {
            return JsonHelper.ToJson(dict, indentLevel: 2);
        }
    }
}
