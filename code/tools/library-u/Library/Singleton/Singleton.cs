﻿using System;
using System.Collections.Generic;

namespace UnityEngine.Library
{

    #region 单例

    /// <summary>
    /// 普通单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonBase<T> : BaseLoad where T : class, new()
    {
        private static T _instance;

        public static T Instance
        {
            get { return _instance ?? (_instance = new T()); }
            set { _instance = value; }
        }

        public static void Reset()
        {
            Instance = null;
        }

        protected SingletonBase()
        {
            Init();
        }

        public virtual void Init()
        {

        }
    }

    /// <summary>
    /// 场景单例(自动生成物体并挂载)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonBehaviourAuto<T> : BaseMonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var go = GameObject.Find("InstanceRoot") ?? new GameObject("InstanceRoot");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<T>();
                return _instance;
            }
            set { _instance = value; }
        }

        public virtual void Init()
        {

        }
    }

    /// <summary>
    /// 场景单例（已有物体手动挂载或代码AddComponent了）
    /// 子类Awake方法调用
    /// base.Awake();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonBehaviour<T> : BaseMonoBehaviour where T : BaseMonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get { return _instance ?? (_instance = FindObjectOfType<T>()); }
            set { _instance = value; }
        }

        public override void OnDestroy()
        {
            base.Awake();
            Instance = null;
        }
    }

    #endregion
}