using System;
using System.Collections.Generic;

namespace UnityEngine.Library
{
    /// <summary>
    /// Random扩展
    /// </summary>
    public static class RandomExtensions
    {
        public static T RandomValue<T>(this T[] target)
        {
            if (target.Length == 0) return default(T);
            return target[Random.Range(0, target.Length)];
        }

        public static T RandomValue<T>(this List<T> target)
        {
            if (target.Count == 0) return default(T);
            return target[Random.Range(0, target.Count)];
        }
    }
}