using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class Access
{

#if UNITY_EDITOR
    public static UnityEditor.BuildTarget activeBuildTarget { get; private set; }
#endif

    /// <summary>
    /// 编辑器模式下使用
    /// </summary>
    /// <returns></returns>
    public static string BuildTargetPath
    {
        get
        {
            string savePath = "";
#if UNITY_EDITOR
            switch (activeBuildTarget)
            {
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    savePath = "PC";
                    break;
#if UNITY_4_6||UNITY_4_7
                    case UnityEditor.BuildTarget.iPhone:
                        savePath = "Raw";
#elif UNITY_5
                case UnityEditor.BuildTarget.iOS:
                    savePath = "Raw";
#endif
                    break;
                case UnityEditor.BuildTarget.Android:
                    savePath = "assets";
                    break;
                default:
                    savePath = "";
                    break;
            }
#endif
            return savePath + "/";
        }
    }


    /// <summary>
    /// 运行模式下使用
    /// </summary>
    /// <returns></returns>
    public static string runTargetPath
    {
        get
        {
#if UNITY_STANDALONE_WIN
             return"ver/pc/";
#elif UNITY_ANDROID
            return "ver/android/";
#elif UNITY_IPHONE || UNITY_IOS
            return "ver/ios/";
#endif
        }
    }

    public static string DataPath { get; private set; }
    public static string PersistentDataPath { get; private set; }
    public static string PersistentDataPatchPath { get; set; }
    public static string PersistentDataTempPath { get; set; }
    public static string StreamingAssetsPath { get; set; }
    public static string StreamingAssetsPathWithoutPrefix { get; set; }

    static Access()
    {
#if UNITY_EDITOR
        activeBuildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
#endif
        DataPath = Application.dataPath;

        PersistentDataPath = Application.persistentDataPath + "/";
        PersistentDataTempPath = PersistentDataPath + "Temp";
        PersistentDataPatchPath = PersistentDataPath + "Patch";

        Func<string> getStreamingAssetsPath = () =>
        {
#if UNITY_EDITOR
            return "file:///" + DataPath + "/StreamingAssets/";
#elif UNITY_STANDALONE_WIN
            return "file:///" + DataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
            return "jar:file://" + DataPath + "!/assets/";
#elif UNITY_IPHONE || UNITY_IOS
            return "file://" + DataPath + "/Raw/";  
#endif
        };
        StreamingAssetsPath = getStreamingAssetsPath();
        Func<string> getStreamingAssetsPathWithoutPrefix = () =>
        {
#if UNITY_EDITOR
            return DataPath + "/StreamingAssets/";
#elif UNITY_STANDALONE_WIN
            return dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
            return dataPath + "!/assets/";
#elif UNITY_IPHONE || UNITY_IOS
            return dataPath + "/Raw/";  
#endif
        };
        StreamingAssetsPathWithoutPrefix = getStreamingAssetsPathWithoutPrefix();
    }

    private class UrlInfo
    {
        public string Url { get; private set; }
        public int RetryCount { get; set; }

        public UrlInfo(string url)
        {
            this.Url = url;
            this.RetryCount = 0;
        }
    }

    private static List<UrlInfo> _urlList = new List<UrlInfo>();

    public static void SetUrls(params string[] urls)
    {
        _urlList = urls.Select(p => new UrlInfo(p)).ToList();
    }

    /// <summary>
    /// 远程加载, 失败后重试
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callAction"></param>
    /// <returns></returns>
    public static IEnumerator FromRemote(string path, Action<byte[]> callAction)
    {
        var urls = _urlList.OrderBy(p => p.RetryCount).ToList();
        foreach (var urlInfo in urls)
        {
            using (WWW www = new WWW(urlInfo.Url + path))
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
                    www.Dispose();
                    yield break;
                }
                urlInfo.RetryCount++;
                www.Dispose();
            }
        }
        Debug.LogError("get www failed ! url:" + path);
        callAction.Invoke(null);
    }

    /// <summary>
    /// 远程加载, 失败后重试
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callAction"></param>
    /// <returns></returns>
    public static IEnumerator FromRemote(string path, Action<string> callAction)
    {
        yield return FromRemote(path, p => { callAction.Invoke(p == null ? null : Encoding.UTF8.GetString(p)); });
    }

    /// <summary>
    /// 本地加载无须重试
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callAction"></param>
    /// <returns></returns>
    private static IEnumerator FromLocal(string path, Action<byte[]> callAction)
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

    /// <summary>
    /// 本地加载无须重试
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callAction"></param>
    /// <returns></returns>
    private static IEnumerator FromLocal(string path, Action<string> callAction)
    {
        yield return FromLocal(path, p => { callAction.Invoke(p == null ? null : Encoding.UTF8.GetString(p)); });
    }

    public static IEnumerator FromStreamingAssetsPath(string path, Action<byte[]> callAction)
    {
        yield return FromLocal(StreamingAssetsPath + path, callAction);
    }

    public static IEnumerator FromStreamingAssetsPath(string path, Action<string> callAction)
    {
        yield return FromLocal(StreamingAssetsPath + path, callAction);
    }

    public static IEnumerator FromPersistentDataTempPath(string path, Action<byte[]> callAction)
    {
        yield return FromLocal(PersistentDataTempPath + path, callAction);
    }

    public static IEnumerator FromPersistentDataTempPath(string path, Action<string> callAction)
    {
        yield return FromLocal(PersistentDataTempPath + path, callAction);
    }

    /// <summary>
    /// PersistentDataPath
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] FromPersistentDataPath(string path)
    {
        path = PersistentDataPath + path;
        return File.Exists(path) ? File.ReadAllBytes(path) : null;
    }

    internal static string PathConvertToMd5(string path, string KeyMd5)
    {
        throw new NotImplementedException();
    }

    [Description("当前网络处于连接WiFi网络状态，更新需消耗{0}M流量，是否继续？")]
    public static bool IsWifi
    {
        get { return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork; }
    }

    [Description("当前网络处于连接移动网络状态，建议在Wifi环境下更新游戏，更新需消耗{0}M流量，是否继续？")]
    public static bool IsDataStream
    {
        get { return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork; }
    }
}

public class AudioAccess : Access
{

    internal static AudioClip BytesToAudioClip(string path)
    {
        throw new NotImplementedException();
    }
}

public class ImageAccess : Access
{

    internal static Texture2D BytesToTexture2D(byte[] b)
    {
        throw new NotImplementedException();
    }

    internal static Texture2D FileToTexture2D(string p)
    {
        throw new NotImplementedException();
    }
}

