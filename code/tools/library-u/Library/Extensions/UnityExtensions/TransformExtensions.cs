using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Library
{
    /// <summary>
    /// Transform扩展
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        ///  获取孩子GameObject列表
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static List<T> GetChildrenComponents<T>(this Transform target) where T : Component
        {
            if (target.childCount == 0) return new List<T>();
            return Enumerable.Range(0, target.childCount).Select(p => target.GetChild(p).GetComponent<T>()).ToList();
        }

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

        public static void SetPosition(this Transform target, float? x = null, float? y = null, float? z = null)
        {
            target.position = new Vector3(x ?? target.position.x, y ?? target.position.y, z ?? target.position.z);
        }

        public static void SetLocalPosition(this Transform target, float x = 0, float y = 0, float z = 0)
        {
            target.localPosition = new Vector3(x, y, z);
        }

        public static void SetLocalScale(this Transform target, float? x = null, float? y = null, float? z = null)
        {
            target.localScale = new Vector3(x ?? target.localScale.x, y ?? target.localScale.y, z ?? target.localScale.z);
        }

        public static void SetLocalEulerAngles(this Transform target, float? x = null, float? y = null, float? z = null)
        {
            target.localEulerAngles = new Vector3(x ?? target.localEulerAngles.x, y ?? target.localEulerAngles.y,
                z ?? target.localEulerAngles.z);
        }

        public static void SetRotation(this Transform target, float? x = null, float? y = null, float? z = null,
            float? w = null)
        {
            target.rotation = new Quaternion(x ?? target.rotation.x, y ?? target.rotation.y, z ?? target.rotation.z,
                w ?? target.rotation.w);
        }

        public static void SetLocalRotation(this Transform target, float? x = null, float? y = null, float? z = null,
            float? w = null)
        {
            target.localRotation = new Quaternion(x ?? target.localRotation.x, y ?? target.localRotation.y,
                z ?? target.localRotation.z, w ?? target.localRotation.w);
        }
    }
}