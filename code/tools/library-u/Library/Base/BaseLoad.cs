using System;
using System.Collections.Generic;

namespace UnityEngine.Library
{

    #region 非MonoBehaviour基类

    public class BaseLoad : ILoad
    {
        #region prefab克隆

        /// <summary>
        /// 判断资源委托注入
        /// </summary>
        public static Func<string, bool> OnFuncHasPath { private get; set; }

        /// <summary>
        /// 加载资源委托注入
        /// </summary>
        public static Func<string, Action<Object>, Object> OnFuncLoad { private get; set; }

        /// <summary>
        /// 资源存在与否接口
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool HasPath(string path)
        {
            return OnFuncHasPath != null && OnFuncHasPath.Invoke(path);
        }

        /// <summary>
        /// 资源加载接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="instance"></param>
        /// <param name="callAction"></param>
        /// <returns></returns>
        public T Load<T>(string filePath, Action<T> callAction = null) where T : Object
        {
            if (OnFuncLoad != null)
            {
                return OnFuncLoad(filePath, obj =>
                {
                    if (callAction != null) callAction(obj as T);
                }) as T;
            }
            var t = Resources.Load<T>(filePath) as T;
            if (callAction != null) callAction(t as T);
            return t;
        }

        #endregion

        #region prefab克隆

        /// <summary>
        /// 预制体资源根目录
        /// </summary>
        public static string PrefabRoot = "";

        public GameObject CreateObject(string path, Transform parent = null)
        {
            var obj = Load<GameObject>(PrefabRoot + path);
            return CreateObject(obj, parent);
        }

        public GameObject CreateObject(GameObject obj, Transform parent = null)
        {
            var go = Object.Instantiate(obj) as GameObject;
            if (parent != null)
                parent.AddChild(go.transform);
            return go;
        }

        public void Destroy(Object obj)
        {
            Object.Destroy(obj);
        }

        public void DestroyImmediate(Object obj)
        {
            Object.DestroyImmediate(obj);
        }

        /// <summary>
        /// 加载UGUI Sprite图集注入
        /// </summary>
        public static Func<string, Sprite> OnLoadSprite { private get; set; }

        /// <summary>
        /// Sprite资源根目录
        /// </summary>
        public static string SpriteRoot = "";

        public Sprite LoadSprite(string path)
        {
            path = SpriteRoot + path;
            return OnLoadSprite != null ? OnLoadSprite.Invoke(path) : Load<Sprite>(path);
        }

        #endregion
    }

    #endregion

}

