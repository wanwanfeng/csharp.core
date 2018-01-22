using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Debug = UnityEngine.Debug;

/// VersionMgr本地路径可解析版
namespace SVN.First
{
    public class VersionMgr : MonoBehaviour
    {
        private class ResInfo
        {
            //Environment.CurrentDirectory + "/";
            public static readonly string LocalRoot = Application.persistentDataPath + "/Temp/";

            /// <summary>
            /// svn版本
            /// </summary>
            public static string SvnVersion { get; set; }

            /// <summary>
            /// svn主资源起始版本号
            /// </summary>
            public static int MinVersion { get; set; }

            /// <summary>
            /// svn主资源结束版本号
            /// </summary>
            private static int maxVersion;

            public static int MaxVersion
            {
                get { return Math.Max(maxVersion, MinVersion); }
                set { maxVersion = value; }
            }

            public enum Source
            {
                Master,
                Patch,
                Local
            }

            public enum Action
            {
                N,
                A, //add
                M, //modified
                D, //delete
            }

            public string path;
            public string fileName;
            public long size;

            private string action;

            /// <summary>
            /// 资源主版本号，不大于svn主资源起始版本号
            /// </summary>
            private int masterVersion;

            /// <summary>
            /// 资源补丁版本号，不大于svn主资源结束版本号，但大于svn主资源起始版本号
            /// </summary>
            private int pacthVersion;

            public Action fileAction
            {
                get
                {
                switch (action)
                {
                    case "A":
                        return Action.A;
                    case "M":
                        return Action.M;
                    case "D":
                        return Action.D;
                    default:
                        return Action.N;
                }
                }
            }

            public int version
            {
                get { return pacthVersion > masterVersion ? pacthVersion : masterVersion; }
            }

            public string formatPath
            {
                get { return string.Format("{0}@{1}", path, version); }
            }

            public string url
            {
                get { return string.Format("{0}?v={1}", path, version); }
            }

            public ResInfo(string info, Source source)
            {
            switch (source)
            {
                case Source.Master:
                    {
                    var queue = new Queue<string>(info.Split(','));
                    pacthVersion = masterVersion = queue.Count == 0 ? 0 : queue.Dequeue().ToInt();
                    size = queue.Count == 0 ? 0 : queue.Dequeue().ToLong();
                    path = queue.Count == 0 ? "" : queue.Dequeue().Replace("\\", "/").Trim();
                    }
                    break;
                case Source.Patch:
                    {
                    var queue = new Queue<string>(info.Split(','));
                    masterVersion = MinVersion;
                    pacthVersion = queue.Count == 0 ? masterVersion : queue.Dequeue().ToInt();
                    action = queue.Count == 0 ? "" : queue.Dequeue();
                    size = queue.Count == 0 ? 0 : queue.Dequeue().ToLong();
                    path = queue.Count == 0 ? "" : queue.Dequeue().Replace("\\", "/").Trim();

                    pacthVersion = Mathf.Clamp(pacthVersion, masterVersion, MaxVersion);
                    }
                    break;
                case Source.Local:
                    {
                    var queue = new Queue<string>(info.Split('@'));
                    path = queue.Count == 0 ? "" : queue.Dequeue().Replace("\\", "/").Replace(LocalRoot, "").Trim();
                    pacthVersion = masterVersion = queue.Count == 0 ? 0 : queue.Dequeue().ToInt();
                    size = new FileInfo(info).Length;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("source", source, null);
            }
            fileName = Path.GetFileName(path);
            }

            public override string ToString()
            {
            return string.Format("Path: {0}, Size: {1}, Action: {2}, version: {3}", path, size, action, version);
            }
        }

        public string url = "http://192.168.8.60:8080/test/master/";

        private Dictionary<string, ResInfo> masterCache { get; set; }
        private Dictionary<string, ResInfo> patchCache { get; set; }

        /// <summary>
        /// 本地路径可以解析
        /// </summary>
        private Dictionary<string, List<ResInfo>> localCache { get; set; }

        public GameObject LoadGameObject(string path)
        {
            ResInfo resInfo = null;
            if (masterCache.TryGetValue(path, out resInfo))
            {
                if (resInfo.fileAction == ResInfo.Action.D)
                {
                    Debug.LogError("访问已被删除资源!");
                    return null;
                }
                return new GameObject(path);
            }
            Debug.LogError("访问不被控制的资源!");
            return null;
        }

        void Awake()
        {
            if (!Directory.Exists(ResInfo.LocalRoot))
                Directory.CreateDirectory(ResInfo.LocalRoot);

            localCache =
                Directory.GetFiles(ResInfo.LocalRoot, "*.*", SearchOption.AllDirectories)
                    .Select(p => new ResInfo(p, ResInfo.Source.Local))
                    .Where(p => !p.fileName.StartsWith("."))
                    .ToLookup(p => p.path)
                    .ToDictionary(p => p.Key, q => new List<ResInfo>(q));
        }

