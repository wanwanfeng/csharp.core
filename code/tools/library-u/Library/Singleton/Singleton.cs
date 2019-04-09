using System;
using System.Collections.Generic;

namespace UnityEngine.Library
{
    #region 单例

    /// <summary>
    /// 场景单例(自动生成物体并挂载)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class SingletonBehaviourAuto<T> : MonoBehaviour where T : MonoBehaviour
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
    public abstract partial class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }
        public virtual void Awake()
        {
            if (Instance != null) return;
            Instance = this as T;
        }


        public virtual void OnDestroy()
        {
            Instance = null;
        }
    }

    #endregion
}