﻿//#define MD5

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;
using Library.Helper;
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

public class VersionMgr : MonoBehaviour
{
    public class Access
    {
        public static string dataPath { get; private set; }
        public static string persistentDataPath { get; private set; }

        public static string streamingAssetsPath
        {
            get
            {
#if UNITY_EDITOR
                return "file:///" + dataPath + "/StreamingAssets/";
#elif UNITY_STANDALONE_WIN
                return "file:///" + dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
                return "jar:file://" + dataPath + "!/assets/";
#elif UNITY_IPHONE || UNITY_IOS
                return "file://" + dataPath + "/Raw/";  
#endif
            }
        }

        public static string LocalTempRoot
        {
            get { return persistentDataPath + "Temp/"; }
        }

        public static string LocalPatchRoot
        {
            get { return persistentDataPath + "Patch/"; }
        }

        private static readonly Queue<string> urls = new Queue<string>();

        static Access()
        {
            dataPath = Application.dataPath;
            persistentDataPath = Application.persistentDataPath + "/";
            urls.Enqueue("http://192.168.8.59:8080/test/master/");
            urls.Enqueue("http://192.168.8.59:8080/test/master/");
            urls.Enqueue("http://192.168.8.59:8080/test/master/");
        }

        private int retry = 0;
        private static readonly int RetryCount = 5;

        public Access()
        {
            retry = 0;
        }

