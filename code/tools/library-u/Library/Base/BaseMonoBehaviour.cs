using System;
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

    #region 加载接口

    public interface ILoad
    {
        bool HasPath(string path);
        T Load<T>(string filePath, bool instance = false, Action<T> callAction = null) where T : Object;
    }

    public class BaseLoad : ILoad
    {
        public static Func<string, bool> OnFuncHasPath { private get; set; }
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

    public partial class BaseMonoBehaviour : MonoBehaviour, ILoad
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
        /// <param name="instance"></param>
        /// <param name="callAction"></param>
        /// <returns></returns>
        public T Load<T>(string filePath, bool instance = false, Action<T> callAction = null) where T : Object
        {
            return BaseLoad.Load(filePath, instance, callAction);
        }

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