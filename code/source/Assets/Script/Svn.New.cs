using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CustomExtensions;
using Debug = UnityEngine.Debug;

//增量更新
//每更新一次，资源放在新的文件夹目录内，老资源不会被覆盖
//目录结构如下
//
//│  
//├─master-00-02
//│      svn-master.txt
//│      
//├─patch-02-09
//│      svn-patch.txt
//│      
//├─patch-09-20
//│      svn-patch.txt
//│      
//├─patch-20-23
//│      svn-patch.txt
//│      
//├─patch-23-33
//│      svn-patch.txt
//│      
//└─patch-33-40
//        svn-patch.txt
//

namespace SVN.NEW
{
    public class VersionMgr : MonoBehaviour
    {
        private class Access
        {
            private static Queue<string> urls = new Queue<string>();

            private static readonly int RetryCount = 5;

            static Access()
            {
                urls.Enqueue("http://192.168.8.60:8080/test/master/");
                urls.Enqueue("http://192.168.8.60:8080/test/master/");
                urls.Enqueue("http://192.168.8.60:8080/test/master/");
            }

            private int retry = 0;

            public Access()
            {
                retry = 0;
            }

            public IEnumerator GetResult(MonoBehaviour mono, string path, Action<byte[]> callAction)
            {
                using (WWW www = new WWW(urls.Peek() + path))
                {
                    yield return www;
                    if (string.IsNullOrEmpty(www.error))
                    {
                        callAction.Invoke(www.bytes);
                    }
                    else
                    {
                        if (retry < RetryCount)
                        {
                            retry++;
                            urls.Enqueue(urls.Dequeue());
                            yield return mono.StartCoroutine(GetResult(mono, path, callAction));
                        }
                        else
                        {
                            callAction.Invoke(null);
                        }
                    }
                    www.Dispose();
                }
            }
        }

        private class PatchListInfo
        {
            public bool isMaster; //主与补丁区分
            public int first; //开始版本号
            public int last; //结束版本好
            public string url; //远程结构
            public string local; //本地结构

            public string hashText; //对应的记录文本hash

            public PatchListInfo(string info)
            {
                var queue = new Queue<string[]>(info.Split(',').Select(p => p.Split('-')));
                var strFirst = queue.Dequeue();
                isMaster = strFirst[0] == "master";
                first = strFirst[1].AsInt();
                last = strFirst[2].AsInt();
                url = info + (isMaster ? "/svn-master.txt" : "/svn-patch.txt");
                local = url.Replace("/", "-");
                hashText = (queue.Count == 0 ? "" : queue.Dequeue().First()).Trim();
            }

            public override string ToString()
            {
                return string.Format("Url: {0}, HashText: {1}, IsMaster: {2}", url, hashText, isMaster);
            }
        }

        private class ResInfo
        {
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

            public string path { get; private set; }
            public string name { get; private set; }
            public int version { get; private set; }
            public long size { get; private set; }
            public string hash { get; private set; }

            /// <summary>
            /// 相对于旧得版本的文件操作行为
            /// </summary>
            private string action;

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

            /// <summary>
            /// 转化本地文件路径使用
            /// 和其余信息一起被格式化
            /// 最低包含文件名与版本信息
            /// </summary>
            public string formatPath { get; private set; }
            public string url { get; private set; }
            public int retryCount { get; set; }

            public ResInfo(string info, Source source)
            {
                switch (source)
                {
                    case Source.Master:
                        {
                            var queue = new Queue<string>(info.Split(','));
                            version = queue.Count == 0 ? 0 : queue.Dequeue().AsInt();
                            size = queue.Count == 0 ? 0 : queue.Dequeue().AsLong();
                            path = queue.Count == 0 ? "" : queue.Dequeue().Replace("\\", "/").Trim();
                        }
                        break;
                    case Source.Patch:
                        {
                            var queue = new Queue<string>(info.Split(','));
                            version = queue.Count == 0 ? 0 : queue.Dequeue().AsInt();
                            action = queue.Count == 0 ? "" : queue.Dequeue();
                            size = queue.Count == 0 ? 0 : queue.Dequeue().AsLong();
                            path = queue.Count == 0 ? "" : queue.Dequeue().Replace("\\", "/").Trim();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("source", source, null);
                }

                name = Path.GetFileName(path);
                url = string.Format("{0}?v={1}", path, version);
                formatPath = string.Format("{0}@{1}", path, version);
                formatPath = EncryptUtils.MD5.Encrypt(formatPath);
            }

            public override string ToString()
            {
                return string.Format("Path: {0}, Size: {1}, Action: {2}, version: {3}", path, size, action, version);
            }
        }

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