        IEnumerator Start()
        {
            yield return StartCoroutine(GetMasterCache("svn-master.txt"));
            yield return StartCoroutine(GetPatchCache("svn-patch.txt"));
            CompareMasterAndPatch();
            //Debug.Log(string.Join("\r\n", masterCache.Keys.Take(20).ToArray()));
            //Debug.Log(string.Join("\r\n", patchCache.Keys.Take(20).ToArray()));
            //Debug.Log(string.Join("\r\n", localCache.Keys.Take(20).ToArray()));
            yield return StartCoroutine(ResourceDelete());
            yield return StartCoroutine(ResourceDownLoad());
        }

        private IEnumerator GetMasterCache(string fileName)
        {
            using (WWW www = new WWW(url + fileName))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    var text =
                        www.text.Split('\r', '\n').Where(p => !string.IsNullOrEmpty(p)).Select(p => p.Trim()).ToArray();
                    ResInfo.SvnVersion = text.First();
                    ResInfo.MinVersion = text.Skip(1).First().ToInt();
                    masterCache =
                        text.Skip(2).Select(p => new ResInfo(p, ResInfo.Source.Master))
                            .Where(p => !p.fileName.StartsWith("."))
                            .ToDictionary(p => p.path);
                }
                else
                {
                    masterCache = new Dictionary<string, ResInfo>();
                }
            }
        }

        private IEnumerator GetPatchCache(string fileName)
        {
            using (WWW www = new WWW(url + fileName))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    var text =
                        www.text.Split('\r', '\n').Where(p => !string.IsNullOrEmpty(p)).Select(p => p.Trim()).ToArray();
                    var svnVersion = text.First();
                    var minVersion = text.Skip(1).First().ToInt();
                    if (ResInfo.SvnVersion != svnVersion || ResInfo.MinVersion != minVersion)
                    {
                        patchCache = new Dictionary<string, ResInfo>();
                    }
                    else
                    {
                        ResInfo.MaxVersion = text.Skip(2).First().ToInt();
                        patchCache =
                            text.Skip(3).Select(p => new ResInfo(p, ResInfo.Source.Patch))
                                .Where(p => !p.fileName.StartsWith("."))
                                .ToDictionary(p => p.path);
                    }
                }
                else
                {
                    patchCache = new Dictionary<string, ResInfo>();
                }
            }
        }

        private void CompareMasterAndPatch()
        {
            foreach (var pair in patchCache)
            {
                ResInfo localValue = null;
                if (masterCache.TryGetValue(pair.Key, out localValue))
                {
                    if (pair.Value.fileAction == ResInfo.Action.D)
                        masterCache.Remove(pair.Key);
                    else
                        masterCache[pair.Key] = pair.Value;
                }
                else
                {
                    masterCache.Add(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// 删除多余资源,清理空间
        /// </summary>
        /// <returns></returns>
        private IEnumerator ResourceDelete()
        {
            var delList =
                localCache.Values.SelectMany(p => p.Select(q => q.formatPath))
                    .Except(masterCache.Values.Select(p => p.formatPath))
                    .ToList();
            Debug.Log(delList.Count + "\r\n" + string.Join("\n", delList.Select(p => p.ToString()).ToArray()));

            foreach (var path in delList)
            {
                try
                {
                    if (File.Exists(ResInfo.LocalRoot + path))
                        File.Delete(ResInfo.LocalRoot + path);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    throw;
                }
            }
            yield break;
        }

        /// <summary>
        /// 本地没有的或版本较低的需形成下载列表
        /// </summary>
        /// <returns></returns>
        private IEnumerator ResourceDownLoad()
        {
            var downList = new List<ResInfo>();
            foreach (var pair in masterCache)
            {
                List<ResInfo> localValue = null;
                if (localCache.TryGetValue(pair.Key, out localValue))
                {
                    if (localValue.Select(p => p.version).Contains(pair.Value.version))
                        continue;
                    downList.Add(pair.Value);
                }
                else
                {
                    downList.Add(pair.Value);
                }
            }

            Debug.Log(downList.Count + "\r\n" + string.Join("\n", downList.Select(p => p.ToString()).ToArray()));
            //yield break;
            var lastList = new Queue<ResInfo>(downList);
            while (lastList.Count != 0)
            {
                var resInfo = lastList.Dequeue();
                using (WWW www = new WWW(url + resInfo.url))
                {
                    Debug.Log(string.Format("down...{0}...{1}", (float) lastList.Count/downList.Count, www.url));
                    yield return www;
                    if (!string.IsNullOrEmpty(www.error)) continue;
                    string dir = Path.GetDirectoryName(ResInfo.LocalRoot + resInfo.formatPath);
                    if (dir != null && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    File.WriteAllBytes(ResInfo.LocalRoot + resInfo.formatPath, www.bytes);
                    //lastList.Dequeue();
                }
            }
        }


        void OnGUI()
        {
            GUILayout.Label("SvnVersion:" + ResInfo.SvnVersion);
            GUILayout.Label("MinVersion:" + ResInfo.MinVersion);
            GUILayout.Label("MaxVersion:" + ResInfo.MaxVersion);
        }
    }
}