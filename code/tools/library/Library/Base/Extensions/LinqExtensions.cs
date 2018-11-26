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
        public static Action<string> WriteFunc = p => { Console.WriteLine(p); };

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

        public static void ForEach<T>(this T[] source, Action<T> action, string tip)
        {
            for (int index = 0; index < source.Length; ++index)
            {
                action(source[index]);
                WriteFunc(string.Format("{2} : {0} \t{1}", index + "/" + source.Length, source[index], tip ?? "is now"));
            }
        }

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

        public static void ForEach<T>(this List<T> source, Action<T> action, string tip)
        {
            for (int index = 0; index < source.Count; ++index)
            {
                action(source[index]);
                WriteFunc(string.Format("{2} : {0} \t{1}", index + "/" + source.Count, source[index], tip ?? "is now"));
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
            foreach (KeyValuePair<T, TV> pair in source)
            {
                action(pair, ++index);
            }
        }

        public static void ForEach<T, TV>(this IDictionary<T, TV> source, Action<KeyValuePair<T, TV>, int, int> action)
        {
            var index = 0;
            foreach (KeyValuePair<T, TV> pair in source)
            {
                action(pair, ++index, source.Count);
            }
        }

        public static void ForEach<T, TV>(this IDictionary<T, TV> source, Action<KeyValuePair<T, TV>> action, string tip)
        {
            var index = 0;
            foreach (KeyValuePair<T, TV> pair in source)
            {
                action(pair);
                WriteFunc(string.Format("{2} : {0} \t{1}", ++index + "/" + source.Count, pair.Key, tip ?? "is now"));
            }
        }
    }
}