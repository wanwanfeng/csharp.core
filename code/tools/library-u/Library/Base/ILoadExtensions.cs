using Library.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Library
{
    public interface ILoad
    {
    }

    public static class ILoadExtensions
    {
        #region prefab克隆

        /// <summary>
        /// 资源存在与否接口
        /// ---------需要在项目内重载---------------
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool HasPath(this ILoad load, string path)
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
        public static T Load<T>(this ILoad load, string filePath, Action<T> callAction = null) where T : Object
        {
            var t = Resources.Load<T>(filePath) as T;
            callAction.Call(t as T);
            return t;
        }

        #endregion

        #region prefab克隆

        /// <summary>
        /// 预制体资源根目录
        /// ---------需要在项目内重载---------------
        /// </summary>
        public static GameObject CreateObject(this ILoad load, string path, Transform parent = null)
        {
            var obj = load.Load<GameObject>(path);
            return load.CreateObject(obj, parent);
        }

        public static GameObject CreateObject(this ILoad load, GameObject obj, Transform parent = null)
        {
            var go = Object.Instantiate(obj) as GameObject;
            if (parent != null)
                parent.AddChild(go.transform);
            return go;
        }

        #endregion
    }
}

