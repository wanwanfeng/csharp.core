using System.Collections;
using System.Collections.Generic;

namespace Library.Extensions
{
    public static class ICollectionExtensions
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> source,
            params IDictionary<TKey, TValue>[] other)
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

        public static Hashtable Merge(this Hashtable source, params Hashtable[] other)
        {
            foreach (var hashTable in other)
            {
                foreach (var key in hashTable.Keys)
                {
                    source.Add(key, hashTable[key]);
                }
            }

            return source;
        }
    }
}