        /// <summary>
        /// 远程加载
        /// 失败后重试，最大重试5次，每次url取队列最新值
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="path"></param>
        /// <param name="callAction"></param>
        /// <returns></returns>
        public IEnumerator FromRemote(MonoBehaviour mono, string path, Action<byte[]> callAction)
        {
            using (WWW www = new WWW(urls.Peek() + path))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
#if UNITY_EDITOR
                Debug.Log(Encoding.UTF8.GetString(www.bytes));
#endif
                    callAction.Invoke(www.bytes);
                }
                else
                {
                    if (retry < RetryCount)
                    {
                        retry++;
                        urls.Enqueue(urls.Dequeue());
                        yield return mono.StartCoroutine(FromRemote(mono, path, callAction));
                    }
                    else
                    {
                        callAction.Invoke(null);
                    }
                }
                www.Dispose();
            }
        }

        /// <summary>
        /// 本地加载无须重试
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="path"></param>
        /// <param name="callAction"></param>
        /// <returns></returns>
        private IEnumerator FromLocal(MonoBehaviour mono, string path, Action<byte[]> callAction)
        {
            using (WWW www = new WWW(path))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
#if UNITY_EDITOR
                    Debug.Log(Encoding.UTF8.GetString(www.bytes));
#endif
                    callAction.Invoke(www.bytes);
                }
                else
                {
                    callAction.Invoke(null);
                }
                www.Dispose();
            }
        }

        public IEnumerator FromStreamingAssetsPath(MonoBehaviour mono, string path, Action<byte[]> callAction)
        {
            yield return mono.StartCoroutine(FromLocal(mono, streamingAssetsPath + path, callAction));
        }

        public IEnumerator FromPersistentDataPath(MonoBehaviour mono, string path, Action<byte[]> callAction)
        {
            yield return mono.StartCoroutine(FromLocal(mono, persistentDataPath + path, callAction));
        }
    }

    public class PatchInfo
    {
        public static string PatchListName = "patch-list.txt";
        public static string MasterName = "/svn-master.txt";
        public static string PatchName = "/svn-patch.txt";

        public string name;
        public string group; //主目录
        public bool isMaster; //主与补丁区分
        public int first; //开始版本号
        public int last; //结束版本好

        public bool isZip = false; //是否是压缩包
        public string zipHash; //压缩包hash
        public long zipSize; //压缩包大小

       
        public string fileUrl; //远程结构
        public string fileLocal; //本地结构全路径
        public string fileHash; //对应的记录文本hash
        public long fileSize; //对应的记录文本大小
        public bool isNeedDownFile
        {
            get { return !Library.Encrypt.MD5.ComparerFile(fileHash, fileLocal); }
        }

        public PatchInfo(string info)
        {
            string[] queue = info.Split(',');
            //svn-潘之琳-0-9-master
            name = queue.Last();
            isMaster = name.Contains("master");
            var strFirst = name.Split('-').ToArray();
            group = strFirst[1];
            first = strFirst[2].AsInt();
            last = strFirst[3].AsInt();

            fileUrl = Path.GetFileNameWithoutExtension(name) + (isMaster ? MasterName : PatchName);
            fileLocal = Access.LocalPatchRoot + fileUrl.Replace("/", "-");
            fileSize = queue.First().AsLong();
            fileHash = queue.Skip(1).First();

            if ((isZip = Path.HasExtension(name)) != true) return;
            zipSize = queue.Skip(2).First().AsLong();
            zipHash = queue.Skip(3).First();
        }

        public override string ToString()
        {
            return
                string.Format(
                    "Name: {0}, Group: {1}, IsMaster: {2}, First: {3}, Last: {4}, IsZip: {5}, ZipHash: {6}, ZipSize: {7}, IsNeedDownFile: {8}, FileUrl: {9}, FileLocal: {10}, FileHash: {11}, FileSize: {12}",
                    name, @group, isMaster, first, last, isZip, zipHash, zipSize, isNeedDownFile, fileUrl, fileLocal,
                    fileHash, fileSize);
        }
    }

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
        public string hash { get; private set; }
        public PatchInfo PatchInfo { get; private set; }

        /// <summary>
        /// 相对于旧得版本的文件操作行为
        /// </summary>
        public Action action = Action.N;
        /// <summary>
        /// 转化本地文件路径使用
        /// 和其余信息一起被格式化
        /// 最低包含文件名与版本信息
        /// </summary>
        public string formatPath { get; private set; }

        public string url { get; private set; }

        public ResInfo(PatchInfo patchListInfo, string info, Source source)
        {
            PatchInfo = patchListInfo;
            switch (source)
            {
                case Source.Master:
                {
                    var queue = new Queue<string>(info.Split(','));
                    version = queue.Count == 0 ? 0 : queue.Dequeue().AsInt();
                    action = Action.A;
                    size = queue.Count == 0 ? 0 : queue.Dequeue().AsLong();
                    hash = queue.Count == 0 ? "" : queue.Dequeue();
                    path = queue.Count == 0 ? "" : queue.Dequeue().Replace("\\", "/").Trim();
                }
                    break;
                case Source.Patch:
                {
                    var queue = new Queue<string>(info.Split(','));
                    version = queue.Count == 0 ? 0 : queue.Dequeue().AsInt();
                    var temp = queue.Count == 0 ? "" : queue.Dequeue();
                    if (Enum.IsDefined(typeof (Action), temp))
                        action = (Action) Enum.Parse(typeof (Action), temp);
                    else
                        action = Action.N;
                    size = queue.Count == 0 ? 0 : queue.Dequeue().AsLong();
                    hash = queue.Count == 0 ? "" : queue.Dequeue();
                    path = queue.Count == 0 ? "" : queue.Dequeue().Replace("\\", "/").Trim();
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("source", source, null);
            }

            if (patchListInfo.isZip)
            {
                action = Action.N;
            }

            name = Path.GetFileName(path);
            url = string.Format("{0}?v={1}", path, version);
            formatPath = string.Format("{0}@{1}", path, version);
#if MD5
            formatPath = Library.Encrypt.MD5.Encrypt(formatPath);
#endif
        }

        public override string ToString()
        {
            return string.Format("Path: {0}, Size: {1}, Action: {2}, version: {3}", path, size, action, version);
        }
    }

    public class VersionInfo : MonoBehaviour
    {
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

        [SerializeField]
        public string GroupName { get; private set; }
        public PatchInfo LastAccessInfo { get; set; }
        private List<PatchInfo> patchList { get; set; }
        public Dictionary<string, ResInfo> masterCache { get; set; }
        private Dictionary<string, ResInfo> patchCache { get; set; }

        public void Init(string group, List<PatchInfo> list)
        {
            GroupName = group;
            masterCache = new Dictionary<string, ResInfo>();
            patchCache = new Dictionary<string, ResInfo>();
            patchList = list;
        }

        public IEnumerator InitStart()
        {
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
                    foreach (var pair in patchCache)
                        masterCache[pair.Key] = pair.Value;
                }
                LastAccessInfo = info;
            }

            yield return StartCoroutine(ResourceDelete());
            yield return StartCoroutine(ResourceDownLoad());
        }

        /// <summary>
        /// 下载记录主资源列表的文本
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private IEnumerator GetMasterCache(PatchInfo info)
        {
            yield return StartCoroutine(GetCacheText(info, text =>
            {
                if (text == null) return;
                SvnVersion = text.First();
                MinVersion = text.Skip(1).First().AsInt();
                MaxVersion = text.Skip(1).First().AsInt();
                masterCache =
                    text.Skip(2).Select(p => new ResInfo(info,p, ResInfo.Source.Master))
                        .Where(p => !p.name.StartsWith("."))
                        .ToDictionary(p => p.path);
            }));
        }

        /// <summary>
        /// 下载每一个补丁文本获取补丁文件列表
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private IEnumerator GetPatchCache(PatchInfo info)
        {
            yield return StartCoroutine(GetCacheText(info, text =>
            {
                if (text == null) return;
                var svnVersion = text.First();
                var minVersion = text.Skip(1).First().AsInt();
                if (SvnVersion != svnVersion || MaxVersion != minVersion) return;
                MaxVersion = text.Skip(2).First().AsInt();
                patchCache =
                    text.Skip(3).Select(p => new ResInfo(info,p, ResInfo.Source.Patch))
                        .Where(p => !p.name.StartsWith("."))
                        .ToDictionary(p => p.path);
            }));
        }

        private IEnumerator GetCacheText(PatchInfo info, Action<string[]> callAction)
        {
            if (info.isNeedDownFile)
            {
                if (info.isZip)
                {
                    yield return StartCoroutine(GetRemoteZip(info));
                }
                else
                {
                    yield return StartCoroutine(new Access().FromRemote(this, info.fileUrl, res =>
                    {
                        if (res == null || Library.Encrypt.MD5.Encrypt(res) != info.fileHash) return;
                        File.WriteAllBytes(info.fileLocal, res);
                    }));
                }
            }
            byte[] bytes = File.ReadAllBytes(info.fileLocal);
            if (bytes.Length == 0)
            {
                callAction.Invoke(null);
                yield break;
            }
            callAction.Invoke(Encoding.UTF8.GetString(bytes)
                .Split('\r', '\n')
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => p.Trim())
                .ToArray());
        }

        /// <summary>
        /// 下载记录主资源压缩包
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private IEnumerator GetRemoteZip(PatchInfo info)
        {
            yield return StartCoroutine(new Access().FromRemote(this, info.name, res =>
            {
                if (res == null || Library.Encrypt.MD5.Encrypt(res) != info.zipHash) return;
                var zipName = Access.LocalPatchRoot + info.name;
                File.WriteAllBytes(zipName, res);
                string msg = Library.Compress.DecompressUtils.UnMakeZipFile(zipName, "", false);
                if (string.IsNullOrEmpty(msg))
                {
                    //File.Delete(zipName);
                    FileHelper.CreateDirectory(info.fileLocal);
                    File.Move(Access.LocalPatchRoot + info.fileUrl, info.fileLocal);
                    //FileMoveTo(zipName);
                }
                else
                {
                    Debug.LogError("解压文件失败！" + msg);
                }
            }));
        }

        /// <summary>
        /// 解压后的文件整合到预定义的目录结构
        /// </summary>
        /// <param name="zipName"></param>
        private static void FileMoveTo(string zipName)
        {
            var folderName = zipName.Replace(Path.GetExtension(zipName), "");
            var list =
                Directory.GetFiles(folderName, "*.*", SearchOption.AllDirectories)
                    .Select(p => p.Replace("\\", "/"))
                    .ToList();
            foreach (string s in list)
            {
                var file = s.Replace(folderName + "/", Access.LocalTempRoot);
                FileHelper.CreateDirectory(file);
                if (File.Exists(file))
                    File.Delete(file);
                File.Move(s, file);
            }
            Directory.Delete(folderName, true);
        }

        /// <summary>
        /// 删除多余资源,清理空间
        /// </summary>
        /// <returns></returns>
        private IEnumerator ResourceDelete()
        {
            var delList = masterCache.Values.Select(p => p.action == ResInfo.Action.D).ToList();
            Debug.Log(delList.Count + "\r\n" + string.Join("\n", delList.Select(p => p.ToString()).ToArray()));

            //yield break;
            foreach (var path in delList)
            {
                try
                {
                    if (File.Exists(Access.LocalTempRoot + path))
                        File.Delete(Access.LocalTempRoot + path);
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
            var downList = masterCache.Values.Where(p => p.action == ResInfo.Action.A || p.action == ResInfo.Action.M).ToList();
            Debug.Log(downList.Count + "\r\n" + string.Join("\n", downList.Select(p => p.ToString()).ToArray()));

            //yield break;
            var lastList = new Queue<ResInfo>(downList);
            while (lastList.Count != 0)
            {
                var resInfo = lastList.Dequeue();
                Debug.Log(string.Format("down...{0}...{1}", (float) lastList.Count/downList.Count, resInfo.url));
                yield return StartCoroutine(new Access().FromRemote(this, resInfo.url, res =>
                {
                    if (res == null || Library.Encrypt.MD5.Encrypt(res) != resInfo.hash)
                        return;
                    FileHelper.CreateDirectory(Access.LocalTempRoot + resInfo.formatPath);
                    File.WriteAllBytes(Access.LocalTempRoot + resInfo.formatPath, res);
                }));
            }
        }
    }

    public enum State
    {
        None,
        [Description("下载补丁文件列表")] DownPathList,

        [Description("下载某主资源包文件压缩包")] DownMasterZip,
        [Description("解压某主资源包文件压缩包")] UnMakeMasterZip,
        [Description("正在应用某主资源压缩包文件资源")] ApplyMasterZip,

        [Description("获取某主资源包文件的包含文件列表")] GetMasterList,

        [Description("下载某补丁文件压缩包")] DownPatchZip,
        [Description("解压某补丁文件压缩包")] UnMakePatchZip,
        [Description("正在应用某补丁压缩包文件资源")] ApplyPatchZip,

        [Description("获取某补丁文件的包含文件列表")] GetPatchList,

        [Description("获取某主资源包文件的包含文件列表")] DownResource,
        [Description("获取某主资源包文件的包含文件列表")] ApplyResource,
    }

    private Dictionary<string,VersionInfo> patchListCache { get; set; }

    private void Awake()
    {
        patchListCache = new Dictionary<string, VersionInfo>();
        FileHelper.CreateDirectory(Access.LocalTempRoot);
    }

    private IEnumerator Start()
    {
        yield return StartCoroutine(GetPatchList());
        foreach (var info in patchListCache)
        {
            yield return StartCoroutine(info.Value.InitStart());
        }
    }

    /// <summary>
    ///  下载记录补丁的文本获取补丁列表
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetPatchList()
    {
        yield return StartCoroutine(new Access().FromRemote(this, PatchInfo.PatchListName, res =>
        {
            if (res == null) return;
            FileHelper.CreateDirectory(Access.LocalPatchRoot + PatchInfo.PatchListName);
            File.WriteAllBytes(Access.LocalPatchRoot + PatchInfo.PatchListName, res);
            patchListCache = Encoding.UTF8.GetString(res)
                .Split('\r', '\n')
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => p.Trim())
                .Select(p => new PatchInfo(p))
                .ToLookup(p => p.group)
                .ToDictionary(p => p.Key, q =>
                {
                    var go = gameObject.AddComponent<VersionInfo>();
                    go.Init(q.Key, new List<PatchInfo>(q).OrderBy(t => t.first).ToList());
                    return go;
                });
        }));
    }

    private void OnGUI()
    {
        //GUILayout.Label("LastAccess:" + LastAccessInfo);
        //GUILayout.Label("SvnVersion:" + SvnVersion);
        //GUILayout.Label("MinVersion:" + MinVersion);
        //GUILayout.Label("MaxVersion:" + MaxVersion);
    }


    #region Load

    private IEnumerator LoadObject(string path, Action<WWW> callAction)
    {
        ResInfo resInfo = null;
        //if (masterCache.TryGetValue(path, out resInfo))
        //{
        //    if (resInfo.action == ResInfo.Action.D)
        //    {
        //        Debug.LogError("访问已被删除资源!");
        //        yield break;
        //    }

        //    using (WWW www = new WWW(Access.LocalRoot + resInfo.formatPath))
        //    {
        //        yield return www;
        //        callAction.Invoke(www);
        //        www.Dispose();
        //        yield break;
        //    }
        //}
        Debug.LogError("访问不被控制的资源!");
        yield break;
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