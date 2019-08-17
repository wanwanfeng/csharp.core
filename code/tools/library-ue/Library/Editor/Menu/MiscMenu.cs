using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.Library
{
    class MiscMenu: BaseMenu
    {
        [MenuItem("Assets/Tools/Find CS Length")]
        public static void SearchCSLength()
        {
            int length = AssetDatabase.GetAllAssetPaths()
                .Where(p => p.EndsWith(".cs"))
                .Sum(p => File.ReadAllLines(p).Length);
            Debug.LogError("CS行数:" + length);
        }

        [MenuItem("Assets/Tools/Find References", false, 10)]
        static private void Find()
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(path))
            {
                string guid = AssetDatabase.AssetPathToGUID(path);
                List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" };
                string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                    .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
                int startIndex = 0;

                Stack<string> res = new Stack<string>();
                EditorApplication.update = delegate ()
                {
                    string file = files[startIndex];

                    bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

                    //if (Regex.IsMatch(File.ReadAllText(file), guid))
                    //{
                    //    Debug.Log(file, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));
                    //}

                    if (File.ReadAllText(file).Contains(guid))
                    {
                        res.Push(GetRelativeAssetsPath(file));
                        Debug.Log(file, AssetDatabase.LoadAssetAtPath(res.Peek(), typeof(Object)));
                    }

                    startIndex++;
                    if (isCancel || startIndex >= files.Length)
                    {
                        EditorUtility.ClearProgressBar();
                        EditorApplication.update = null;
                        startIndex = 0;
                        Debug.Log("匹配结束");
                        File.WriteAllLines("xxx.txt", res.ToArray());
                    }
                };
            }
        }

        [MenuItem("Assets/Tools/Find References", true)]
        static private bool VFind()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            return (!string.IsNullOrEmpty(path));
        }
    }
}
