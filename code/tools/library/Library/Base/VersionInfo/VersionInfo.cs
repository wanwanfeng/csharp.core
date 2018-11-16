using System.Collections.Generic;
using Library.Extensions;

namespace FileVersion
{
    public abstract class BaseFileInfo
    {
        public string path;
        public string content_hash;
        public long content_size;
        public string encrypt_hash;
        public long encrypt_size;

        /// <summary>
        /// 是否加密
        /// </summary>
        public bool IsEncrypt()
        {
            if (string.IsNullOrEmpty(content_hash))
                return false;
            if (string.IsNullOrEmpty(encrypt_hash))
                return false;
            return content_hash != encrypt_hash;
        }
    }

    public class FileDetailInfo : BaseFileInfo
    {
        public bool is_delete;

        //实际版本号
        public long version;
        //修订版本号
        public long revision;

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", is_delete, version, revision, content_hash,
                content_size, encrypt_hash, encrypt_size, path);
        }

        public static implicit operator string(FileDetailInfo info)
        {
            return info.ToString();
        }
    }

    public class FilePatchInfo : BaseFileInfo
    {
        public string group;
        public int firstVersion;
        public int lastVersion;
        public string zip_hash;
        public long zip_size;

        //是否是压缩包
        public bool IsZip()
        {
            return !string.IsNullOrEmpty(zip_hash);
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", group, firstVersion, lastVersion, path,
                content_hash,
                content_size, encrypt_hash, encrypt_size, zip_hash, zip_size);
        }

        public static implicit operator string(FilePatchInfo info)
        {
            return info.ToString();
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
            //var runIt = oldV.AsIntArray(separator).GetEnumerator();
            //var remoteIt = newV.AsIntArray(separator).GetEnumerator();
            //do
            //{
            //    runIt.MoveNext();
            //    var run = (int) (runIt.Current ?? 0);
            //    remoteIt.MoveNext();
            //    var remote = (int) (remoteIt.Current ?? 0);
            //    if (run == remote) continue;
            //    return run < remote;
            //} while (runIt.Current != null || remoteIt.Current != null);
            //return false;

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