using System.Collections.Generic;

namespace Library.Extensions
{
    /// <summary>
    /// Random扩展
    /// </summary>
    public static partial class RandomExtensions
    {
#if !UNITY
        public static T RandomValue<T>(this T[] target)
        {
            if (target.Length == 0) return default(T);
            return target[new System.Random().Next(0, target.Length)];
        }

        public static T RandomValue<T>(this List<T> target)
        {
            if (target.Count == 0) return default(T);
            return target[new System.Random().Next(0, target.Count)];
        }
#endif
    }
}