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
        public static bool HasAlpha(this Color[] colors)
        {
            return colors.Any(p => p.a < 1);
        }

        public static void SplitAlpha(this Texture2D texture, string path, string alpha_path)
        {
            texture.SaveRGB(path);
            texture.SaveAlpha(alpha_path);
        }

        public static void SaveRGB(this Texture2D texture, string path)
        {
            var srcPixels = texture.GetPixels();
            Texture2D target = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
            target.SetPixels(srcPixels.Select(p => new Color(p.r, p.g, p.b)).ToArray());
            target.Apply();
            File.WriteAllBytes(path, target.EncodeToJPG(100));
        }

        public static void SaveAlpha(this Texture2D texture, string path)
        {
            var srcPixels = texture.GetPixels();
            if (!srcPixels.HasAlpha())
                throw new Exception("!srcPixels.HasAlpha()");
            Texture2D target = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
            target.SetPixels(srcPixels.Select(p => new Color(p.a, p.a, p.a)).ToArray());
            target.Apply();
            File.WriteAllBytes(path, target.EncodeToJPG(100));
        }

        public static void CombineAlpha(this Texture2D texture, string alpha_path, string path)
        {
            Texture2D alpha_texture = TextureHelper.GetRawTexture(alpha_path);
            texture.CombineAlpha(alpha_texture, path);
        }

        public static void CombineAlpha(this Texture2D texture, Texture2D alpha_texture, string path)
        {
            var alphaPixels = alpha_texture.GetPixels();
            var srcPixels = texture.GetPixels();
            if (srcPixels.Length != alphaPixels.Length)
                throw new Exception("srcPixels.Length != alphaPixels.Length");
            Texture2D target = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            target.SetPixels(srcPixels.Select((p, i) => new Color(p.r, p.g, p.b, alphaPixels[i].r)).ToArray());
            target.Apply();
            File.WriteAllBytes(path, target.EncodeToPNG());
        }
    }
}