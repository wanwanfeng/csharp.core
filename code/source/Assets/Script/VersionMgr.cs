#define MD5

using Library;
using Library.Extensions;
using Library.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
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

public class VersionMgr : SingletonBehaviour<VersionMgr>
{
    public static string SoftwareVersion { get; set; }
    protected static string KeyMd5 { get; private set; }
    protected static string KeyAes { get; private set; }
    protected static string HeadAes { get; private set; }

    static VersionMgr()
    {
        KeyMd5 = "";
        KeyAes = "YmbEV0FVzZN/SvKCCoJje/jSpM".MD5();
        HeadAes = "JKRihFwgicIzkBPEyyEn9pnpoANbyFuplHl".MD5();
    }

    public class FilePatchInfo : FileVersion.FilePatchInfo
    {
        public static string PatchListName = "patch-list.txt";

        public string fileUrl; //远程文本
        public string fileLocal; //本地对应远程全路径文本

        /// <summary>
        /// 记录文本是否需要下载
        /// </summary>
        public bool isNeedDownFile
        {
            get
            {
                if (File.Exists(fileLocal))
                    return !content_hash.ComparerMD5(File.ReadAllText(fileLocal), KeyMd5);
                return true;
            }
        }

        public FilePatchInfo(FileVersion.FilePatchInfo info)
        {
            path = info.path;
            group = info.group;
            firstVersion = info.firstVersion;
            lastVersion = info.lastVersion;
            content_size = info.content_size;
            content_hash = info.content_hash;
            zip_size = info.zip_size;
            zip_hash = info.zip_hash;

            fileUrl = Path.GetFileNameWithoutExtension(path) + ".txt";
            fileLocal = Access.PersistentDataPatchPath + fileUrl.Replace("/", "-");
        }

        public override string ToString()
        {
            return string.Format(
                "Group: {0}, Name: {1}, FileHash: {2}, ZipHash: {3}, ZipSize: {4}, isNeedDownFile: {5}", @group, path,
                content_hash, zip_hash, zip_size, isNeedDownFile);
        }
    }

    public class FileDetailInfo : FileVersion.FileDetailInfo
    {
        public enum DataType
        {
            ResourcesDataPath,
            StreamingAssetsPath,
            PersistentDataPath,
        }

        public DataType dataType = DataType.PersistentDataPath;
        public string name { get; private set; }
        public string url { get; private set; }
        public bool is_ready { get; set; }
        public FilePatchInfo patchInfo { get; private set; }

        public string localhash
        {
            get
            {
                if (!File.Exists(Access.PersistentDataTempPath + path))
                    return "";
                var temp = File.ReadAllBytes(Access.PersistentDataTempPath + path).MD5();
                return temp;
            }
        }

        public bool isNeedDownFile
        {
            get
            {
                if (is_delete) return false;
                if (is_ready) return false;
                if (IsEncrypt())
                    return encrypt_hash != localhash;
                return content_hash != localhash;
            }
        }

        public FileDetailInfo(string path, DataType dataType)
        {
            this.path = path;
            this.dataType = dataType;
        }

        public FileDetailInfo(FilePatchInfo info, FileVersion.FileDetailInfo fileInfo)
        {
            this.dataType = DataType.PersistentDataPath;
            patchInfo = info;

            path = fileInfo.path;
            version = fileInfo.version;
            is_delete = fileInfo.is_delete;
            content_size = fileInfo.content_size;
            content_hash = fileInfo.content_hash;

            if (patchInfo.IsEncrypt())
                is_ready = false;
            if (content_hash == localhash)
                is_ready = true;

            name = Path.GetFileName(path);
            url = string.Format("{0}?v={1}", path, version);
        }

