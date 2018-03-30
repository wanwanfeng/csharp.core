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

    #endregion
}