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
        public static bool HasPath(this ILoad load, string path)
        {
            return OnFuncHasPath.Call(path);
        }

        /// <summary>
        /// 资源加载接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="callAction"></param>
        /// <returns></returns>
        public static T Load<T>(this ILoad load, string filePath, Action<T> callAction = null) where T : Object
        {
            if (OnFuncLoad != null)
            {
                return OnFuncLoad(filePath, obj =>
                {
                   callAction.Call(obj as T);
                }) as T;
            }
            var t = Resources.Load<T>(filePath) as T;
            callAction.Call(t as T);
            return t;
        }

        #endregion

        #region prefab克隆

        /// <summary>
        /// 预制体资源根目录
        /// </summary>
        public static string PrefabRoot = "";

        public static GameObject CreateObject(this ILoad load, string path, Transform parent = null)
        {
            var obj = load.Load<GameObject>(PrefabRoot + path);
            return load.CreateObject(obj, parent);
        }

        public static GameObject CreateObject(this ILoad load, GameObject obj, Transform parent = null)
        {
            var go = Object.Instantiate(obj) as GameObject;
            if (parent != null)
                parent.AddChild(go.transform);
            return go;
        }

        /// <summary>
        /// 加载UGUI Sprite图集注入
        /// </summary>
        public static Func<string, Sprite> OnLoadSprite { private get; set; }

        /// <summary>
        /// Sprite资源根目录
        /// </summary>
        public static string SpriteRoot = "";

        public static Sprite LoadSprite(this ILoad load, string path)
        {
            path = SpriteRoot + path;
            return OnLoadSprite != null ? OnLoadSprite.Invoke(path) : load.Load<Sprite>(path);
        }

        #endregion
    }
}

