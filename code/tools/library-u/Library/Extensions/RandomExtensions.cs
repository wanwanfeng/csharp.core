using System;
using System.Collections.Generic;

namespace Library.Extensions
{
    /// <summary>
    /// Random扩展
    /// </summary>
    public static partial class RandomExtensions
    {
  #if UNITY
        public static T RandomValue<T>(this T[] target)
        {
            if (target.Length == 0) return default(T);
            return target[UnityEngine.Random.Range(0, target.Length)];
        }

        public static T RandomValue<T>(this IList<T> target)
        {
            if (target.Count == 0) return default(T);
            return target[UnityEngine.Random.Range(0, target.Count)];
        }
#endif
    }
}