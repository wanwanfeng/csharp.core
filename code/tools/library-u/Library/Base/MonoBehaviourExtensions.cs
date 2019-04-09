using System;

namespace UnityEngine.Library
{
    public static partial class MonoBehaviourExtensions
    {
        #region 资源加载

        static readonly BaseLoad BaseLoad = new BaseLoad();

        /// <summary>
        /// 资源存在与否接口
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool HasPath(this MonoBehaviour monoBehaviour, string path)
        {
            return BaseLoad.HasPath(path);
        }

        /// <summary>
        /// 资源加载接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="monoBehaviour"></param>
        /// <param name="filePath"></param>
        /// <param name="callAction"></param>
        /// <returns></returns>
        public static T Load<T>(this MonoBehaviour monoBehaviour, string filePath, Action<T> callAction = null)
            where T : Object
        {
            return BaseLoad.Load(filePath, callAction);
        }

        #endregion

        #region prefab克隆

        public static GameObject CreateObject(this MonoBehaviour monoBehaviour, string path, Transform parent = null)
        {
            return BaseLoad.CreateObject(path, parent);
        }

        public static GameObject CreateObject(this MonoBehaviour monoBehaviour, GameObject obj, Transform parent = null)
        {
            return BaseLoad.CreateObject(obj, parent);
        }

        public static Sprite LoadSprite(this MonoBehaviour monoBehaviour, string path)
        {
            return BaseLoad.LoadSprite(path);
        }

        #endregion
    }
}