using System.Collections;
using System.Collections.Generic;

namespace Library.Extensions
{
    public static class ICollectionExtensions
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> source,
            params IDictionary<TKey, TValue>[] other)
        {
            if (other == null) return source;
            foreach (var dictionary in other)
            {
                if (dictionary == null) continue;
                foreach (var value in dictionary)
                {
                    source[value.Key] = value.Value;
                }
            }

            return source;
        }

        public static Hashtable Merge(this Hashtable source, params Hashtable[] other)
        {
            if (other == null) return source;
            foreach (var hashTable in other)
            {
                if (hashTable == null) continue;
                foreach (var key in hashTable.Keys)
                {
                    source.Add(key, hashTable[key]);
                }
            }

            return source;
        }
    }
}