        public override string ToString()
        {
            return string.Format("DataType: {0}, is_delete: {1}, path: {2}, version: {3}, content_hash: {4}, isNeedDownFile: {5}",
                dataType, is_delete, path, version, content_hash, isNeedDownFile);
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

        public FilePatchInfo LastAccessInfo { get; set; }
        private List<FilePatchInfo> filePatchList { get; set; }
        public Dictionary<string, FileDetailInfo> mainCache { get; set; }
        private Dictionary<string, Dictionary<string, FileDetailInfo>> patchCache { get; set; }

        public void Init(string group, List<FilePatchInfo> list)
        {
            GroupName = group;
            mainCache = new Dictionary<string, FileDetailInfo>();
            patchCache = new Dictionary<string, Dictionary<string, FileDetailInfo>>();
            filePatchList = list;
        }

        public IEnumerator InitStart(List<string> resourcesCacheList, List<string> streamingAssetsCachelist)
        {
            //Resources载入缓存
            foreach (var path in resourcesCacheList)
            {
                mainCache[path] = new FileDetailInfo(path, FileDetailInfo.DataType.ResourcesDataPath);
            }
            //StreamingAssets载入缓存
            foreach (var path in streamingAssetsCachelist)
            {
                mainCache[path] = new FileDetailInfo(path, FileDetailInfo.DataType.StreamingAssetsPath);
            }
            //PersistentDataPath载入缓存
            foreach (var info in filePatchList)
            {
                if (LastAccessInfo != null && info.firstVersion != LastAccessInfo.lastVersion)
                    yield break;
                yield return GetFilePatchCache(info);
                LastAccessInfo = info;
            }
            foreach (var pair in patchCache)
            {
                foreach (var patch in pair.Value)
                {
                    mainCache[patch.Key] = patch.Value;
                }
            }
            yield return ResourceDelete();
            yield return ResourceDownLoad();
        }

        /// <summary>
        /// 下载记录主资源或补丁列表的文件列表
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private IEnumerator GetFilePatchCache(FilePatchInfo info)
        {
            if (info.isNeedDownFile)
            {
                ActionState(State.DownPathList);
                yield return Access.FromRemote(info.fileUrl, res =>
                {
                    if (res == null)
                        return;
                    var hash = res.MD5();
                    if (info.IsEncrypt())
                    {
                        if (hash != info.encrypt_hash) return;
                    }
                    else
                    {
                        if (hash != info.content_hash) return;
                    }
                    File.WriteAllBytes(info.fileLocal, res);
                });
            }

            ActionState(State.GetPatchList);
            MinVersion = Math.Min(MinVersion, info.firstVersion);
            MaxVersion = Math.Max(MaxVersion, info.lastVersion);

            if (File.Exists(info.fileLocal))
            {
                byte[] bytes = File.ReadAllBytes(info.fileLocal);
                if (bytes.Length == 0) yield break;

                ActionState(State.ApplyPatchList);
                string content = info.IsEncrypt() ? bytes.AES_Dencrypt() : Encoding.UTF8.GetString(bytes);
                patchCache[info.fileUrl] = Library.Helper.JsonHelper.ToObject<FileVersion.FileDetailInfo[]>(
                    content.Trim())
                    .Select(p => new FileDetailInfo(info, p))
                    .ToDictionary(p => p.path);
            }
            else
            {
                Debug.LogError("file is not exist! " + info.fileLocal);
            }
        }

        /// <summary>
        /// 删除多余资源,清理空间
        /// </summary>
        /// <returns></returns>
        private IEnumerator ResourceDelete()
        {
            var delList = mainCache.Values.Where(p => p.dataType == FileDetailInfo.DataType.PersistentDataPath && p.is_delete).ToList();
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
            var downList = mainCache.Values.Where(p => p.dataType == FileDetailInfo.DataType.PersistentDataPath && p.isNeedDownFile).ToList();
            Debug.Log(downList.Count + "\r\n" + string.Join("\n", downList.Select(p => p.ToString()).ToArray()));

            var dic = downList.ToLookup(p => p.patchInfo).ToDictionary(p => p.Key, q => new List<FileDetailInfo>(q));

            foreach (KeyValuePair<FilePatchInfo, List<FileDetailInfo>> keyValuePair in dic)
            {
                if (keyValuePair.Key.IsZip())
                {
                    ActionState(State.DownResource);
                    yield return GetRemoteZip(keyValuePair.Key);
                }
                else
                {
                    //yield break;
                    var lastList = new Queue<FileDetailInfo>(keyValuePair.Value);
                    while (lastList.Count != 0)
                    {
                        var resInfo = lastList.Dequeue();
                        ActionState(State.DownResource);
                        Debug.Log(string.Format("down...{0}...{1}", (float) lastList.Count/downList.Count, resInfo.url));
                        yield return Access.FromRemote(keyValuePair.Key.path + "/" + resInfo.url,
                            res =>
                            {
                                if (res == null)
                                    return;

                                var hash = res.MD5();

                                if (resInfo.IsEncrypt())
                                {
                                    if (hash != resInfo.encrypt_hash) return;
                                }
                                else
                                {
                                    if (hash != resInfo.content_hash) return;
                                }

                                ActionState(State.ApplyResource);
                                DirectoryHelper.CreateDirectory(Access.PersistentDataTempPath + resInfo.path);
                                File.WriteAllBytes(Access.PersistentDataTempPath + resInfo.path, res);
                                resInfo.is_ready = true;
                            });
                    }
                }
            }
        }

        /// <summary>
        /// 下载记录主资源压缩包
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public IEnumerator GetRemoteZip(FilePatchInfo info)
        {
            ActionState(State.DownPatchZip);
            yield return Access.FromRemote(info.path + ".zip", res =>
            {
                if (res == null || res.MD5() != info.zip_hash) return;
                var zipName = Access.PersistentDataPatchPath + info.path + ".zip";
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
            });
        }

        /// <summary>
        /// 解压后的文件整合到预定义的目录结构
        /// </summary>
        /// <param name="info"></param>
        /// <param name="zipName"></param>
        private void FileMoveTo(FilePatchInfo info, string zipName)
        {
            ActionState(State.ApplyPatchZip);
            Dictionary<string, FileDetailInfo> dic;
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

    private Dictionary<string, List<string>> GetCache(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes).Split('\r', '\n')
            .Where(p => !string.IsNullOrEmpty(p))
            .Select(p => p.Split(','))
            .ToLookup(p => p.First(), q => q.Last())
            .ToDictionary(p => p.Key, q => new List<string>(q));
    }

    public override void Awake()
    {
        base.Awake();
        patchListCache = new Dictionary<string, VersionInfo>();
        DirectoryHelper.CreateDirectory(Access.PersistentDataTempPath);
    }

    private IEnumerator Start()
    {
        //  下载记录补丁的文本获取补丁列表

        var request = Resources.LoadAsync<TextAsset>("resources");
        yield return request;

        var swww = new WWW(Application.streamingAssetsPath + "streamingAssets.txt");
        yield return swww;

        var pwww = new WWW(Application.persistentDataPath + "persistentAssets.txt");
        yield return pwww;

        var rwww = new WWW("" + "fileAssets.txt");
        yield return rwww;

        var cache = new Dictionary<string, List<string>>().Merge(
            GetCache(((TextAsset) request.asset).bytes),
            GetCache(swww.bytes),
            GetCache(pwww.bytes),
            GetCache(rwww.bytes)
            );

        //  下载记录补丁的文本获取补丁列表
        yield return Access.FromRemote(FilePatchInfo.PatchListName, res =>
        {
            if (res == null) return;
            DirectoryHelper.CreateDirectory(Access.PersistentDataPatchPath + FilePatchInfo.PatchListName);
            File.WriteAllBytes(Access.PersistentDataPatchPath + FilePatchInfo.PatchListName, res);

            var content = res.AES_Dencrypt();
            var versionInfo = Library.Helper.JsonHelper.ToObject<FileVersion.VersionInfo>(content);
            SoftwareVersion = versionInfo.softwareVersion;
            patchListCache = versionInfo.pathInfos.Select(p => new FilePatchInfo(p)).ToLookup(p => p.group)
                .ToDictionary(p => p.Key, q =>
                {
                    var go = gameObject.AddComponent<VersionInfo>();
                    go.Init(q.Key, new List<FilePatchInfo>(q).OrderBy(t => t.firstVersion).ToList());
                    return go;
                });
        });

        foreach (var info in patchListCache.Values)
        {
            var resourcesCacheList = resourcesCache.ContainsKey(info.GroupName)
                ? resourcesCache[info.GroupName]
                : new List<string>();
            var streamingAssetsCachelist = streamingAssetsCache.ContainsKey(info.GroupName)
                ? streamingAssetsCache[info.GroupName]
                : new List<string>();
            yield return info.InitStart(resourcesCacheList, streamingAssetsCachelist);
        }

        yield return Load("image/10253318_640x640_0.jpg", tex =>
        {
            texture2D = tex;
        });
    }

    private Texture2D texture2D = null;

    private void OnGUI()
    {
        if (texture2D == null)
            texture2D = ImageAccess.FileToTexture2D("image/10253312_640x640_0.jpg");
        if (texture2D == null)
            texture2D = ImageAccess.FileToTexture2D(Access.PathConvertToMd5("image/10253312_640x640_0.jpg", KeyMd5));
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

    private IEnumerator LoadObject(string path, Action<byte[], UnityEngine.Object> callAction)
    {
        string hash = Access.PathConvertToMd5(path, KeyMd5);
        foreach (var cache in patchListCache.Values)
        {
            FileDetailInfo resInfo = null;
            if (!cache.mainCache.TryGetValue(hash, out resInfo)) continue;
            if (resInfo.is_delete)
            {
                Debug.LogError("访问不在资源版本库中的资源!");
                yield break;
            }
            if (!resInfo.is_ready)
            {
                Debug.LogError("访问未准备好的资源!");
                yield break;
            }
            switch (resInfo.dataType)
            {
                case FileDetailInfo.DataType.ResourcesDataPath:
                    callAction.Invoke(null, Resources.Load(path));
                    yield break;
                case FileDetailInfo.DataType.StreamingAssetsPath:
                    yield return Access.FromStreamingAssetsPath(path, www => { callAction.Invoke(www, null); });
                    yield break;
                default:
                    yield return Access.FromPersistentDataTempPath(resInfo.path, www => { callAction.Invoke(www, null); });
                    yield break;
            }
        }

        Debug.LogError("无效资源!");
        callAction.Invoke(null, null);
    }

    public IEnumerator Load(string path, Action<Texture2D> callAction)
    {
        yield return LoadObject(path,(b, obj) =>
        {
            callAction.Invoke(b == null ? obj as Texture2D : ImageAccess.BytesToTexture2D(b));
        });
    }

    public IEnumerator Load(string path, Action<AudioClip> callAction)
    {
        yield return LoadObject(path, (b, obj) =>
        {
            callAction.Invoke(b == null ? obj as AudioClip : AudioAccess.BytesToAudioClip(path));
        });
    }

    public IEnumerator Load(string path, Action<byte[]> callAction)
    {
        yield return LoadObject(path, (b, obj) =>
        {
            callAction.Invoke(b == null ? (obj as TextAsset).bytes : b);
        });
    }

    public IEnumerator Load(string path, Action<string> callAction)
    {
        yield return LoadObject(path, (b, obj) =>
        {
            callAction.Invoke(b == null ? (obj as TextAsset).text : Encoding.UTF8.GetString(b));
        });
    }

    public IEnumerator Load(string path, Action<AssetBundle> callAction)
    {
        yield return LoadObject(path, (b, obj) =>
        {
            callAction.Invoke(b == null ? null : AssetBundle.LoadFromMemory(b));
        });
    }

//#if !(UNITY_ANDROID || UNITY_IPHONE)

//    public IEnumerator Load(string path, Action<MovieTexture> callAction)
//    {
//        yield return LoadObject(path, (b, obj) =>
//        {
//            callAction.Invoke(b == null ? obj as MovieTexture : b.movie);
//        });
//    }

//#endif

    #endregion
}