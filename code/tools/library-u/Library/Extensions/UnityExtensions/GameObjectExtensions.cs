using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Library
{
    /// <summary>
    /// GameObject扩展
    /// </summary>
    public static class GameObjectExtensions
    {

        /// <summary>
        /// 获取组件(没有时自动添加)
        /// </summary>
        /// <param name="go"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Component GetOrAddComponent(this GameObject go, Type type)
        {
            if (go == null) return null;
            Component t = go.GetComponent(type);
            if (t == null) t = go.AddComponent(type);
            return t;
        }

        /// <summary>
        /// 获取组件(没有时自动添加)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go == null)
                return default(T);
            T t = go.GetComponent<T>();
            if (t == null) t = go.AddComponent<T>();
            return t;
        }


        /// <summary>
        /// 查找组件,若果没有，则自动添加一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Find<T>(this GameObject target, string path = null) where T : Component
        {
            return target.transform.Find<T>(path);
        }

        /// <summary>
        /// 判断路径下组件是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ComponentIsNull<T>(this GameObject target, string path = null) where T : Component
        {
            var trans = target.transform;
            if (!string.IsNullOrEmpty(path))
                trans = target.transform.Find(path);
            if (!trans) return false;
            T t = trans.GetComponent<T>();
            return t == null;
        }

        /// <summary>
        /// 获取组件,若果没有，则自动添加一个
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Component Find(this GameObject target, Type type, string path = null)
        {
            return target.transform.Find(type, path);
        }

        /// <summary>
        /// 判断路径下组件是否存在
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ComponentIsNull(this GameObject target, Type type, string path = null)
        {
            var trans = target.transform;
            if (!string.IsNullOrEmpty(path))
                trans = target.transform.Find(path);
            if (!trans) return false;
            var component = trans.GetComponent(type);
            return component == null;
        }

        /// <summary>
        ///  删除组件
        /// </summary>
        /// <param name="gameObject"></param>
        internal static void Destroy<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject == null) return;
            Component component = gameObject.GetComponent<T>();
            if (component == null) return;
            Object.Destroy(component);
        }

        /// <summary>
        ///  使组件生效
        /// </summary>
        /// <param name="gameObject"></param>
        internal static void Enabled<T>(this GameObject gameObject) where T : Behaviour
        {
            if (gameObject == null) return;
            var behaviour = gameObject.GetComponent<T>();
            if (behaviour == null) return;
            behaviour.enabled = true;
        }

        /// <summary>
        ///  使组件失效
        /// </summary>
        /// <param name="gameObject"></param>
        internal static void Disabled<T>(this GameObject gameObject) where T : Behaviour
        {
            if (gameObject == null) return;
            var behaviour = gameObject.GetComponent<T>();
            if (behaviour == null) return;
            behaviour.enabled = false;
        }

        /// <summary>
        ///  使组件生效
        /// </summary>
        /// <param name="gameObject"></param>
        internal static void EnabledAll<T>(this GameObject gameObject) where T : Behaviour
        {
            if (gameObject == null) return;
            var behaviour = gameObject.GetComponents<T>();
            if (behaviour == null || behaviour.Length == 0) return;
            behaviour.ToList().ForEach(p => p.enabled = true);
        }

        /// <summary>
        ///  使组件失效
        /// </summary>
        /// <param name="gameObject"></param>
        internal static void DisabledAll<T>(this GameObject gameObject) where T : Behaviour
        {
            if (gameObject == null) return;
            var behaviour = gameObject.GetComponents<T>();
            if (behaviour == null || behaviour.Length == 0) return;
            behaviour.ToList().ForEach(p => p.enabled = false);
        }


        #region 获取预定义个数的物体或组件

        /// <summary>
        /// 获取预定义个数的同种类型子物体
        /// 个数不够时自动克隆，多余时隐藏
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="needCount"></param>
        /// <returns></returns>
        public static List<Transform> GetChildTransforms(this Transform parent, int needCount)
        {
            var clone = parent.GetChild(0);
            return clone == null
                ? new List<Transform>()
                : ChildTransforms(parent, needCount, clone.gameObject);
        }

        /// <summary>
        /// 获取预定义个数的兄弟姐妹物体(包括自己)
        /// 个数不够时自动克隆，多余时隐藏
        /// </summary>
        /// <param name="clone"></param>
        /// <param name="needCount"></param>
        /// <returns></returns>
        public static List<Transform> GetSiblingTransforms(this GameObject clone, int needCount)
        {
            Transform parent = clone.transform.parent;
            return parent == null
                ? new List<Transform>()
                : ChildTransforms(parent, needCount, clone);
        }

        /// <summary>
        /// 获取预定义个数的同种类型子物体组件
        /// 个数不够时自动克隆，多余时隐藏
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="needCount"></param>
        /// <returns></returns>
        public static List<T> GetChildComponents<T>(this Transform parent, int needCount) where T : Component
        {
            var clone = parent.GetChild(0);
            return clone == null
                ? new List<T>()
                : ChildTransforms(parent, needCount, clone.gameObject).Select(p => p.GetOrAddComponent<T>()).ToList();
        }

        /// <summary>
        /// 获取预定义个数的兄弟姐妹物体组件(包括自己)
        /// 个数不够时自动克隆，多余时隐藏
        /// </summary>
        /// <param name="clone"></param>
        /// <param name="needCount"></param>
        /// <returns></returns>
        public static List<T> GetSiblingComponents<T>(this GameObject clone, int needCount) where T : Component
        {
            Transform parent = clone.transform.parent;
            return parent == null
                ? new List<T>()
                : ChildTransforms(parent, needCount, clone).Select(p => p.GetOrAddComponent<T>()).ToList();
        }

        private static List<Transform> ChildTransforms(Transform parent, int needCount, GameObject clone)
        {
            List<Transform> list = new List<Transform>();

            if (parent.childCount < needCount)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    parent.GetChild(i).gameObject.SetActive(true);
                    list.Add(parent.GetChild(i));
                }

                while (parent.childCount < needCount)
                {
                    var obj = parent.CloneAndAddClild(clone);
                    obj.SetActive(true);
                    list.Add(obj.transform);
                }
            }
            else
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    if (i < needCount)
                    {
                        parent.GetChild(i).gameObject.SetActive(true);
                        list.Add(parent.GetChild(i));
                    }
                    else
                    {
                        parent.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }

            return list;
        }

        #endregion

    }
}