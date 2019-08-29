using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;

[CustomEditor(typeof(UnityEngine.UIAtlas))]
[CanEditMultipleObjects()]
public class UIAtlasEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Sort"))
        {
            foreach (var result in Selection.objects.OfType<UnityEngine.UIAtlas>())
            {
                result.SortByKey();
                EditorUtility.SetDirty(result);
            }
        }
        GUILayout.Space(10);
        base.DrawDefaultInspector();
    }

    #region UGUI

    /// <summary>
    /// 精灵生成设置PackingTag
    /// </summary>
    [MenuItem("Assets/UGUI/SetPackingTag")]
    private static void SetPackingTag()
    {
        var objects = Selection.instanceIDs.ToList();
        if (objects.Count == 0)
            return;
        for (var j = 0; j < objects.Count; j++)
        {
            var id = objects[j];
            var filePath = AssetDatabase.GetAssetPath(id);
            EditorUtility.DisplayProgressBar("操作中,请稍后...", filePath, (float) (j + 1)/objects.Count);
            if (!filePath.Contains("."))
                continue;
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
            if (sprite == null) continue;

            TextureImporter ti = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (ti != null)
            {
                var tis = new TextureImporterSettings();
                ti.ReadTextureSettings(tis);
                ti.spritePackingTag = new FileInfo(filePath).Directory.Name;
                ti.SetTextureSettings(tis);
                AssetDatabase.ImportAsset(filePath);
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    ///// <summary>
    ///// 精灵生成预制
    ///// </summary>
    //[MenuItem("Assets/UGUI/AtlasMaker")]
    //private static void MakeAtlas()
    //{
    //    var objects = Selection.objects.ToList();
    //    if (objects.Count == 0)
    //        return;
    //    for (var j = 0; j < objects.Count; j++)
    //    {
    //        var obj = objects[j];
    //        var filePath = AssetDatabase.GetAssetPath(obj);
    //        EditorUtility.DisplayProgressBar("操作中,请稍后...", filePath, (float)(j + 1) / objects.Count);
    //        if (!filePath.Contains("."))
    //            continue;
    //        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
    //        if (sprite == null) continue;
    //        var outPath = filePath.Replace("/ToBundle/", "/Resources/");


    //        FileInfo info = new FileInfo(outPath);
    //        outPath = outPath.Replace(info.Extension, ".prefab");
    //        //Debug.LogError(outPath);

    //        if (info.Directory != null && info.Directory.Exists == false)
    //            Directory.CreateDirectory(info.Directory.FullName);
    //        var go = new GameObject(sprite.name);
    //        go.AddComponent<SpriteRenderer>().sprite = sprite;
    //        PrefabUtility.CreatePrefab(outPath, go);
    //        GameObject.DestroyImmediate(go);
    //    }
    //    EditorUtility.ClearProgressBar();
    //    AssetDatabase.Refresh();
    //}

    /// <summary>
    /// 精灵生成预制
    /// </summary>
    [MenuItem("Assets/UGUI/AtlasMaker")]
    private static void MakeAtlas()
    {
        var objects = Selection.objects.ToList();
        if (objects.Count == 0)
            return;
        for (var j = 0; j < objects.Count; j++)
        {
            var obj = objects[j];
            var filePath = AssetDatabase.GetAssetPath(obj);
            EditorUtility.DisplayProgressBar("操作中,请稍后...", filePath, (float) (j + 1)/objects.Count);
            if (!filePath.Contains("."))
                continue;
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
            if (sprite == null) continue;
            var outPath = filePath.Replace("/ToBundle/", "/Resources/")
                .Replace("/" + Path.GetFileName(filePath), ".asset");
            var db = CreateAsset<UnityEngine.UIAtlas>(outPath);
            if (db.Contains(sprite.name)) continue;
            db.Add(sprite);

            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 创建Asset
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    internal static T CreateAsset<T>(string assetPath) where T : ScriptableObject
    {
        CreateDirectory(assetPath);
        var db = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
        if (db != null) return db;
        db = ScriptableObject.CreateInstance(typeof(T)) as T;
        AssetDatabase.CreateAsset(db, assetPath);
        return db;
    }

    /// <summary>
    /// 输入全路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static void CreateDirectory(string path)
    {
        string dir = Path.GetDirectoryName(path);
        if (dir != null && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    #endregion
}


///// <summary>
///// 假如我想自定义打包方式咋办?比如我想设置图片打包格式，或者图集大小等等怎么办？把如下代码放在Editor文件夹下， 在代码里面就可以设置图集的属性了。
///// http://www.xuanyusong.com/archives/3315
///// DefaultPackerPolicy will pack rectangles no matter what Sprite mesh type is unless their packing tag contains "[TIGHT]".
///// </summary>
//public class DefaultPackerPolicySample : UnityEditor.Sprites.IPackerPolicy
//{
//    protected class Entry
//    {
//        public Sprite sprite;
//        public AtlasSettings settings;
//        public string atlasName;
//        public SpritePackingMode packingMode;
//    }

//    public virtual int GetVersion() { return 1; }

//    protected virtual string TagPrefix { get { return "[TIGHT]"; } }
//    protected virtual bool AllowTightWhenTagged { get { return true; } }

//    public void OnGroupAtlases(BuildTarget target, PackerJob job, int[] textureImporterInstanceIDs)
//    {
//        List<Entry> entries = new List<Entry>();

//        foreach (int instanceID in textureImporterInstanceIDs)
//        {
//            TextureImporter ti = EditorUtility.InstanceIDToObject(instanceID) as TextureImporter;

//            //TextureImportInstructions ins = new TextureImportInstructions();
//            //ti.ReadTextureImportInstructions(ins, target);

//            //TextureImporterSettings tis = new TextureImporterSettings();
//            //ti.ReadTextureSettings(tis);

//            //Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(ti.assetPath).Select(x => x as Sprite).Where(x => x != null).ToArray();
//            //foreach (Sprite sprite in sprites)
//            //{
//            //    //在这里设置每个图集的参数
//            //    Entry entry = new Entry();
//            //    entry.sprite = sprite;
//            //    entry.settings.format = ins.desiredFormat;
//            //    entry.settings.usageMode = ins.usageMode;
//            //    entry.settings.colorSpace = ins.colorSpace;
//            //    entry.settings.compressionQuality = ins.compressionQuality;
//            //    entry.settings.filterMode = Enum.IsDefined(typeof(FilterMode), ti.filterMode) ? ti.filterMode : FilterMode.Bilinear;
//            //    entry.settings.maxWidth = 1024;
//            //    entry.settings.maxHeight = 1024;
//            //    entry.atlasName = ParseAtlasName(ti.spritePackingTag);
//            //    entry.packingMode = GetPackingMode(ti.spritePackingTag, tis.spriteMeshType);

//            //    entries.Add(entry);
//            //}

//            Resources.UnloadAsset(ti);
//        }

//        // First split sprites into groups based on atlas name
//        var atlasGroups =
//            from e in entries
//            group e by e.atlasName;
//        foreach (var atlasGroup in atlasGroups)
//        {
//            int page = 0;
//            // Then split those groups into smaller groups based on texture settings
//            var settingsGroups =
//                from t in atlasGroup
//                group t by t.settings;
//            foreach (var settingsGroup in settingsGroups)
//            {
//                string atlasName = atlasGroup.Key;
//                if (settingsGroups.Count() > 1)
//                    atlasName += string.Format(" (Group {0})", page);

//                job.AddAtlas(atlasName, settingsGroup.Key);
//                foreach (Entry entry in settingsGroup)
//                {
//                    job.AssignToAtlas(atlasName, entry.sprite, entry.packingMode, SpritePackingRotation.None);
//                }

//                ++page;
//            }
//        }
//    }

//    protected bool IsTagPrefixed(string packingTag)
//    {
//        packingTag = packingTag.Trim();
//        if (packingTag.Length < TagPrefix.Length)
//            return false;
//        return (packingTag.Substring(0, TagPrefix.Length) == TagPrefix);
//    }

//    private string ParseAtlasName(string packingTag)
//    {
//        string name = packingTag.Trim();
//        if (IsTagPrefixed(name))
//            name = name.Substring(TagPrefix.Length).Trim();
//        return (name.Length == 0) ? "(unnamed)" : name;
//    }

//    private SpritePackingMode GetPackingMode(string packingTag, SpriteMeshType meshType)
//    {
//        if (meshType == SpriteMeshType.Tight)
//            if (IsTagPrefixed(packingTag) == AllowTightWhenTagged)
//                return SpritePackingMode.Tight;
//        return SpritePackingMode.Rectangle;
//    }
//}

///// <summary>
///// 有可能我们会同时把很多图片都拖入unity中，虽然可以全选在设置图片的pack tag，但是我觉得最好全自动完成，比如我们把图片放在不同的文件夹下，那么文件夹的名子就可以用做Atals的名子。
///// http://www.xuanyusong.com/archives/3315
///// </summary>
//public class Post : AssetPostprocessor
//{
//    //void OnPostprocessTexture(Texture2D texture)
//    //{
//    //    string atlasName = Path.GetFileName(Path.GetDirectoryName(assetPath));
//    //    var textureImporter = assetImporter as TextureImporter;
//    //    if (textureImporter == null) return;
//    //    textureImporter.textureType = TextureImporterType.Sprite;
//    //    textureImporter.spritePackingTag = atlasName;
//    //    textureImporter.mipmapEnabled = false;
//    //}
//}
