using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Debug = UnityEngine.Debug;

//覆盖更新
//只进行master与最新patch比较
//中间差异跳过
//web端为覆盖更新
//资源统一存放在固定的结构内
namespace Svn.Second
{
    public class VersionMgr : MonoBehaviour
    {
        public class ResInfo
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

            /// <summary>
            /// 想比旧得版本的文件操作行为
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

        public string url = "http://192.168.8.60:8080/test/master/";

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
            yield return StartCoroutine(GetTextCache(fileName, text =>
            {
                if (text == null)
                {
                    masterCache = new Dictionary<string, ResInfo>();
                    return;
                }
                SvnVersion = text.First();
                MinVersion = text.Skip(1).First().ToInt();
                masterCache =
                    text.Skip(2).Select(p => new ResInfo(p, ResInfo.Source.Master))
                        .Where(p => !p.name.StartsWith("."))
                        .ToDictionary(p => p.path);
            }));
        }

        private IEnumerator GetPatchCache(string fileName)
        {
            yield return StartCoroutine(GetTextCache(fileName, text =>
            {
                if (text == null)
                {
                    patchCache = new Dictionary<string, ResInfo>();
                    return;
                }
                var svnVersion = text.First();
                var minVersion = text.Skip(1).First().ToInt();
                if (SvnVersion != svnVersion || MaxVersion != minVersion)
                {
                    patchCache = new Dictionary<string, ResInfo>();
                }
                else
                {
                    MaxVersion = text.Skip(2).First().ToInt();
                    patchCache =
                        text.Skip(3).Select(p => new ResInfo(p, ResInfo.Source.Patch))
                            .Where(p => !p.name.StartsWith("."))
                            .ToDictionary(p => p.path);
                }
            }));
        }

        private IEnumerator GetTextCache(string fileName, Action<string[]> callAction)
        {
            using (WWW www = new WWW(url + fileName))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    callAction.Invoke(
                        Encoding.UTF8.GetString(www.bytes)
                            .Split('\r', '\n')
                            .Where(p => !string.IsNullOrEmpty(p))
                            .Select(p => p.Trim())
                            .ToArray());
                }
                else
                {
                    callAction.Invoke(null);
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
                using (WWW www = new WWW(url + WWW.EscapeURL(resInfo.url)))
                {
                    Debug.Log(string.Format("down...{0}...{1}", (float) lastList.Count/downList.Count, www.url));
                    yield return www;
                    if (string.IsNullOrEmpty(www.error))
                    {
                        string dir = Path.GetDirectoryName(LocalRoot + resInfo.formatPath);
                        if (dir != null && !Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        File.WriteAllBytes(LocalRoot + resInfo.formatPath, www.bytes);
                        resInfo.retryCount = 0;
                    }
                    else
                    {
                        Debug.LogError(www.error);
                        if (resInfo.retryCount < 5)
                        {
                            lastList.Enqueue(resInfo);
                            resInfo.retryCount++;
                        }
                    }
                }
            }
        }
        private void OnGUI()
        {
            GUILayout.Label("SvnVersion:" + SvnVersion);
            GUILayout.Label("MinVersion:" + MinVersion);
            GUILayout.Label("MaxVersion:" + MaxVersion);
        }

        #region Load

        public IEnumerator LoadObject(string path, Action<WWW> callAction)
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
            yield return StartCoroutine(LoadObject(path, www => { callAction.Invoke(www.texture); }));
        }

        public IEnumerator Load(string path, Action<AudioClip> callAction)
        {
            yield return StartCoroutine(LoadObject(path, www => { callAction.Invoke(www.audioClip); }));
        }

        public IEnumerator Load(string path, Action<AssetBundle> callAction)
        {
            yield return StartCoroutine(LoadObject(path, www => { callAction.Invoke(www.assetBundle); }));
        }

        public IEnumerator Load(string path, Action<byte[]> callAction)
        {
            yield return StartCoroutine(LoadObject(path, www => { callAction.Invoke(www.bytes); }));
        }

        public IEnumerator Load(string path, Action<string> callAction)
        {
            yield return StartCoroutine(LoadObject(path, www => { callAction.Invoke(www.text); }));
        }

        public IEnumerator Load(string path, Action<MovieTexture> callAction)
        {
            yield return StartCoroutine(LoadObject(path, www => { callAction.Invoke(www.movie); }));
        }

        #endregion
    }
}