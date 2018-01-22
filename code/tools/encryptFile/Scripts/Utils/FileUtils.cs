using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Encrypt
{
    internal class FileUtils
    {
        public static List<string> GetFiles(string[] rootPath, SearchOption searchOption, params string[] exclude)
        {
            var files =
                rootPath.SelectMany(p => Directory.GetFiles(p, "*.*", searchOption))
                    .Distinct()
                    .Select(p => p.Replace("\\", "/"))
                    .ToList();
            if (exclude.Length == 0) return files.ToList();
            var excludeList = new List<string>();
            foreach (var s in exclude)
                excludeList.AddRange(files.Where(p => p.ToLower().EndsWith(s.ToLower())));
            return files.Except(excludeList).ToList();
        }


        public static List<string> GetFiles(string rootPath, SearchOption searchOption, params string[] exclude)
        {
            var files = Directory.GetFiles(rootPath, "*.*", searchOption)
                .Distinct()
                .Select(p => p.Replace("\\", "/"))
                .ToList();
            if (exclude.Length == 0) return files.ToList();
            var excludeList = new List<string>();
            foreach (var s in exclude)
                excludeList.AddRange(files.Where(p =>
                {
                    var extension = Path.GetExtension(p);
                    return extension != null && extension.ToLower().Equals(s.ToLower());
                }));
            return files.Except(excludeList).ToList();
        }

        public static Queue<string> GetQueue(string path, int depth = 0)
        {
            var res = new Queue<string>(path.Split('/'));
            var queue = new Queue<string>();
            for (var i = 0; i < depth; i++)
            {
                if (res.Count == 0) break;
                queue.Enqueue(res.Dequeue());
            }
            if (res.Count != 0)
                queue.Enqueue(string.Join("/", res.ToArray()));
            return queue;
        }
    }
}
