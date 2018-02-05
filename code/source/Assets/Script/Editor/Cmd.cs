
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Compress;
using Library.Helper;
using UnityEngine;
using UnityEditor;

public class Cmd : Editor
{
    [MenuItem("Tools/GetFileData")]
    public static void GetFileData()
    {
        string[] paths = AssetDatabase.GetAllAssetPaths();
        GetValue("Assets/StreamingAssets/streamingAssets.txt", "StreamingAssets/", paths);
        GetValue("Assets/Resources/resources.txt", "Resources/", paths.Where(p => !p.Contains("Editor/")).ToArray());
    }

    private static void GetValue(string path, string flag, string[] list)
    {
        var cache =
            AssetDatabase.GetAllAssetPaths()
                .Where(p => File.Exists(p))
                .Where(p => !p.EndsWith(path))
                .Where(p => p.Contains(flag))
                .ToDictionary(q => q, p =>
                {
                    var index = p.LastIndexOf(flag, StringComparison.Ordinal);
                    return p.Substring(index + flag.Length);
                });
        FileHelper.CreateDirectory(path);
        File.WriteAllLines(path,
            cache.Select(
                p =>
                {
                    int index = p.Value.IndexOf('/');
                    string group = "";
                    if (index != -1)
                        group = p.Value.Substring(0, index);
                    return Library.Encrypt.MD5.Encrypt("潘之琳") + "," + p.Value + "," +
                           Library.Encrypt.MD5.Encrypt(p.Value);
                })
                .ToArray());
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/MakeZipFile")]
    public static void MakeZipFile()
    {
        Debug.LogError(DecompressUtils.MakeZipFile("E:\\Git\\yg\\trunk\\潘之琳"));
    }

    [MenuItem("Tools/UnMakeZipFile")]
    public static void UnMakeZipFile()
    {
        Debug.LogError(DecompressUtils.UnMakeZipFile("E:\\Git\\yg\\trunk\\潘之琳3.zip"));
    }
}
