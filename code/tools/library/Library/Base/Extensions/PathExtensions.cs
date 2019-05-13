using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Extensions
{
    public static class PathExtensions
    {
        public static string GetFilePathWithoutExtension(this FileInfo fileInfo)
        {
            return Path.Combine(fileInfo.DirectoryName, fileInfo.Name);
        }


        public static void ForEachPaths(this IEnumerable<string> paths, Action<string> callAction)
        {
            paths.Select(p => p.Replace("\\", "/").TrimStart('/')).ToList().ForEach((p, i, target) =>
            {
                SystemConsole.SetInfo(string.Format("is now : {0} {1}", (((float) i)/target.Count).ToString("p"), p));
                //Console.WriteLine("is now : " + (((float) i)/target.Count).ToString("p") + "\t" + p);
                if (File.Exists(p)) callAction(p);
            });
        }
    }
}