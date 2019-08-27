using Library.Helper;
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
            return TextureHelper.HasAlpha(colors);
        }

        public static void SplitAlpha(this Texture2D texture, string path, string alpha_path)
        {
            TextureHelper.SplitAlpha(texture, path, alpha_path);
        }

        public static void SaveRGB(this Texture2D texture, string path)
        {
            TextureHelper.SaveRGB(texture, path);
        }

        public static void SaveAlpha(this Texture2D texture, string path)
        {
            TextureHelper.SaveAlpha(texture, path);
        }

        public static void CombineAlpha(this Texture2D texture, string alpha_path, string path)
        {
            TextureHelper.CombineAlpha(texture, alpha_path, path);

        }

        public static void CombineAlpha(this Texture2D texture, Texture2D alpha_texture, string path)
        {
            TextureHelper.CombineAlpha(texture, alpha_texture, path);
        }

        /// <summary>
        /// 强制2次幂
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="alpha_texture"></param>
        /// <param name="path"></param>
        /// <param name="forceOption"></param>
        public static void ForceSquared(this Texture2D texture, string path, TextAnchor forceOption)
        {
            TextureHelper.ForceSquared(texture, path, forceOption);

        }
    }
}