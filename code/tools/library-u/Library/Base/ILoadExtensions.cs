using Library.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Library
{
    using Object = UnityEngine.Object;
    public static class ILoadExtensions
    {
        #region prefab克隆

        /// <summary>
        /// 资源存在与否接口
        /// ---------需要在项目内重载---------------
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool HasPath(this UnityEngine.MonoBehaviour load, string path)
        {
            return true;
        }

        /// <summary>
        /// 资源加载接口
        /// ---------需要在项目内重载---------------
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="callAction"></param>
        /// <returns></returns>
        public static T Load<T>(this UnityEngine.MonoBehaviour load, string filePath, Action<T> callAction = null) where T : Object
        {
            if (Resources.Load<T>(filePath) is T t)
            {
                callAction?.Invoke(t);
                return t;
            }
            callAction?.Invoke(null);
            return null;
        }

        #endregion

        #region prefab克隆

        /// <summary>
        /// 预制体资源根目录
        /// ---------需要在项目内重载---------------
        /// </summary>
        public static GameObject CreateObject(this UnityEngine.MonoBehaviour load, string path, Transform parent = null)
        {
            var obj = load.Load<GameObject>(path);
            return load.CreateObject(obj, parent);
        }

        public static GameObject CreateObject(this UnityEngine.MonoBehaviour load, GameObject obj, Transform parent = null)
        {
            if (Object.Instantiate(obj) is GameObject go)
            {
                parent?.AddChild(go.transform);
                return go;
            }
            return null;
        }

        #endregion
    }
}

