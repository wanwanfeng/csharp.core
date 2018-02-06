﻿#define MD5

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using FileVersion;
using Library.Extensions;
using Library.Helper;
using Debug = UnityEngine.Debug;

//增量更新
//每更新一次，资源放在新的文件夹目录内，老资源不会被覆盖
//目录结构如下
//
//
//注:文本编码均需为UTF8
//
//│  
//├─master-00-02
//├─master-00-02.txt
//│      
//├─patch-02-09
//├─patch-02-09.txt
//│      
//├─patch-09-20
//├─patch-09-20.txt
//│      
//├─patch-20-23
//├─patch-20-23.txt
//│      
//├─patch-23-33
//├─patch-23-33.txt
//│      


public class VersionMgr : MonoBehaviour
{
    public static string SoftwareVersion { get; set; }
    protected static string KeyMd5 { get; private set; }

    static VersionMgr()
    {
        KeyMd5 = "";
        Library.Encrypt.AES.Key = Library.Encrypt.MD5.Encrypt("YmbEV0FVzZN/SvKCCoJje/jSpM");
        Library.Encrypt.AES.Head = "JKRihFwgicIzkBPEyyEn9pnpoANbyFuplHl";
    }

    public class Access
    {
        public static string DataPath { get; private set; }
        public static string PersistentDataPath { get; private set; }

        public static string StreamingAssetsPath
        {
            get
            {
#if UNITY_EDITOR
                return "file:///" + DataPath + "/StreamingAssets/";
#elif UNITY_STANDALONE_WIN
                return "file:///" + dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
                return "jar:file://" + dataPath + "!/assets/";
#elif UNITY_IPHONE || UNITY_IOS
                return "file://" + dataPath + "/Raw/";  
#endif
            }
        }

        public static string PersistentDataTempPath
        {
            get { return PersistentDataPath + "Temp/"; }
        }

        public static string PersistentDataPatchPath
        {
            get { return PersistentDataPath + "Patch/"; }
        }

        public static string StreamingAssetsTempPath
        {
            get { return StreamingAssetsPath + "Temp/"; }
        }

        private static readonly Queue<string> urls = new Queue<string>();

        static Access()
        {
            DataPath = Application.dataPath;
            PersistentDataPath = Application.persistentDataPath + "/";
            urls.Enqueue("http://127.0.0.1:8080/test/master/");
            urls.Enqueue("http://127.0.0.1:8080/test/master/");
            urls.Enqueue("http://127.0.0.1:8080/test/master/");
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
#if UNITY_EDITOR
                Debug.Log(www.url);
#endif
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
#if UNITY_EDITOR
                    Debug.Log(Encoding.UTF8.GetString(www.bytes) ?? "");
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
        private static IEnumerator FromLocal(MonoBehaviour mono, string path, Action<byte[]> callAction)
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

        public static IEnumerator FromStreamingAssetsPath(MonoBehaviour mono, string path, Action<byte[]> callAction)
        {
            path = StreamingAssetsPath + path;
            yield return mono.StartCoroutine(FromLocal(mono, path, callAction));
        }

        public static byte[] FromPersistentDataPath(string path)
        {
            path = PersistentDataPath + path;
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }

        public static byte[] FromPersistentDataTempPath(string path)
        {
            path = PersistentDataTempPath + path;
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }

        public static Texture2D FileToTexture2D(string path)
        {
            byte[] bytes = FromPersistentDataTempPath(path);
            if (bytes == null) return null;
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(bytes);
            return texture;
        }
    }

    public class PatchInfo
    {
        public static string PatchListName = "patch-list.txt";

        public string group; //资源包所属
        public string name; //资源包名称
        public int first; //开始版本号
        public int last; //结束版本号 

        public string fileUrl; //远程文本
        public string fileLocal; //本地对应远程全路径文本
        public string fileHash; //对应的记录文本hash
        public long fileSize; //对应的记录文本大小

        /// <summary>
        /// 记录文本是否需要下载
        /// </summary>
        public bool isNeedDownFile
        {
            get { return !Library.Encrypt.MD5.ComparerFile(fileHash, fileLocal); }
        }

        public string zipHash; //压缩包hash
        public long zipSize; //压缩包大小

        //是否是压缩包
        public bool isZip
        {
            get { return !string.IsNullOrEmpty(zipHash); }
        }

