using System.Collections.Generic;

namespace Library.Helper
{
    public class CollectionHelper
    {
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(IDictionary<TKey, TValue> source, params IDictionary<TKey, TValue>[] other)
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
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> source, params IDictionary<TKey, TValue>[] other)
        {
            return (Dictionary<TKey, TValue>) CollectionHelper.Merge(source, other);
        }
    }
}