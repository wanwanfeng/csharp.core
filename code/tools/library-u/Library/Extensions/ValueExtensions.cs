using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Library.Extensions
{
    /// <summary>
    /// 值类型扩展
    /// </summary>
    public static partial class ValueExtensions
    {
        public static Vector2 Sum(this Vector2[] target)
        {
            var v = Vector2.zero;
            foreach (var item in target)
            {
                v += item;
            }
            return v;
        }

        public static Vector2 Sum(this List<Vector2> target)
        {
            var v = Vector2.zero;
            foreach (var item in target)
            {
                v += item;
            }
            return v;
        }

        public static Vector2 Average(this Vector2[] target)
        {
            if (target.Length == 0) return Vector2.zero;
            return target.Sum() / target.Length;
        }

        public static Vector2 Average(this List<Vector2> target)
        {
            if (target.Count == 0) return Vector2.zero;
            return target.Sum() / target.Count;
        }

        public static Vector3 Sum(this Vector3[] target)
        {
            var v = Vector3.zero;
            foreach (var item in target)
            {
                v += item;
            }
            return v;
        }

        public static Vector3 Sum(this List<Vector3> target)
        {
            var v = Vector3.zero;
            foreach (var item in target)
            {
                v += item;
            }
            return v;
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