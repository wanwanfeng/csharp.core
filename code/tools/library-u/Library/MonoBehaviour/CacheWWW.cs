using System.Collections.Generic;
using UnityEngine;

namespace Library
{
#if UNITY2017
    public class CacheWWW : WWW
    {
        public CacheWWW(string url) : base(url) { }
        public CacheWWW(string url, WWWForm form) : base(url, form) { }
        public CacheWWW(string url, byte[] postData) : base(url, postData) { }
        public CacheWWW(string url, byte[] postData, Dictionary<string, string> headers) : base(url, postData, headers) { }


        private string _error;
        public new string error
        {
            get { return _error == null ? _error = base.error : _error; }
            set { _error = value; }
        }

        private byte[] _bytes;
        public new byte[] bytes
        {
            get { return _bytes == null ? _bytes = base.bytes : _bytes; }
            set { _bytes = value; }
        }

        private string _text;
        public new string text
        {
            get { return _text == null ? _text = base.text : _text; }
            set { _text = value; }
        }

        private Texture2D _texture;
        public new Texture2D texture
        {
            get { return _texture == null ? _texture = base.texture : _texture; }
            set { _texture = value; }
        }

        private AssetBundle _assetBundle;
        public new AssetBundle assetBundle
        {
            get { return _assetBundle == null ? _assetBundle = base.assetBundle : _assetBundle; }
            set { _assetBundle = value; }
        }

        private AudioClip _audioClip;
        public new AudioClip audioClip
        {
            get { return _audioClip == null ? _audioClip = (GetAudioClip()) : _audioClip; }
            set { _audioClip = value; }
        }

        public new void Dispose()
        {
            error = null;
            bytes = null;
            text = null;
            texture = null;
            assetBundle = null;
            audioClip = null;
            base.Dispose();
        }
    }
#endif
}