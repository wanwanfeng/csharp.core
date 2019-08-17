using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Helper;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace UnityEditor.Library
{
    public partial class BaseEditorUtils
    {
        public static T LoadAssetAtPath<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.Log(typeof (T).Name + ":" + path);
            }
#if UNITY_4_6 || UNITY_4_7
            return UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof (T)) as T;
#elif UNITY_5
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
#endif

#else 
            return default(T);
#endif
        }
    }
}
