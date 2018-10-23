using System.Collections.Generic;
using System.Linq;
namespace UnityEngine.Library
{
    /// <summary>
    /// Component扩展
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
            var obj = (GameObject)UnityEngine.Object.Instantiate(clone);
            parent.AddChild(obj.transform);
            return obj;
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

    }
}