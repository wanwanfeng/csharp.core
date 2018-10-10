﻿using System;
using System.Linq;
using System.Collections.Generic;
using Library.Extensions;

namespace UnityEngine.Library
{
    /// <summary>
    /// Unity扩展
    /// </summary>
    public static class ExtensionsForUnity
    {
        /// <summary>
        /// 获取相对于根节点的路径
        /// </summary>
        /// <param name="target"></param>
        /// <param name="haveRoot"></param>
        /// <returns></returns>
        public static string GetFullPath(this Component target, bool haveRoot = true)
        {
            var path = target.name;
            while (target.transform.parent != null)
            {
                path = target.transform.parent.name + "/" + path;
                target = target.transform.parent;
            }
            return haveRoot ? path : path.Replace(target.transform.root.name + "/", "");
        }


        /// <summary>
        /// 转为Vector3型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static Vector3 AsVector3(this string value, char separator = ',')
        {
            if (string.IsNullOrEmpty(value)) return Vector3.zero;
            string[] result = value.Split(separator);
            var temp = Array.ConvertAll(result, p => p.AsFloat());
            switch (temp.Length)
            {
                default:
                    return Vector3.zero;
                case 1:
                    return new Vector3(temp[0], 0, 0);
                case 2:
                    return new Vector3(temp[0], temp[1], 0);
                case 3:
                    return new Vector3(temp[0], temp[1], temp[2]);
            }
        }

        /// <summary>
        /// 转为Vector2型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static Vector2 AsVector2(this string value, char separator = ',')
        {
            if (string.IsNullOrEmpty(value)) return Vector2.zero;
            var result = value.Split(separator);
            var temp = Array.ConvertAll(result, p => p.AsFloat());
            switch (temp.Length)
            {
                default:
                    return Vector2.zero;
                case 1:
                    return new Vector2(temp[0], 0);
                case 2:
                    return new Vector2(temp[0], temp[1]);
            }
        }

        /// <summary>
        /// 转为string型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string AsString(this Vector3 value, char separator = ',')
        {
            return string.Format("{1}{0}{2}{0}{3}", separator, value.x, value.y, value.z);
        }

        /// <summary>
        /// 转为string型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string AsString(this Vector2 value, char separator = ',')
        {
            return string.Format("{1}{0}{2}", separator, value.x, value.y);
        }
    }


    public static class UnityExtensions
    {
        /// <summary>
        ///  获取孩子GameObject列表
        /// </summary>
        /// <param name="target"></param>
        /// <param name="isAll">真,获取全部孩子;否,获取显式孩子</param>
        /// <returns></returns>
        public static List<T> GetChildrenComponents<T>(this Transform target, bool isAll = true) where T : Component
        {
            var list = new List<T>();
            for (int i = 0; i < target.transform.childCount; i++)
            {
                var trans = target.transform.GetChild(i);
                if (isAll)
                {
                    list.Add(trans.GetComponent<T>());
                }
                else
                {
                    if (trans.gameObject.activeSelf)
                    {
                        list.Add(trans.GetComponent<T>());
                    }
                }
            }
            return list;
        }

