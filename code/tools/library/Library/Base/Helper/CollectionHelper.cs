using System.Collections.Generic;

namespace Library.Helper
{
    public class CollectionHelper
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(Dictionary<TKey, TValue> source, params Dictionary<TKey, TValue>[] other)
        {
            foreach (var dictionary in other)
            {
                foreach (var value in dictionary)
                {
                    source[value.Key] = value.Value;
                }
            }

            return source;
        }
    }

    public static class CollectionHelperExtensions
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> source, params Dictionary<TKey, TValue>[] other)
        {
            return CollectionHelper.Merge(source, other);
        }
    }
}