        public static string LocalRoot
        {
            get { return Application.persistentDataPath + "/Temp/"; }
        }

        private PatchListInfo LastAccessInfo { get; set; }
        private List<PatchListInfo> patchList { get; set; }
        private Dictionary<string, ResInfo> masterCache { get; set; }
        private Dictionary<string, ResInfo> patchCache { get; set; }

        /// <summary>
        /// 本地路径不可以解析
        /// 如文件名称与其余信息拼接后加密后形成新的名称
        /// </summary>
        private List<string> localCache { get; set; }

        private void Awake()
        {
            if (!Directory.Exists(LocalRoot))
                Directory.CreateDirectory(LocalRoot);

            localCache =
                Directory.GetFiles(LocalRoot, "*.*", SearchOption.AllDirectories)
                    .Select(p => p.Replace("\\", "/").Replace(LocalRoot, "").Trim())
                    .ToList();
        }

        private IEnumerator Start()
        {
            yield return StartCoroutine(GetPatchList("patch-list.txt"));
            foreach (var info in patchList)
            {
                if (LastAccessInfo != null && info.first != LastAccessInfo.last)
                    yield break;
                if (info.isMaster)
                {
                    yield return StartCoroutine(GetMasterCache(info));
                }
                else
                {
                    yield return StartCoroutine(GetPatchCache(info));
                    CompareMasterAndPatch();
                }
                LastAccessInfo = info;
            }
            yield return StartCoroutine(ResourceDelete());
            yield return StartCoroutine(ResourceDownLoad());
        }

        /// <summary>
        ///  下载记录补丁的文本获取补丁列表
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private IEnumerator GetPatchList(string fileName)
        {
            yield return StartCoroutine(new Access().GetResult(this, fileName, res =>
            {
                if (res != null)
                {
                    patchList = Encoding.UTF8.GetString(res)
                        .Split('\r', '\n')
                        .Where(p => !string.IsNullOrEmpty(p))
                        .Select(p => p.Trim())
                        .Select(p => new PatchListInfo(p))
                        .OrderBy(p => p.first)
                        .ToList();
                }
                else
                {
                    patchList = new List<PatchListInfo>();
                }
            }));
       }

        /// <summary>
        /// 下载记录主资源列表的文本
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private IEnumerator GetMasterCache(PatchListInfo info)
        {
            yield return StartCoroutine(GetCacheText(info, text =>
            {
                if (text == null)
                {
                    masterCache = new Dictionary<string, ResInfo>();
                    return;
                }
                SvnVersion = text.First();
                MinVersion = text.Skip(1).First().AsInt();
                MaxVersion = text.Skip(2).First().AsInt();
                masterCache =
                    text.Skip(3).Select(p => new ResInfo(p, ResInfo.Source.Master))
                        .Where(p => !p.name.StartsWith("."))
                        .ToDictionary(p => p.path);
            }));
        }

        /// <summary>
        /// 下载每一个补丁文本获取补丁文件列表
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private IEnumerator GetPatchCache(PatchListInfo info)
        {
            yield return StartCoroutine(GetCacheText(info, text =>
            {
                if (text == null)
                {
                    patchCache = new Dictionary<string, ResInfo>();
                    return;
                }
                var svnVersion = text.First();
                var minVersion = text.Skip(1).First().AsInt();
                if (SvnVersion != svnVersion || MaxVersion != minVersion)
                {
                    patchCache = new Dictionary<string, ResInfo>();
                    return;
                }
                MaxVersion = text.Skip(2).First().AsInt();
                patchCache = 
                    text.Skip(3).Select(p => new ResInfo(p, ResInfo.Source.Patch))
                    .Where(p => !p.name.StartsWith("."))
                    .ToDictionary(p => p.path);
            }));
        }

