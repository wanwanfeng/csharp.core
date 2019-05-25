
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using Library;
using Library.Extensions;
using search.Script;

namespace search
{
    internal static class Program
    {
        public enum ConvertType
        {
            [TypeValue(typeof (SearchIP))] [Description("获取代理IP")] SearchIP,
            [TypeValue(typeof (SearchMovie))] [Description("SearchMovie")] SearchMovie,
            [TypeValue(typeof (SearchIndexM3U8))] [Description("SearchIndexM3U8")] SearchIndexM3U8,
        }

        private static void Main(string[] args)
        {
            SystemConsole.Run<ConvertType>();
        }

        public static void ForEachPaths(this IEnumerable<string> paths, Action<string> callAction)
        {
            paths.Select(p => p.Replace("\\", "/")).ToList().ForEach((p, i, target) =>
            {
                Thread.Sleep(1000);
                SystemConsole.SetProgress(p, ((float)i) / target.Count);
                callAction(p);
            });
            SystemConsole.ClearProgress();
        }
    }
}
