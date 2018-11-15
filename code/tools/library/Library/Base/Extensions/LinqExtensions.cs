using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Extensions
{
    /// <summary>
    /// Linq扩展
    /// </summary>
    public static class LinqExtensions
    {
        public static void ForEach<T>(this T[] source, Action<T> action)
        {
            for (int index = 0; index < source.Length; ++index)
                action(source[index]);
        }

        public static void ForEach<T>(this T[] source, Action<T, int> action)
        {
            for (int index = 0; index < source.Length; ++index)
                action(source[index], index);
        }

        public static void ForEach<T>(this T[] source, Action<T, int, int> action)
        {
            for (int index = 0; index < source.Length; ++index)
                action(source[index], index, source.Length);
        }

        //public static void ForEach<T>(this T[] source, Action<T, float> action)
        //{
        //    for (int index = 0; index < source.Length; ++index)
        //        action(source[index], (float) index/source.Length);
        //}

        //public static void ForEach<T>(this T[] source, Action<T, string> action)
        //{
        //    for (int index = 0; index < source.Length; ++index)
        //        action(source[index], index + "/" + source.Length);
        //}

        public static void ForEach<T>(this List<T> source, Action<T> action)
        {
            for (int index = 0; index < source.Count; ++index)
                action(source[index]);
        }

        public static void ForEach<T>(this List<T> source, Action<T, int> action)
        {
            for (int index = 0; index < source.Count; ++index)
                action(source[index], index);
        }

        public static void ForEach<T>(this List<T> source, Action<T, int, int> action)
        {
            for (int index = 0; index < source.Count; ++index)
                action(source[index], index, source.Count);
        }

        //public static void ForEach<T>(this List<T> source, Action<T, float> action)
        //{
        //    for (int index = 0; index < source.Count; ++index)
        //        action(source[index], ((float) index/source.Count));
        //}

        //public static void ForEach<T>(this List<T> source, Action<T, string> action)
        //{
        //    for (int index = 0; index < source.Count; ++index)
        //        action(source[index], index + "/" + source.Count);
        //}


        public static void Parallel<T>(this List<T> value, Action<T> callAction)
        {
            // Parallel.ForEach(CheckPath(exs), p => { });
        }
    }
}