        public string zipName
        {
            get { return name + ".zip"; }
        }

        public PatchInfo(FilePatchInfo info)
        {
            name = info.path;
            group = info.group;
            first = info.firstVersion;
            last = info.lastVersion;
            fileSize = info.content_size.AsLong();
            fileHash = info.content_hash;
            zipSize = info.zip_size.AsLong();
            zipHash = info.zip_hash;

            fileUrl = Path.GetFileNameWithoutExtension(name) + ".txt";
            fileLocal = Access.PersistentDataPatchPath + fileUrl.Replace("/", "-");
        }

        public override string ToString()
        {
            return string.Format("Group: {0}, Name: {1}, FileHash: {2}, ZipHash: {3}, ZipSize: {4}, isNeedDownFile: {5}", @group, name, fileHash, zipHash, zipSize, isNeedDownFile);
        }
    }

    public class ResInfo
    {
        public enum DataType
        {
            ResourcesDataPath,
            StreamingAssetsPath,
            PersistentDataPath,
        }

        public DataType dataType = DataType.PersistentDataPath;

        public enum Action
        {
            N,
            A, //add
            M, //modified
            D, //delete
        }

        /// <summary>
        /// 相对于旧得版本的文件操作行为
        /// </summary>
        public Action action = Action.N;

        public string path { get; private set; }
        public string name { get; private set; }
        public int version { get; private set; }
        public long size { get; private set; }
        public string hash { get; private set; }
        public string url { get; private set; }
        public PatchInfo patchInfo { get; private set; }

        public string localhash
        {
            get
            {
                if (!File.Exists(Access.PersistentDataTempPath + path))
                    return "";
                var temp = Library.Encrypt.MD5.Encrypt(File.ReadAllBytes(Access.PersistentDataTempPath + path));
                return temp;
            }
        }

        public bool isNeedDownFile
        {
            get
            {
                if (action == Action.D)
                    return false;
                if (action == Action.A)
                    return true;
                if (action == Action.M)
                    return true;
                return hash != localhash;
            }
        }

        public ResInfo(string path, DataType dataType)
        {
            this.path = path;
            this.dataType = dataType;
            action = Action.N;
        }

        public ResInfo(PatchInfo info, FileDetailInfo fileInfo)
        {
            this.dataType = DataType.PersistentDataPath;
            patchInfo = info;
            if (Enum.IsDefined(typeof(Action), fileInfo.action))
                action = (Action)Enum.Parse(typeof(Action), fileInfo.action);
            else
                action = Action.N;
            size = fileInfo.content_size.AsLong();
            hash = fileInfo.content_hash;
            path = fileInfo.path_md5;
            version = fileInfo.version.AsInt();
            if (patchInfo.isZip && action != Action.D)
                action = Action.N;
            if (hash == localhash)
                action = Action.N;

            name = Path.GetFileName(path);
            url = string.Format("{0}?v={1}", path, version);
        }

        public override string ToString()
        {
            return string.Format("DataType: {0}, Action: {1}, path: {2}, version: {3}, hash: {4}, isNeedDownFile: {5}", dataType, action, path, version, hash, isNeedDownFile);
        }
    }

    public class VersionInfo : MonoBehaviour
    {
        public int MinVersion { get; set; }
        public int MaxVersion { get; set; }

        public event Action<State> OnActionState;

        private void ActionState(State state)
        {
            if (OnActionState != null)
                OnActionState.Invoke(state);
        }

        [SerializeField]
        public string GroupName { get; private set; }

        public PatchInfo LastAccessInfo { get; set; }
        private List<PatchInfo> patchList { get; set; }
        public Dictionary<string, ResInfo> masterCache { get; set; }
        private Dictionary<string, Dictionary<string, ResInfo>> patchCache { get; set; }

        public void Init(string group, List<PatchInfo> list)
        {
            GroupName = group;
            masterCache = new Dictionary<string, ResInfo>();
            patchCache = new Dictionary<string, Dictionary<string, ResInfo>>();
            patchList = list;
        }

