using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Library.Extensions
{
    /// <summary>
    /// Texture扩展
    /// </summary>
    public static partial class TextureExtensions
    {
        public static void GetTextureRGB(this Texture2D texture, string path)
        {
            var srcPixels = texture.GetPixels();
            Texture2D target = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
            target.SetPixels(srcPixels.Select(p => new Color(p.r, p.g, p.b)).ToArray());
            target.Apply();
            File.WriteAllBytes(path, target.EncodeToJPG(100));
        }

        public static void GetTextureAlpha(this Texture2D texture,string path)
        {
            var srcPixels = texture.GetPixels();
            Texture2D target = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
            target.SetPixels(srcPixels.Select(p => new Color(p.a, p.a, p.a)).ToArray());
            target.Apply();
            File.WriteAllBytes(path, target.EncodeToJPG(100));
        }
    }
}