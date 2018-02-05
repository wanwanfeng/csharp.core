using UnityEngine;
using UnityEditor;
using System.Linq;
using Library.Extensions;
using Object = UnityEngine.Object;

namespace UnityEditor
{
    public class ShowMenu : Editor
    {
        [MenuItem("Assets/将路径复制到剪帖板")]
        private static void GetObjectPaths()
        {
            var go = Selection.GetFiltered(typeof (Object), SelectionMode.TopLevel);
            TextEditorPaste(go.Select(item => AssetDatabase.GetAssetPath(item)).ToArray().JoinToString());
        }

        [MenuItem("GameObject/将路径复制到剪帖板", false, 0)]
        private static void GetGameObjectPath()
        {
            var go = Selection.GetFiltered(typeof (GameObject), SelectionMode.Editable);
            TextEditorPaste(go.Select(item => GetPath(((GameObject) item).transform)).ToArray().JoinToString());
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


        /// <summary>
        /// 内容自动复制到剪切板
        /// </summary>
        /// <param name="content"></param>
        public static void TextEditorPaste(string content)
        {
            Debug.Log(content);
            TextEditor te = new TextEditor();
            te.content = new GUIContent() {text = content};
            te.SelectAll();
            te.Copy();
        }
    }
}