        public IEnumerator InitStart(List<string> resourcesCacheList, List<string> streamingAssetsCachelist)
        {
            //Resources载入缓存
            foreach (var path in resourcesCacheList)
            {
                masterCache[path] = new ResInfo(path, ResInfo.DataType.ResourcesDataPath);
            }
            //StreamingAssets载入缓存
            foreach (var path in streamingAssetsCachelist)
            {
                masterCache[path] = new ResInfo(path, ResInfo.DataType.StreamingAssetsPath);
            }
            //PersistentDataPath载入缓存
            foreach (var info in patchList)
            {
                if (LastAccessInfo != null && info.first != LastAccessInfo.last)
                    yield break;
                yield return StartCoroutine(GetPatchCache(info));
                LastAccessInfo = info;
            }
            foreach (var pair in patchCache)
            {
                foreach (var patch in pair.Value)
                {
                    masterCache[patch.Key] = patch.Value;
                }
            }
            yield return StartCoroutine(ResourceDelete());
            yield return StartCoroutine(ResourceDownLoad());
        }

        /// <summary>
        /// 下载记录主资源或补丁列表的文件列表
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private IEnumerator GetPatchCache(PatchInfo info)
        {
            if (info.isNeedDownFile)
            {
                ActionState(State.DownPathList);
                yield return StartCoroutine(new Access().FromRemote(this, info.fileUrl, res =>
                {
                    if (res == null || Library.Encrypt.MD5.Encrypt(res) != info.fileHash) return;
                    File.WriteAllBytes(info.fileLocal, res);
                }));
            }

            ActionState(State.GetPatchList);
            byte[] bytes = File.ReadAllBytes(info.fileLocal);
            if (bytes.Length == 0) yield break;

            ActionState(State.ApplyPatchList);
            var content = Library.Encrypt.AES.Decrypt(Encoding.UTF8.GetString(bytes));
            patchCache[info.fileUrl] =
                LitJson.JsonMapper.ToObject<FileDetailInfo[]>(content)
                    .Select(p => new ResInfo(info, p))
                    .ToDictionary(p => p.path);
            MinVersion = Math.Min(MinVersion, info.first);
            MaxVersion = Math.Max(MaxVersion, info.last);
        }

