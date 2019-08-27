using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Library.Helper
{
    public class TextureHelper
    {
        public static Texture2D GetRawTexture(string path)
        {
            Texture2D texture = new Texture2D(2, 2);
            try
            {
                byte[] lBytes = System.IO.File.ReadAllBytes(path);
                texture.LoadImage(lBytes, false);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogErrorFormat("Fail to load texture [{0}].", path);
                return null;
            }
            return texture;
        }

        public static bool HasAlpha(Color[] colors)
        {
            return colors.Any(p => p.a < 1);
        }

        public static void SplitAlpha(Texture2D texture, string path, string alpha_path)
        {
            SaveRGB(texture, path);
            SaveAlpha(texture, alpha_path);
        }

        public static void SaveRGB(Texture2D texture, string path)
        {
            var srcPixels = texture.GetPixels();
            Texture2D target = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
            target.SetPixels(srcPixels.Select(p => new Color(p.r, p.g, p.b)).ToArray());
            target.Apply();
            File.WriteAllBytes(path, target.EncodeToJPG(100));
        }

        public static void SaveAlpha(Texture2D texture, string path)
        {
            var srcPixels = texture.GetPixels();
            if (!HasAlpha(srcPixels))
                throw new Exception("!srcPixels.HasAlpha()");
            Texture2D target = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
            target.SetPixels(srcPixels.Select(p => new Color(p.a, p.a, p.a)).ToArray());
            target.Apply();
            File.WriteAllBytes(path, target.EncodeToJPG(100));
        }

        public static void CombineAlpha(Texture2D texture, string alpha_path, string path)
        {
            Texture2D alpha_texture = GetRawTexture(alpha_path);
            CombineAlpha(texture, alpha_texture, path);
        }

        public static void CombineAlpha(Texture2D texture, Texture2D alpha_texture, string path)
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

        /// <summary>
        /// 强制2次幂
        /// </summary>
        /// <param name="alpha_texture"></param>
        /// <param name="path"></param>
        public static void ForceSquared(Texture2D texture, string path, TextAnchor forceOption)
        {
            int max = Mathf.Max(texture.width, texture.height);
            int w = Mathf.NextPowerOfTwo(max);
            var srcPixels = texture.GetPixels();
            Texture2D target = new Texture2D(w, w, texture.format, false);
            var color = srcPixels.Select((p, i) => new Color(p.r, p.g, p.b, p.a)).ToArray();
            switch (forceOption)
            {
                default:
                case TextAnchor.LowerLeft:
                    target.SetPixels(0, 0, texture.width, texture.height, color);
                    break;
                case TextAnchor.LowerCenter:
                    target.SetPixels((w - texture.width) / 2, 0, texture.width, texture.height, color);
                    break;
                case TextAnchor.LowerRight:
                    target.SetPixels(w - texture.width, 0, texture.width, texture.height, color);
                    break;
                case TextAnchor.UpperLeft:
                    target.SetPixels(0, w - texture.height, texture.width, texture.height, color);
                    break;
                case TextAnchor.UpperCenter:
                    target.SetPixels((w - texture.width) / 2, w - texture.height, texture.width, texture.height, color);
                    break;
                case TextAnchor.UpperRight:
                    target.SetPixels(w - texture.width, w - texture.height, texture.width, texture.height, color);
                    break;
                case TextAnchor.MiddleLeft:
                    target.SetPixels(0, (w - texture.height) / 2, texture.width, texture.height, color);
                    break;
                case TextAnchor.MiddleCenter:
                    target.SetPixels((w - texture.width) / 2, (w - texture.height) / 2, texture.width, texture.height, color);
                    break;
                case TextAnchor.MiddleRight:
                    target.SetPixels(w - texture.width, (w - texture.height) / 2, texture.width, texture.height, color);
                    break;
            }
            target.Apply();
            File.WriteAllBytes(path, target.EncodeToPNG());
        }
    }
}
