using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace Svn.Three
{
    public class ResInfo
    {
        //Environment.CurrentDirectory + "/";
        public static readonly string LocalRoot = Application.persistentDataPath + "/Temp/";
        public static readonly int RetryCount = 5;

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
                    version = queue.Count == 0 ? 0 : queue.Dequeue().ToInt();
                    size = queue.Count == 0 ? 0 : queue.Dequeue().ToLong();
                    path = queue.Count == 0 ? "" : queue.Dequeue().Replace("\\", "/").Trim();
                }
                    break;
                case Source.Patch:
                {
                    var queue = new Queue<string>(info.Split(','));
                    version = queue.Count == 0 ? 0 : queue.Dequeue().ToInt();
                    action = queue.Count == 0 ? "" : queue.Dequeue();
                    size = queue.Count == 0 ? 0 : queue.Dequeue().ToLong();
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

    public class VersionMgr : MonoBehaviour
    {
        public string[] urls = new[] {"http://192.168.8.60:8080/test/master/"};
        public string url = "";

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
                first = strFirst[1].ToInt();
                last = strFirst[2].ToInt();
                url = info + (isMaster ? "/svn-master.txt" : "/svn-patch.txt");
                local = url.Replace("/", "-");
                hashText = (queue.Count == 0 ? "" : queue.Dequeue().First()).Trim();
            }

            public override string ToString()
            {
                return string.Format("Url: {0}, HashText: {1}, IsMaster: {2}", url, hashText, isMaster);
            }
        }
        PatchListInfo lastAccessInfo { get; set; }
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
            if (!Directory.Exists(ResInfo.LocalRoot))
                Directory.CreateDirectory(ResInfo.LocalRoot);

            localCache =
                Directory.GetFiles(ResInfo.LocalRoot, "*.*", SearchOption.AllDirectories)
                    .Select(p => p.Replace("\\", "/").Replace(ResInfo.LocalRoot, "").Trim())
                    .ToList();
        }

        private IEnumerator Start()
        {
            yield return StartCoroutine(GetPatchList("patch-list.txt"));
            foreach (var info in patchList)
            {
                if (lastAccessInfo != null && info.first != lastAccessInfo.last)
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
                lastAccessInfo = info;
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
            using (WWW www = new WWW(url + fileName))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    patchList = Encoding.UTF8.GetString(www.bytes)
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
            }
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
                ResInfo.SvnVersion = text.First();
                ResInfo.MinVersion = text.Skip(1).First().ToInt();
                ResInfo.MaxVersion = text.Skip(2).First().ToInt();
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
                var minVersion = text.Skip(1).First().ToInt();
                if (ResInfo.SvnVersion != svnVersion || ResInfo.MaxVersion != minVersion)
                {
                    patchCache = new Dictionary<string, ResInfo>();
                    return;
                }
                ResInfo.MaxVersion = text.Skip(2).First().ToInt();
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
                using (WWW www = new WWW(url + info.url))
                {
                    yield return www;
                    if (string.IsNullOrEmpty(www.error) && EncryptUtils.MD5.Encrypt(www.bytes) == info.hashText)
                    {
                        bytes = www.bytes;
                        File.WriteAllBytes(path, bytes);
                    }
                }
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
            var downList = masterCache.Values.Where(p => !localCache.Contains(p.formatPath)).ToList();
            Debug.Log(downList.Count + "\r\n" + string.Join("\n", downList.Select(p => p.ToString()).ToArray()));

            //yield break;
            var lastList = new Queue<ResInfo>(downList);
            while (lastList.Count != 0)
            {
                var resInfo = lastList.Dequeue();
                using (WWW www = new WWW(url + WWW.EscapeURL(resInfo.url)))
                {
                    Debug.Log(string.Format("down...{0}...{1}", (float) lastList.Count/downList.Count, www.url));
                    yield return www;

                    bool isOver = false;
                    if (string.IsNullOrEmpty(www.error) && EncryptUtils.MD5.Encrypt(www.bytes) == resInfo.hash)
                    {
                        string dir = Path.GetDirectoryName(ResInfo.LocalRoot + resInfo.formatPath);
                        if (dir != null && !Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        File.WriteAllBytes(ResInfo.LocalRoot + resInfo.formatPath, www.bytes);
                        resInfo.retryCount = 0;
                        isOver = true;
                    }
                    if (isOver) continue;
                    Debug.LogError(www.error);
                    if (resInfo.retryCount < ResInfo.RetryCount)
                    {
                        lastList.Enqueue(resInfo);
                        resInfo.retryCount++;
                    }
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("LastAccess:" + lastAccessInfo);
            GUILayout.Label("SvnVersion:" + ResInfo.SvnVersion);
            GUILayout.Label("MinVersion:" + ResInfo.MinVersion);
            GUILayout.Label("MaxVersion:" + ResInfo.MaxVersion);
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

                using (WWW www = new WWW(ResInfo.LocalRoot + resInfo.formatPath))
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