        /// <summary>
        /// 删除多余资源,清理空间
        /// </summary>
        /// <returns></returns>
        private IEnumerator ResourceDelete()
        {
            var delList =
                masterCache.Values.Where(
                    p => p.dataType == ResInfo.DataType.PersistentDataPath && p.action == ResInfo.Action.D).ToList();
            Debug.Log(delList.Count + "\r\n" + string.Join("\n", delList.Select(p => p.ToString()).ToArray()));

            //yield break;
            bool isDelete = false;
            foreach (var path in delList)
            {
                try
                {
                    if ((isDelete = File.Exists(Access.PersistentDataTempPath + path)) == true)
                        File.Delete(Access.PersistentDataTempPath + path);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
                finally
                {
                    //Debug.Log(isDelete);
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
            var downList =
                masterCache.Values.Where(
                    p => p.dataType == ResInfo.DataType.PersistentDataPath && p.isNeedDownFile == true).ToList();
            Debug.Log(downList.Count + "\r\n" + string.Join("\n", downList.Select(p => p.ToString()).ToArray()));

            var dic = downList.ToLookup(p => p.patchInfo).ToDictionary(p => p.Key, q => new List<ResInfo>(q));

            foreach (KeyValuePair<PatchInfo, List<ResInfo>> keyValuePair in dic)
            {
                if (keyValuePair.Key.isZip)
                {
                    ActionState(State.DownResource);
                    yield return StartCoroutine(GetRemoteZip(keyValuePair.Key));
                }
                else
                {
                    //yield break;
                    var lastList = new Queue<ResInfo>(keyValuePair.Value);
                    while (lastList.Count != 0)
                    {
                        var resInfo = lastList.Dequeue();
                        ActionState(State.DownResource);
                        Debug.Log(string.Format("down...{0}...{1}", (float) lastList.Count/downList.Count, resInfo.url));
                        yield return
                            StartCoroutine(new Access().FromRemote(this, keyValuePair.Key.name + "/" + resInfo.url,
                                res =>
                                {
                                    if (res == null || Library.Encrypt.MD5.Encrypt(res) != resInfo.hash)
                                        return;
                                    ActionState(State.ApplyResource);
                                    FileHelper.CreateDirectory(Access.PersistentDataTempPath + resInfo.path);
                                    File.WriteAllBytes(Access.PersistentDataTempPath + resInfo.path, res);
                                    resInfo.action = ResInfo.Action.N;
                                }));
                    }
                }
            }
        }

        /// <summary>
        /// 下载记录主资源压缩包
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private IEnumerator GetRemoteZip(PatchInfo info)
        {
            ActionState(State.DownPatchZip);
            yield return StartCoroutine(new Access().FromRemote(this, info.zipName, res =>
            {
                if (res == null || Library.Encrypt.MD5.Encrypt(res) != info.zipHash) return;
                var zipName = Access.PersistentDataPatchPath + info.zipName;
                File.WriteAllBytes(zipName, res);
                ActionState(State.UnMakePatchZip);
                string msg = Library.Compress.DecompressUtils.UnMakeZipFile(zipName, "", true);
                if (string.IsNullOrEmpty(msg))
                {
                    File.Delete(zipName);
                    FileMoveTo(info, zipName);
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
        /// <param name="info"></param>
        /// <param name="zipName"></param>
        private void FileMoveTo(PatchInfo info, string zipName)
        {
            ActionState(State.ApplyPatchZip);
            Dictionary<string, ResInfo> dic;
            if (!patchCache.TryGetValue(info.fileUrl, out dic))
            {
                Debug.LogError("不存在的补丁包！");
                return;
            }

            var folderName = zipName.Replace(Path.GetExtension(zipName), "");
            var list =
                Directory.GetFiles(folderName, "*.*", SearchOption.AllDirectories)
                    .Select(p => p.Replace("\\", "/"))
                    .OrderByDescending(p => p.Length)
                    .ToList();

            foreach (string s in list)
            {
                var file = s.Replace(folderName + "/", Access.PersistentDataTempPath);
                FileHelper.CreateDirectory(file);
                if (File.Exists(file))
                    File.Delete(file);
                File.Move(s, file);
            }
            Directory.Delete(folderName, true);
        }
    }

    public enum State
    {
        None,
        [Description("下载补丁文件列表")] DownPathList,

        [Description("下载某主资源或补丁压缩包")] DownPatchZip,
        [Description("解压某资源或补丁文件压缩包")] UnMakePatchZip,
        [Description("正在应用某主资源或补丁压缩包文件资源")] ApplyPatchZip,

        [Description("获取某主资源或补丁文件的包含文件列表")] GetPatchList,
        [Description("正在应用某主资源或补丁文件的包含文件列表")] ApplyPatchList,

        [Description("获取某主资源包文件的包含文件列表")] DownResource,
        [Description("获取某主资源包文件的包含文件列表")] ApplyResource,
    }

    private Dictionary<string, List<string>> resourcesCache { get; set; }
    private Dictionary<string, List<string>> streamingAssetsCache { get; set; }
    private Dictionary<string, VersionInfo> patchListCache { get; set; }

    /// <summary>
    ///  下载记录补丁的文本获取补丁列表
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetResourcesAndStreamingAssetsData()
    {
        resourcesCache = GetCache(Resources.Load<TextAsset>("resources").text);
        yield return StartCoroutine(Access.FromStreamingAssetsPath(this, "streamingAssets.txt", res =>
        {
            streamingAssetsCache = GetCache(Encoding.UTF8.GetString(res));
        }));
    }

    private Dictionary<string, List<string>> GetCache(string text)
    {
        return
            text.Split('\r', '\n')
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => p.Split(','))
                .ToLookup(p => p.First(), q => q.Last())
                .ToDictionary(p => p.Key, q => new List<string>(q));
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
            FileHelper.CreateDirectory(Access.PersistentDataPatchPath + PatchInfo.PatchListName);
            File.WriteAllBytes(Access.PersistentDataPatchPath + PatchInfo.PatchListName, res);

            var content = Library.Encrypt.AES.Decrypt(Encoding.UTF8.GetString(res));
            var versionInfo = LitJson.JsonMapper.ToObject<FileVersion.VersionInfo>(content);
            SoftwareVersion = versionInfo.softwareVersion;
            patchListCache = versionInfo.pathInfos.Select(p => new PatchInfo(p)).ToLookup(p => p.group)
                .ToDictionary(p => p.Key, q =>
                {
                    var go = gameObject.AddComponent<VersionInfo>();
                    go.Init(q.Key, new List<PatchInfo>(q).OrderBy(t => t.first).ToList());
                    return go;
                });
        }));
    }

    private void Awake()
    {
        patchListCache = new Dictionary<string, VersionInfo>();
        FileHelper.CreateDirectory(Access.PersistentDataTempPath);
    }

    private IEnumerator Start()
    {
        yield return StartCoroutine(GetResourcesAndStreamingAssetsData());
        yield return StartCoroutine(GetPatchList());
        foreach (var info in patchListCache.Values)
        {
            var resourcesCacheList = resourcesCache.ContainsKey(info.GroupName)
                ? resourcesCache[info.GroupName]
                : new List<string>();
            var streamingAssetsCachelist = streamingAssetsCache.ContainsKey(info.GroupName)
                ? streamingAssetsCache[info.GroupName]
                : new List<string>();
            yield return StartCoroutine(info.InitStart(resourcesCacheList, streamingAssetsCachelist));
        }


        StartCoroutine(Load("557028_1.jpg", tex =>
        {
            texture2D = tex;
        }));
    }

    private Texture2D texture2D = null;

    private void OnGUI()
    {
        if (texture2D == null)
            texture2D = Access.FileToTexture2D("557028_1.jpg");
        if (texture2D == null)
            texture2D = Access.FileToTexture2D(Library.Encrypt.MD5.Encrypt("10253312_640x640_0.jpg"));
        if (texture2D != null)
            GUI.DrawTexture(new Rect(0, 0, texture2D.width, texture2D.height), texture2D);
        GUILayout.Label("SvnVersion:" + SoftwareVersion);
        if (patchListCache == null)
            return;
        foreach (var versionInfo in patchListCache)
        {
            GUILayout.Label(string.Format("{0}:{1}-{2}", versionInfo.Value.GroupName, versionInfo.Value.MinVersion,
                versionInfo.Value.MaxVersion));
        }
    }

    #region Load

    private IEnumerator LoadObject(string path, Action<WWW, UnityEngine.Object> callAction)
    {
        string hash = Library.Encrypt.MD5.Encrypt(path);
        foreach (var cache in patchListCache.Values)
        {
            ResInfo resInfo = null;
            if (!cache.masterCache.TryGetValue(hash, out resInfo)) continue;
            if (resInfo.action == ResInfo.Action.D)
            {
                Debug.LogError("访问已被删除资源!");
                yield break;
            }
            switch (resInfo.dataType)
            {
                case ResInfo.DataType.ResourcesDataPath:
                    callAction.Invoke(null, Resources.Load(path));
                    yield break;
                case ResInfo.DataType.StreamingAssetsPath:
                    using (WWW www = new WWW(Access.StreamingAssetsPath + path))
                    {
                        yield return www;
                        callAction.Invoke(www, null);
                        www.Dispose();
                        yield break;
                    }
                default:
                    using (WWW www = new WWW(Access.PersistentDataTempPath + hash))
                    {
                        yield return www;
                        callAction.Invoke(www, null);
                        www.Dispose();
                        yield break;
                    }
            }
        }

        Debug.LogError("无效资源!");
        callAction.Invoke(null, null);
    }

    public IEnumerator Load(string path, Action<Texture2D> callAction)
    {
        yield return
            StartCoroutine(LoadObject(path,
                (www, obj) => { callAction.Invoke(www == null ? obj as Texture2D : www.texture); }));
    }

    public IEnumerator Load(string path, Action<AudioClip> callAction)
    {
        yield return
            StartCoroutine(LoadObject(path,
                (www, obj) => { callAction.Invoke(www == null ? obj as AudioClip : www.audioClip); }));
    }

    public IEnumerator Load(string path, Action<byte[]> callAction)
    {
        yield return
            StartCoroutine(LoadObject(path,
                (www, obj) => { callAction.Invoke(www == null ? (obj as TextAsset).bytes : www.bytes); }));
    }

    public IEnumerator Load(string path, Action<string> callAction)
    {
        yield return
            StartCoroutine(LoadObject(path,
                (www, obj) => { callAction.Invoke(www == null ? (obj as TextAsset).text : www.text); }));
    }

    public IEnumerator Load(string path, Action<MovieTexture> callAction)
    {
        yield return
            StartCoroutine(LoadObject(path,
                (www, obj) => { callAction.Invoke(www == null ? obj as MovieTexture : www.movie); }));
    }

    public IEnumerator Load(string path, Action<AssetBundle> callAction)
    {
        yield return StartCoroutine(LoadObject(path, (www, obj) => { callAction.Invoke(www.assetBundle); }));
    }

    #endregion
}