        private IEnumerator GetCacheText(PatchListInfo info, Action<string[]> callAction)
        {
            byte[] bytes = new byte[0];
            var path = Application.persistentDataPath + "/" + info.local;
            bool isDown = true;
            if (File.Exists(path))
            {
                bytes = File.ReadAllBytes(path);
                isDown = EncryptUtils.MD5.Encrypt(bytes) != info.hashText;
            }
            if (isDown)
            {
                yield return StartCoroutine(new Access().GetResult(this, info.url, res =>
                {
                    if (res == null || EncryptUtils.MD5.Encrypt(res) != info.hashText) return;
                    bytes = res;
                    File.WriteAllBytes(path, bytes);
                }));
            }
            if (bytes.Length == 0)
            {
                callAction.Invoke(null);
                yield break; // yield break;
            }
            callAction.Invoke(Encoding.UTF8.GetString(bytes)
                .Split('\r', '\n')
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => p.Trim())
                .ToArray());
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
            var delList = localCache.Except(masterCache.Values.Select(p => p.formatPath)).ToList();
            Debug.Log(delList.Count + "\r\n" + string.Join("\n", delList.Select(p => p.ToString()).ToArray()));

            //yield break;
            foreach (var path in delList)
            {
                try
                {
                    if (File.Exists(LocalRoot + path))
                        File.Delete(LocalRoot + path);
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
            var downList = masterCache.Values.Where(p => !localCache.Contains(p.formatPath)).ToList();
            Debug.Log(downList.Count + "\r\n" + string.Join("\n", downList.Select(p => p.ToString()).ToArray()));

            //yield break;
            var lastList = new Queue<ResInfo>(downList);
            while (lastList.Count != 0)
            {
                var resInfo = lastList.Dequeue();
                Debug.Log(string.Format("down...{0}...{1}", (float)lastList.Count / downList.Count, resInfo.url));
                yield return StartCoroutine(new Access().GetResult(this, resInfo.url, res =>
                {
                    if (res == null || EncryptUtils.MD5.Encrypt(res) != resInfo.hash) return;
                    string dir = Path.GetDirectoryName(LocalRoot + resInfo.formatPath);
                    if (dir != null && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    File.WriteAllBytes(LocalRoot + resInfo.formatPath, res);
                }));
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("LastAccess:" + LastAccessInfo);
            GUILayout.Label("SvnVersion:" + SvnVersion);
            GUILayout.Label("MinVersion:" + MinVersion);
            GUILayout.Label("MaxVersion:" + MaxVersion);
        }


        #region Load

        private IEnumerator LoadObject(string path, Action<WWW> callAction)
        {
            ResInfo resInfo = null;
            if (masterCache.TryGetValue(path, out resInfo))
            {
                if (resInfo.fileAction == ResInfo.Action.D)
                {
                    Debug.LogError("访问已被删除资源!");
                    yield break;
                }

                using (WWW www = new WWW(LocalRoot + resInfo.formatPath))
                {
                    yield return www;
                    callAction.Invoke(www);
                    www.Dispose();
                    yield break;
                }
            }
            Debug.LogError("访问不被控制的资源!");
            callAction.Invoke(null);
        }

        public IEnumerator Load(string path, Action<Texture2D> callAction)
        {
            yield return StartCoroutine(LoadObject(path, www =>
            { callAction.Invoke(www == null ? Resources.Load<Texture2D>(path) : www.texture); }));
        }

        public IEnumerator Load(string path, Action<AudioClip> callAction)
        {
            yield return StartCoroutine(LoadObject(path, www =>
            { callAction.Invoke(www == null ? Resources.Load<AudioClip>(path) : www.audioClip); }));
        }

        public IEnumerator Load(string path, Action<byte[]> callAction)
        {
            yield return StartCoroutine(LoadObject(path, www =>
            { callAction.Invoke(www == null ? Resources.Load<TextAsset>(path).bytes : www.bytes); }));
        }

        public IEnumerator Load(string path, Action<string> callAction)
        {
            yield return StartCoroutine(LoadObject(path, www =>
            { callAction.Invoke(www == null ? Resources.Load<TextAsset>(path).text : www.text); }));
        }

        public IEnumerator Load(string path, Action<MovieTexture> callAction)
        {
            yield return StartCoroutine(LoadObject(path,
                www => { callAction.Invoke(www == null ? Resources.Load<MovieTexture>(path) : www.movie); }));
        }

        public IEnumerator Load(string path, Action<AssetBundle> callAction)
        {
            yield return StartCoroutine(LoadObject(path, www => { callAction.Invoke(www.assetBundle); }));
        }

        #endregion
    }
}