        /// <summary>
        ///  获取孩子GameObject列表
        /// </summary>
        /// <param name="target"></param>
        /// <param name="isAll">真,获取全部孩子;否,获取显式孩子</param>
        /// <returns></returns>
        public static List<GameObject> GetChildren(this GameObject target, bool isAll = true)
        {
            var list = new List<GameObject>();
            for (int i = 0; i < target.transform.childCount; i++)
            {
                if (isAll)
                {
                    list.Add(target.transform.GetChild(i).gameObject);
                }
                else
                {
                    if (target.transform.GetChild(i).gameObject.activeSelf)
                    {
                        list.Add(target.transform.GetChild(i).gameObject);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 按路径获取物体
        /// </summary>
        /// <param name="go"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GameObject GetGameObject(this GameObject go, string path)
        {
            var trans = go.transform.Find(path);
            if (trans)
            {
                return trans.gameObject;
            }
            Debug.LogWarning("Error! Can not find the gameObject.");
            return null;
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
            {
                trans = target.transform.Find(path);
            }
            if (trans)
            {
                T t = trans.GetComponent<T>();
                return t == null;
            }
            return false;
        }

        /// <summary>
        /// 查找组件,若果没有，则自动添加一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Find<T>(this Component target, string path = null) where T : Component
        {
            var trans = target.transform;
            if (!string.IsNullOrEmpty(path))
                trans = target.transform.Find(path);
            return trans ? trans.GetOrAddComponent<T>() : default(T);
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
        /// 查找组件,若果没有，则自动添加一个
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Component Find(this Component target, Type type, string path = null)
        {
            var trans = target.transform;
            if (!string.IsNullOrEmpty(path))
                trans = target.transform.Find(path);
            return trans ? trans.GetOrAddComponent(type) : null;
        }

        /// <summary>
        /// 查找组件,若果没有，则自动添加一个
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Component Find(this GameObject target, Type type, string path = null)
        {
            return Find(target.transform, type, path);
        }

        /// <summary>
        ///  删除组件
        /// </summary>
        /// <param name="gameObject"></param>
        internal static void Destroy<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject != null)
            {
                Component component = gameObject.GetComponent<T>();
                if (component != null)
                {
                    Object.Destroy(component);
                }
            }
        }

        /// <summary>
        ///  使组件生效
        /// </summary>
        /// <param name="gameObject"></param>
        internal static void Enabled<T>(this GameObject gameObject) where T : Behaviour
        {
            if (gameObject != null)
            {
                Behaviour behaviour = gameObject.GetComponent<T>();
                if (behaviour != null)
                {
                    behaviour.enabled = true;
                }
            }
        }

        /// <summary>
        ///  使组件失效
        /// </summary>
        /// <param name="gameObject"></param>
        internal static void Disabled<T>(this GameObject gameObject) where T : Behaviour
        {
            if (gameObject != null)
            {
                Behaviour behaviour = gameObject.GetComponent<T>();
                if (behaviour != null)
                {
                    behaviour.enabled = false;
                }
            }
        }

        #region  Position

        public static void SetPosX(this Transform target, float value)
        {
            target.position = new Vector3(value, target.position.y, target.position.z);
        }

        public static void SetPosY(this Transform target, float value)
        {
            target.position = new Vector3(target.position.x, value, target.position.z);
        }

        public static void SetPosZ(this Transform target, float value)
        {
            target.position = new Vector3(target.position.x, target.position.y, value);
        }

        public static void SetLocalPosX(this Transform target, float value)
        {
            target.localPosition = new Vector3(value, target.localPosition.y, target.localPosition.z);
        }

        public static void SetLocalPosY(this Transform target, float value)
        {
            target.localPosition = new Vector3(target.localPosition.x, value, target.localPosition.z);
        }

        public static void SetLocalPosZ(this Transform target, float value)
        {
            target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, value);
        }

        #endregion

        /// <summary>
        ///随机位置
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static Vector3 RangeVector3(this Vector3 from, Vector3 to)
        {
            return new Vector3(UnityEngine.Random.Range(from.x, to.x), UnityEngine.Random.Range(from.y, to.y),
                UnityEngine.Random.Range(from.z, to.z));
        }

        /// <summary>
        /// 随机位置
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static Vector2 RangeVector2(this Vector2 from, Vector2 to)
        {
            return new Vector3(UnityEngine.Random.Range(from.x, to.x), UnityEngine.Random.Range(from.y, to.y));
        }

        #region GetOrAddComponent

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
        /// <param name="component"></param>
        /// <returns></returns>
        public static Component GetOrAddComponent(this Component component, Type type)
        {
            if (component == null) return null;
            return component.gameObject.GetOrAddComponent(type);
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
        /// 获取组件(没有时自动添加)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (component == null)
                return default(T);
            T t = component.GetComponent<T>();
            if (t == null) t = component.gameObject.AddComponent<T>();
            return t;
        }

        #endregion


        /// <summary>
        /// 移除物体下所有孩子
        /// </summary>
        /// <param name="target"></param>
        public static void ClearChild(this Transform target)
        {
            if (target.childCount == 0) return;
            while (target.childCount != 0)
                Object.Destroy(target.GetChild(0).gameObject);
        }

        /// <summary>
        /// 向物体添加孩子
        /// </summary>
        /// <param name="target"></param>
        /// <param name="child"></param>
        public static void AddChild(this Transform target, Transform child)
        {
            child.transform.SetParent(target);
            child.transform.localPosition = Vector3.zero;
            child.transform.localScale = Vector3.one;
            child.transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// 克隆物体并添加到parent下
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="clone"></param>
        /// <returns></returns>
        public static GameObject CloneAndAddClild(this Transform parent, GameObject clone)
        {
            var obj = (GameObject) UnityEngine.Object.Instantiate(clone);
            parent.AddChild(obj.transform);
            return obj;
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
                for (int i = 0; i < needCount; i++)
                {
                    parent.GetChild(i).gameObject.SetActive(true);
                    list.Add(parent.GetChild(i));
                }

                for (int i = needCount; i < parent.childCount; i++)
                {
                    parent.GetChild(i).gameObject.SetActive(false);
                }
            }

            return list;
        }

        #endregion

    }
}