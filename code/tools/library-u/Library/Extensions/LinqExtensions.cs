using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Library
{
    /// <summary>
    /// Random扩展
    /// </summary>
    public static class LinqExtensions
    {
        //public static Vector2 Sum(this Vector2[] target)
        //{
        //    if (target.Length == 0) return Vector2.zero;
        //    return new Vector2(target.Sum(p => p.x), target.Sum(p => p.y));
        //}

        //public static Vector2 Sum(this List<Vector2> target)
        //{
        //    if (target.Count == 0) return Vector2.zero;
        //    return new Vector3(target.Sum(p => p.x), target.Sum(p => p.y));
        //}

        //public static Vector2 Average(this Vector2[] target)
        //{
        //    if (target.Length == 0) return Vector2.zero;
        //    return target.Sum() / target.Length;
        //}

        //public static Vector2 Average(this List<Vector2> target)
        //{
        //    if (target.Count == 0) return Vector2.zero;
        //    return target.Sum() / target.Count;
        //}

        public static Vector3 Sum(this Vector3[] target)
        {
            if (target.Length == 0) return Vector3.zero;
            return new Vector3(target.Sum(p => p.x), target.Sum(p => p.y), target.Sum(p => p.z));
        }

        public static Vector3 Sum(this List<Vector3> target)
        {
            if (target.Count == 0) return Vector3.zero;
            return new Vector3(target.Sum(p => p.x), target.Sum(p => p.y), target.Sum(p => p.z));
        }

        public static Vector3 Average(this Vector3[] target)
        {
            if (target.Length == 0) return Vector3.zero;
            return target.Sum() / target.Length;
        }

        public static Vector3 Average(this List<Vector3> target)
        {
            if (target.Count == 0) return Vector3.zero;
            return target.Sum() / target.Count;
        }
    }
}