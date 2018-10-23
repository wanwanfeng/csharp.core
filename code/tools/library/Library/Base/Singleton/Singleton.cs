using System;
using System.Collections.Generic;

namespace Library
{

    #region 单例

    /// <summary>
    /// 普通单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class SingletonBase<T> where T : class, new()
    {
        private static readonly object singletonLock = new object(); //锁同步
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (singletonLock)
                {
                    return _instance ?? (_instance = new T());
                }
            }
        }

        public static void Reset()
        {
            _instance = null;
        }

        protected SingletonBase()
        {
            Init();
        }

        public virtual void Init()
        {

        }
    }

    #endregion
}