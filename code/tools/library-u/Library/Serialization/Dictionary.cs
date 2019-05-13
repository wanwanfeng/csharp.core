using System;
using System.Collections.Generic;

namespace UnityEngine.Library.Serialization
{
    public class Dictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys;
        [SerializeField] private List<TValue> values;

        private System.Collections.Generic.Dictionary<TKey, TValue> target;

        public System.Collections.Generic.Dictionary<TKey, TValue> ToDictionary()
        {
            return target;
        }

        public Dictionary(System.Collections.Generic.Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(target.Keys);
            values = new List<TValue>(target.Values);
        }

        public void OnAfterDeserialize()
        {
            var count = Math.Min(keys.Count, values.Count);
            target = new System.Collections.Generic.Dictionary<TKey, TValue>(count);
            for (var i = 0; i < count; ++i)
            {
                target.Add(keys[i], values[i]);
            }
        }
    }
}