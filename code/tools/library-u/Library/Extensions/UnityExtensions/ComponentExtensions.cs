using System;
using UnityEngine;

namespace Library
{
    /// <summary>
    /// Component扩展
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// 获取相对于根节点的路径
        /// </summary>
        /// <param name="component"></param>
        /// <param name="haveRoot"></param>
        /// <returns></returns>
        public static string GetFullPath(this Component component, bool haveRoot = true)
        {
            var path = component.name;
            while (component.transform.parent != null)
            {
                path = component.transform.parent.name + "/" + path;
                component = component.transform.parent;
            }
            return haveRoot ? path : path.Substring(component.transform.root.name.Length + 1);
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
        /// <param name="component"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (component == null) return default(T);
            T t = component.GetComponent<T>();
            if (t == null) t = component.gameObject.AddComponent<T>();
            return t;
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
    }
}