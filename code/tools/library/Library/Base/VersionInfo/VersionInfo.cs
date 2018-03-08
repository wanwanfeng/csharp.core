using System.Collections.Generic;
using Library.Extensions;

namespace FileVersion
{
    public class FileDetailInfo
    {
        public string path;
        public string path_md5;
        public string action;
        public string version;
        public string content_hash;
        public string content_size;
        public string encrypt_hash;
        public string encrypt_size;

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", path_md5, action, version, content_hash,
                content_size,
                encrypt_hash, encrypt_size, path);
        }
    }

    public class FilePatchInfo
    {
        public string group;
        public int firstVersion;
        public int lastVersion;
        public string path;
        public string content_hash;
        public string content_size;
        public string zip_hash;
        public string zip_size;

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", group, firstVersion, lastVersion, path, content_hash,
                content_size, zip_hash, zip_size);
        }
    }

    public class VersionInfo
    {
        public string softwareVersion;
        public List<FilePatchInfo> pathInfos;

        /// <summary>
        /// App版本比较
        /// </summary>
        /// <param name="oldV">0.0.0</param>
        /// <param name="newV">1.1.1</param>
        /// <param name="separator">,</param>
        /// <returns></returns>
        public static bool IsCanDownLoad(string oldV, string newV, params char[] separator)
        {
            var runArray = new Queue<int>(oldV.AsIntArray(separator));
            var remoteArray = new Queue<int>(newV.AsIntArray(separator));
            while (runArray.Count > 0 || remoteArray.Count > 0)
            {
                var run = runArray.Count == 0 ? 0 : runArray.Dequeue();
                var remote = remoteArray.Count == 0 ? 0 : remoteArray.Dequeue();
                if (run == remote) continue;
                return run < remote;
            }
            return false;
        }
    }
}