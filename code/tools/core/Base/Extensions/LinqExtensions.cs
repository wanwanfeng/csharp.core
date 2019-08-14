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
        public static Action<string> LogAction = p => { Console.WriteLine(p); };

        public static void ForEach<T>(this T[] source, Action<T> action)
        {
            for (int index = 0, max = source.Length; index < max; ++index)
                action(source[index]);
        }

        public static void ForEach<T>(this T[] source, Action<T, int> action)
        {
            for (int index = 0, max = source.Length; index < max; ++index)
                action(source[index], index);
        }

        public static void ForEach<T>(this T[] source, Action<T, int, int> action)
        {
            for (int index = 0, max = source.Length; index < max; ++index)
                action(source[index], index, max);
        }

        public static void ForEach<T>(this T[] source, Action<T, int, T[]> action)
        {
            for (int index = 0, max = source.Length; index < max; ++index)
                action(source[index], index, source);
        }

        public static void ForEach<T>(this T[] source, Action<T> action, string tip)
        {
            for (int index = 0, max = source.Length; index < max; ++index)
            {
                action(source[index]);
                LogAction(string.Format("{2} : {0} \t{1}", index + "/" + source.Length, source[index],
                    tip ?? "is now"));
            }
        }

        public static void ForEach<T>(this List<T> source, Action<T> action)
        {
            for (int index = 0, max = source.Count; index < max; ++index)
                action(source[index]);
        }

        public static void ForEach<T>(this List<T> source, Action<T, int> action)
        {
            for (int index = 0, max = source.Count; index < max; ++index)
                action(source[index], index);
        }

        public static void ForEach<T>(this List<T> source, Action<T, int, int> action)
        {
            for (int index = 0, max = source.Count; index < max; ++index)
                action(source[index], index, max);
        }

        public static void ForEach<T>(this List<T> source, Action<T, int, List<T>> action)
        {
            for (int index = 0, max = source.Count; index < max; ++index)
                action(source[index], index, source);
        }

        public static void ForEach<T>(this List<T> source, Action<T> action, string tip)
        {
            for (int index = 0, max = source.Count; index < max; ++index)
            {
                action(source[index]);
                LogAction(string.Format("{2} : {0} \t{1}", index + "/" + source.Count, source[index], tip ?? "is now"));
            }
        }

        public static void ForEach<T, TV>(this IDictionary<T, TV> source, Action<KeyValuePair<T, TV>> action)
        {
            foreach (KeyValuePair<T, TV> pair in source)
            {
                action(pair);
            }
        }

        public static void ForEach<T, TV>(this IDictionary<T, TV> source, Action<KeyValuePair<T, TV>, int> action)
        {
            var index = 0;
            foreach (var pair in source)
            {
                action(pair, ++index);
            }
        }

        public static void ForEach<T, TV>(this IDictionary<T, TV> source, Action<KeyValuePair<T, TV>, int, int> action)
        {
            var index = 0;
            foreach (var pair in source)
            {
                action(pair, ++index, source.Count);
            }
        }

        public static void ForEach<T, TV>(this IDictionary<T, TV> source,
            Action<KeyValuePair<T, TV>, int, IDictionary<T, TV>> action)
        {
            var index = 0;
            foreach (var pair in source)
            {
                action(pair, ++index, source);
            }
        }

        public static void ForEach<T, TV>(this IDictionary<T, TV> source, Action<KeyValuePair<T, TV>> action,
            string tip)
        {
            var index = 0;
            foreach (var pair in source)
            {
                action(pair);
                LogAction(string.Format("{2} : {0} \t{1}", ++index + "/" + source.Count, pair.Key, tip ?? "is now"));
            }
        }

        /// <summary>
        /// 拾取集合中后几个元素
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> TakeLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            return source.Reverse().Skip(count);
            var array = source.ToArray();
            return array.Skip(Math.Max(0, array.Length - count));
        }
    }
}