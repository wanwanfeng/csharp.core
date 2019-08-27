using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine
{
    public class UIAtlas : ScriptableObject
    {
        [Serializable]
        public class AtlasInfo
        {
            public string Key;
            public Sprite Sprite;
        }
         
        public List<AtlasInfo> Content = new List<AtlasInfo>();

        public bool Contains(string key)
        {
            var sp = Content.FirstOrDefault(p => p.Key == key);
            if (sp == null || sp.Sprite == null) return false;
            return true;
        }

        public void Add(Sprite sp)
        {
            Content.Add(new AtlasInfo()
            {
                Key = sp.name,
                Sprite = sp
            });
            SortByKey();
        }

        public Sprite Get(string key)
        {
            var sp = Content.FirstOrDefault(p => p.Key == key);
            return sp == null ? null : sp.Sprite;
        }

        public void SortByKey()
        {
            Content.Sort((p1, p2) => string.Compare(p1.Key, p2.Key, StringComparison.Ordinal));
        }
    }
}