using System.Collections.Generic;
using UnityEngine;

namespace Library
{
    public class WWW
    {
        private UnityEngine.WWW www { get; set; }
        public WWW(string url) { www = new UnityEngine.WWW(url); }
        public WWW(string url, WWWForm form) { www = new UnityEngine.WWW(url, form); }
        public WWW(string url, byte[] postData) { www = new UnityEngine.WWW(url, postData); }
        public WWW(string url, byte[] postData, Dictionary<string, string> headers) { www = new UnityEngine.WWW(url, postData, headers); }

        private string _error;
        public string error
        {
            get { return _error == null ? _error = www.error : _error; }
            set { _error = value; }
        }

        private byte[] _bytes;
        public byte[] bytes
        {
            get { return _bytes == null ? _bytes = www.bytes : _bytes; }
            set { _bytes = value; }
        }

        private string _text;
        public string text
        {
            get { return _text == null ? _text = www.text : _text; }
            set { _text = value; }
        }

        private Texture2D _texture;
        public Texture2D texture
        {
            get { return _texture == null ? _texture = www.texture : _texture; }
            set { _texture = value; }
        }

        private AssetBundle _assetBundle;
        public AssetBundle assetBundle
        {
            get { return _assetBundle == null ? _assetBundle = www.assetBundle : _assetBundle; }
            set { _assetBundle = value; }
        }

        private AudioClip _audioClip;
        public AudioClip audioClip
        {
            get { return _audioClip == null ? _audioClip = (www.GetAudioClip()) : _audioClip; }
            set { _audioClip = value; }
        }

        public void Dispose()
        {
            error = null;
            bytes = null;
            text = null;
            texture = null;
            assetBundle = null;
            audioClip = null;
            www.Dispose();
        }
    }
}
