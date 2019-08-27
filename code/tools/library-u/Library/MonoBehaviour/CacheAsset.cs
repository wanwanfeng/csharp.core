using Library.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Library
{
    class CacheAsset<T> where T : UnityEngine.Object
    {
        public Dictionary<string, T> cache;

        public CacheAsset()
        {
            cache = new Dictionary<string, T>();
        }

        public bool TryGetValue(string path, out T t)
        {
            return cache.TryGetValue(path, out t);
        }

        public T this[string path]
        {
            get
            {
                TryGetValue(path, out T t);
                return t;
            }
            set { cache[path] = value; }
        }

        public void Clear()
        {
            cache.Clear();
        }
    }

    public static class CacheAsset
    {
        private readonly static CacheAsset<Texture2D> CacheTexture2D;
        private readonly static CacheAsset<AudioClip> CacheAudioClip;
        private readonly static CacheAsset<AssetBundle> CacheAssetBundle;

        static CacheAsset()
        {
            CacheTexture2D = new CacheAsset<Texture2D>();
            CacheAudioClip = new CacheAsset<AudioClip>();
            CacheAssetBundle = new CacheAsset<AssetBundle>();
        }

        public static Texture2D GetTexture2D(string path)
        {
            Texture2D asset = null;
            if (CacheTexture2D.TryGetValue(path, out asset))
                return asset;
            asset = TextureHelper.GetRawTexture(path);
            CacheTexture2D[path] = asset;
            return asset;
        }

        public static void ClearTexture2D(string path)
        {
            CacheTexture2D.Clear();
        }

        public static AudioClip GetAudioClip(string path)
        {
            AudioClip asset = null;
            if (CacheAudioClip.TryGetValue(path, out asset))
                return asset;
            asset = AudioClipHelper.GetRawAudioClip(path);
            CacheAudioClip[path] = asset;
            return asset;
        }

        public static void ClearAudioClip(string path)
        {
            CacheAudioClip.Clear();
        }

        public static AssetBundle GetAssetBundle(string path)
        {
            AssetBundle asset = null;
            if (CacheAssetBundle.TryGetValue(path, out asset))
                return asset;
            asset = AssetBundle.LoadFromFile(path);
            CacheAssetBundle[path] = asset;
            return asset;
        }

        public static void ClearAssetBundle(string path)
        {
            CacheAssetBundle.Clear();
        }
    }
    
}
