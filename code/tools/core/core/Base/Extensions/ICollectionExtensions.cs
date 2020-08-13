using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Library.Extensions
{
    public static partial class ICollectionExtensions
    { 
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue defaultValue = default(TValue))
        {
            if (source.ContainsKey(key))
                return source[key];
            return defaultValue;
        }

        public static object Get<TKey, TValue>(this Hashtable source, TKey key, TValue defaultValue = default(TValue))
        {
            if (source.ContainsKey(key))
                return source[key];
            return defaultValue;
        }
    }
}