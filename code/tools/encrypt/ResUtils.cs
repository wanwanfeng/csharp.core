using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace encrypt
{
    class ResInfo
    {
        public readonly string LocalRoot = Environment.CurrentDirectory + "/";

        public enum Source
        {
            Master,
            Patch,
            Local
        }

        public Source source = Source.Local;
        public string path;
        public string action;
        public int version;
        public int pacthVersion;
        public long size;

        public ResInfo(string info, Source source)
        {
            switch (source)
            {
                case Source.Master:
                {
                    var queue = new Queue<string>(info.Split(','));
                    version = queue.Count == 0 ? 0 : int.Parse(queue.Dequeue());
                    size = queue.Count == 0 ? 0 : long.Parse(queue.Dequeue());
                    path = queue.Count == 0 ? "" : queue.Dequeue();
                }
                    break;
                case Source.Patch:
                {
                    var queue = new Queue<string>(info.Split(','));
                    version = queue.Count == 0 ? 0 : int.Parse(queue.Dequeue());
                    pacthVersion = queue.Count == 0 ? 0 : int.Parse(queue.Dequeue());
                    action = queue.Count == 0 ? "" : queue.Dequeue();
                    size = queue.Count == 0 ? 0 : long.Parse(queue.Dequeue());
                    path = queue.Count == 0 ? "" : queue.Dequeue();
                }
                    break;
                case Source.Local:
                {
                    var queue = new Queue<string>(info.Split('@'));
                    path = info.Replace("\\", "/").Replace(LocalRoot, "");
                    version = queue.Count == 0 ? 0 : int.Parse(queue.Dequeue());
                    size = new FileInfo(info).Length;
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("source", source, null);
            }
        }
    }

    class ResUtils
    {
        public ResUtils()
        {
            var masterArray = File.ReadAllLines("").Select(p => new ResInfo(p, ResInfo.Source.Master));
            var patchArray = File.ReadAllLines("").Select(p => new ResInfo(p, ResInfo.Source.Patch));
            var masterCache = masterArray.ToDictionary(p => p.path, q => q);
            var patchCache = patchArray.ToDictionary(p => p.path, q => q);
            foreach (var pair in patchCache) masterCache[pair.Key] = pair.Value;
            var fileArray = Directory.GetFiles("", ".*", SearchOption.AllDirectories);
            var localArray = fileArray.Select(p => new ResInfo(p, ResInfo.Source.Local));
            var localCache = localArray.ToDictionary(p => p.path, q => q);

            //本地没有的或版本较低的需形成下载列表
            var downList = new List<ResInfo>();
            foreach (var pair in masterCache)
            {
                ResInfo localValue = null;
                if (localCache.TryGetValue(pair.Key, out localValue))
                {
                    if (pair.Value.version > localValue.version)
                        downList.Add(pair.Value);
                }
                else
                {
                    downList.Add(pair.Value);
                }
                masterCache[pair.Key] = pair.Value;
            }
        }
    }
}
