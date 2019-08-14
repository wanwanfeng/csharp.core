using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Library.Serialization
{
    public class HashTable : ISerializationCallbackReceiver
    {
        [SerializeField] private List<object> keys;
        [SerializeField] private List<object> values;

        private System.Collections.Hashtable target;

        public System.Collections.Hashtable ToHashtable()
        {
            return target;
        }

        public HashTable(System.Collections.Hashtable target)
        {
            this.target = target;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<object>(target.Keys.Cast<object>());
            values = new List<object>(target.Values.Cast<object>());
        }

        public void OnAfterDeserialize()
        {
            var count = Math.Min(keys.Count, values.Count);
            target = new System.Collections.Hashtable(count);
            for (var i = 0; i < count; ++i)
            {
                target.Add(keys[i], values[i]);
            }
        }
    }
}