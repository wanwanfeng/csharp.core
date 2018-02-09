using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

public class BundleHelper : Editor
{

    //[MenuItem("AssetBundle/Clear All AssetBundle Name")]
    //public static void ClearAllAssetBundleName()
    //{
    //    AssetDatabase.RemoveUnusedAssetBundleNames();
    //}

    [MenuItem("Assets/AssetBundleName/Del")]
    public static void DelAssetBundleName()
    {
        string[] paths = Selection.instanceIDs.Select(p => AssetDatabase.GetAssetPath(p)).ToArray();
        foreach (var path in paths)
        {
            AssetImporter importer = AssetImporter.GetAtPath(path);
            importer.assetBundleName = "";
            importer.SaveAndReimport();
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/AssetBundleName/Set")]
    public static void SetAssetBundleName()
    {
        string[] paths = Selection.instanceIDs.Select(p => AssetDatabase.GetAssetPath(p)).ToArray();
        var flag = "Resources/";
        foreach (var path in paths)
        {
            var extension = Path.GetExtension(path);
            if (extension == null) continue;
            if (extension == ".prefab")
            {

            }
            else
            {
                var index = path.IndexOf(flag, StringComparison.Ordinal);
                var assetName = index != -1
                    ? path.Substring(index + flag.Length)
                    : Path.GetFileName(path);
                assetName = assetName.Replace(extension, "");
                AssetImporter importer = AssetImporter.GetAtPath(path);
                importer.assetBundleName = assetName;
                importer.SaveAndReimport();
                Debug.Log(path + "\n" + assetName);
            }
        }
        AssetDatabase.Refresh();
    }

    public static string GetBundleRoot()
    {
        string root = Application.dataPath.Replace("Assets", "RES/unity_asset/");
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.iOS:
                root += "ios/";
                break;
            case BuildTarget.Android:
                root += "android/";
                break;
            default:
                root += "pc/";
                break;
        }
        if (!Directory.Exists(root))
            Directory.CreateDirectory(root);
        return root;
    }

    public static BuildAssetBundleOptions GetBuildAssetBundleOptions()
    {
        return BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.DeterministicAssetBundle;
    }

    private static void BuildAssetList(List<string> assetsList)
    {
        AssetBundleBuild[] builds = assetsList.Select(p =>
        {
            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetBundleName = p + ".unity3d";
            assetBundleBuild.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(p);
            assetBundleBuild.assetBundleVariant="";
            return assetBundleBuild;
        }).ToArray();
        BuildPipeline.BuildAssetBundles(GetBundleRoot(), builds, GetBuildAssetBundleOptions(), EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }

    [MenuItem("AssetBundle/Build All Asset")]
    public static void BuildAllAsset()
    {
        BuildAssetList(AssetDatabase.GetAllAssetBundleNames().ToList());
    }

    [MenuItem("AssetBundle/Build All Asset But Not Ignore")]
    public static void Asset()
    {
        var alls = AssetDatabase.GetAllAssetBundleNames().ToList();
        var ignores = new List<string>()
        {

        };
        if (ignores.Count > 0)
        {
            var del = new List<string>();
            foreach (string all in alls)
            {
                foreach (var ignore in ignores)
                {
                    if (all.StartsWith(ignore))
                    {
                        del.Add(all);
                    }
                }
            }
            foreach (string s in del)
            {
                alls.Remove(s);
            }
        }
        BuildAssetList(alls);
    }

    [MenuItem("AssetBundle/Build Select Asset")]
    public static void BuildSelectAsset()
    {
        string[] paths = Selection.instanceIDs.Select(p => AssetDatabase.GetAssetPath(p)).ToArray();
        List<string> assetsList = paths.Select(p => AssetImporter.GetAtPath(p).assetBundleName).ToList();
        BuildAssetList(assetsList);
    }
}