using UnityEngine;
using UnityEditor;
using System.Linq;
using Library.Extensions;
using UnityEditor.Library;
using Object = UnityEngine.Object;

namespace UnityEditor
{
    public class ShowMenu : Editor
    {
        [MenuItem("Assets/将路径复制到剪帖板")]
        public static void GetObjectPaths()
        {
            var go = Selection.GetFiltered(typeof (Object), SelectionMode.TopLevel);
            EditorUtils.CopyToClipboard((go.Select(item => AssetDatabase.GetAssetPath(item)).ToArray().JoinToString()));
        }

        [MenuItem("GameObject/将路径复制到剪帖板", false, 0)]
        public static void GetGameObjectsPath()
        {
            var go = Selection.GetFiltered(typeof (GameObject), SelectionMode.Editable);
            EditorUtils.CopyToClipboard(go.Select(item => GetPath(((GameObject)item).transform)).ToArray().JoinToString());
        }

        public static string GetPath(Transform target, bool haveRoot = true)
        {
            var path = target.name;
            while (target.parent != null)
            {
                path = target.parent.name + "/" + path;
                target = target.parent;
            }
            return haveRoot ? path : path.Replace(target.root.name + "/", "");
        }

        [MenuItem("CONTEXT/Transform/Copy LocalPosition")]
        public static void CopyPosition()
        {
            Vector3 pos = Selection.activeTransform.localPosition;
            EditorUtils.CopyToClipboard(string.Format("{0},{1},{2}", pos.x, pos.y, pos.z));
        }

        [MenuItem("CONTEXT/Transform/Copy LocalEulerAngles")]
        public static void CopyLocalEulerAngles()
        {
            Vector3 pos = Selection.activeTransform.localEulerAngles;
            EditorUtils.CopyToClipboard(string.Format("{0},{1},{2}", pos.x, pos.y, pos.z));
        }

        [MenuItem("CONTEXT/Transform/Copy LocalScale")]
        public static void CopyScale()
        {
            Vector3 pos = Selection.activeTransform.localScale;
            EditorUtils.CopyToClipboard(string.Format("{0},{1},{2}", pos.x, pos.y, pos.z));
        }

        [MenuItem("CONTEXT/Transform/Copy LocalPosition and LocalEulerAngles")]
        public static void CopyPositionAndLocalEulerAngles()
        {
            Vector3 pos = Selection.activeTransform.localPosition;
            Vector3 angle = Selection.activeTransform.localEulerAngles;
            EditorUtils.CopyToClipboard(string.Format("{0},{1},{2};{3},{4},{5}", pos.x, pos.y, pos.z, angle.x, angle.y, angle.z));
        }

    }
}