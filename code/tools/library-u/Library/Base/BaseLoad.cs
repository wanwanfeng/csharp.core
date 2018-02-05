using System;
using System.Collections.Generic;

namespace UnityEngine.Library
{

    #region 非MonoBehaviour基类

    public class BaseLoad : ILoad
    {
        /// <summary>
        /// 判断资源委托注入
        /// </summary>
        public static Func<string, bool> OnFuncHasPath { private get; set; }

        /// <summary>
        /// 加载资源委托注入
        /// </summary>
        public static Func<string, bool, Action<Object>, Object> OnFuncLoad { private get; set; }

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
        public T Load<T>(string filePath, bool instance = false, Action<T> callAction = null) where T : Object
        {
            if (OnFuncLoad != null)
            {
                return OnFuncLoad(filePath, instance, obj => { if (callAction != null) callAction(obj as T); }) as T;
            }
            var t = (instance ? Object.Instantiate(Resources.Load<T>(filePath)) : Resources.Load<T>(filePath)) as T;
            if (callAction != null) callAction(t as T);
            return t;
        }

        public virtual GameObject CreateObject(string path, Transform parent = null)
        {
            var obj = Load<GameObject>(path);
            return CreateObject(obj, parent);
        }

        public virtual GameObject CreateObject(GameObject obj, Transform parent = null)
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

        public virtual Sprite LoadSprite(string path)
        {
            return Load<Sprite>(path);
        }
    }

    #endregion
}