﻿using UnityEngine;
using System.Linq;
using Library.Extensions;
using UnityEditor.Library;
using Object = UnityEngine.Object;
using Library;

namespace UnityEditor
{
    public class ShowMenu : Editor
    {
        [MenuItem("OpenScene/First")]
        public static void OpenFirstScene()
        {
            SceneManagement.EditorSceneManager.OpenScene(EditorBuildSettings.scenes.First(p => p.enabled).path);
        }

        [MenuItem("OpenScene/Second")]
        public static void OpenSecondScene()
        {
            SceneManagement.EditorSceneManager.OpenScene(EditorBuildSettings.scenes.Where(p => p.enabled).Skip(1).First().path);
        }

        [MenuItem("OpenScene/Third")]
        public static void OpenThirdScene()
        {
            SceneManagement.EditorSceneManager.OpenScene(EditorBuildSettings.scenes.Where(p => p.enabled).Skip(2).First().path);
        }

        [MenuItem("Assets/CopyToClipboard/Path")]
        public static void GetObjectPaths()
        {
            var go = Selection.GetFiltered(typeof (Object), SelectionMode.TopLevel);
            EditorUtils.CopyToClipboard((go.Select(item => AssetDatabase.GetAssetPath(item)).Join()));
        }

        [MenuItem("Assets/CopyToClipboard/Name")]
        public static void GetObjecNames()
        {
            var go = Selection.GetFiltered(typeof(Object), SelectionMode.TopLevel);
            EditorUtils.CopyToClipboard((go.Select(item => item.name).Join()));
        }

        [MenuItem("GameObject/CopyToClipboard/Path", false, 0)]
        public static void GetGameObjectsPath()
        {
            var go = Selection.GetFiltered(typeof (GameObject), SelectionMode.Editable);
            EditorUtils.CopyToClipboard(go.Select(item => ((GameObject)item).transform.GetFullPath()).Join());
        }

        [MenuItem("GameObject/CopyToClipboard/Name", false, 0)]
        public static void GetGameObjectsName()
        {
            var go = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable);
            EditorUtils.CopyToClipboard(go.Select(item => ((GameObject)item).name).Join());
        }

        [MenuItem("CONTEXT/Transform/CopyToClipboard/LocalPosition")]
        public static void CopyPosition()
        {
            Vector3 pos = Selection.activeTransform.localPosition;
            EditorUtils.CopyToClipboard(string.Format("{0},{1},{2}", pos.x, pos.y, pos.z));
        }

        [MenuItem("CONTEXT/Transform/CopyToClipboard/LocalEulerAngles")]
        public static void CopyLocalEulerAngles()
        {
            Vector3 pos = Selection.activeTransform.localEulerAngles;
            EditorUtils.CopyToClipboard(string.Format("{0},{1},{2}", pos.x, pos.y, pos.z));
        }

        [MenuItem("CONTEXT/Transform/CopyToClipboard/LocalScale")]
        public static void CopyScale()
        {
            Vector3 pos = Selection.activeTransform.localScale;
            EditorUtils.CopyToClipboard(string.Format("{0},{1},{2}", pos.x, pos.y, pos.z));
        }

        [MenuItem("CONTEXT/Transform/CopyToClipboard/LocalPosition and LocalEulerAngles")]
        public static void CopyPositionAndLocalEulerAngles()
        {
            Vector3 pos = Selection.activeTransform.localPosition;
            Vector3 angle = Selection.activeTransform.localEulerAngles;
            EditorUtils.CopyToClipboard(string.Format("{0},{1},{2};{3},{4},{5}", pos.x, pos.y, pos.z, angle.x, angle.y, angle.z));
        }

    }
}