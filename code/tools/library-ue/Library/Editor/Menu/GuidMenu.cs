using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.Library
{
    class GuidMenu: BaseMenu
    {
        [MenuItem("Tools/Guid/ReplaceGuid")]
        private static void ReplaceGuid()
        {
            if (!EditorUtility.DisplayDialog("提示", "是否进行GUID替换?", "确定", "取消")) return;
            Queue<string> queue = new Queue<string>(File.ReadAllLines("ReplaceGuid.txt"));
            string guid = queue.Dequeue().Trim();
            Debug.LogError("guid:" + guid);
            string newGuid = queue.Dequeue().Trim();
            Debug.LogError("newGuid:" + newGuid);

            if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(newGuid))
                return;

            List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" };
            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            int startIndex = 0;

            Stack<string> res = new Stack<string>();
            EditorApplication.update = delegate ()
            {
                string file = files[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("替换资源中", file,
                    (float)startIndex / (float)files.Length);

                string content = File.ReadAllText(file);
                if (content.Contains(guid))
                {
                    content = content.Replace(guid, newGuid);
                    res.Push(GetRelativeAssetsPath(file));
                    Debug.Log(file, AssetDatabase.LoadAssetAtPath(res.Peek(), typeof(Object)));
                }
                File.WriteAllText(file, content);
                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    Debug.Log("替换结束");
                }
            };
        }
    }
}
