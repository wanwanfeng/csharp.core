using Library.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityEditor.Library
{
    public class ScriptableObjectUtils: BaseEditorUtils
    {
        /// <summary>
        /// 创建新Asset
        /// </summary>
        /// <returns></returns>
        public static T NewAsset<T>(string outPath) where T : ScriptableObject
        {
            var t = LoadAssetAtPath<T>(outPath);
            if (t != null)
            {
                AssetDatabase.DeleteAsset(outPath);
            }
            t = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(t, outPath);
            return t;
        }

        public static void NewAsset<T>(Action<T> runAction, string outPath) where T : ScriptableObject
        {
            outPath += ".asset";
            FileHelper.CreateDirectory(outPath);
            var db = NewAsset<T>(outPath);
            runAction(db);
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("提示", "创建Asset成功", "确定");
            Selection.activeObject = db;
        }

        /// <summary>
        /// 载入Asset
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static T OpenAsset<T>(string dbName) where T : ScriptableObject
        {
            T db = LoadAssetAtPath<T>(dbName);
            if (db != null) return db;
            db = ScriptableObject.CreateInstance<T>();
            FileHelper.CreateDirectory(dbName);
            AssetDatabase.CreateAsset(db, dbName);
            return db;
        }
    }
}
