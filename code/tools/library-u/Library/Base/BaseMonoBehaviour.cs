using System;
using System.Collections.Generic;

namespace UnityEngine.Library
{

    #region MonoBehaviour基类

    public abstract partial class BaseMonoBehaviour : MonoBehaviour, ILoad
    {
        #region 资源加载

        protected static readonly BaseLoad BaseLoad = new BaseLoad();

        /// <summary>
        /// 资源存在与否接口
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool HasPath(string path)
        {
            return BaseLoad.HasPath(path);
        }

        /// <summary>
        /// 资源加载接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="callAction"></param>
        /// <returns></returns>
        public T Load<T>(string filePath, Action<T> callAction = null) where T : Object
        {
            return BaseLoad.Load(filePath, callAction);
        }

        #endregion

        #region prefab克隆

        public GameObject CreateObject(string path, Transform parent = null)
        {
            return BaseLoad.CreateObject(path, parent);
        }

        public GameObject CreateObject(GameObject obj, Transform parent = null)
        {
            return BaseLoad.CreateObject(obj, parent);
        }

        public Sprite LoadSprite(string path)
        {
            return BaseLoad.LoadSprite(path);
        }

        #endregion

        public virtual void Awake()
        {

        }

        public virtual void OnDestroy()
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {

        }
    }

    #endregion
}