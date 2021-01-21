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
			texture.SaveRGB(path);
			texture.SaveAlpha(alpha_path);
		}

        public static void SaveRGB(this Texture2D texture, string path)
        {
			var result =  TextureHelper.SaveRGB(texture);
			File.WriteAllBytes(path, result.EncodeToJPG(100));
        }

        public static void SaveAlpha(this Texture2D texture, string path)
        {
			var result = TextureHelper.SaveAlpha(texture);
			File.WriteAllBytes(path, result.EncodeToJPG(100));
		}

        public static void CombineAlpha(this Texture2D texture, string alpha_path, string path)
        {
			var result = TextureHelper.CombineAlpha(texture, alpha_path);
			File.WriteAllBytes(path, result.EncodeToPNG());
        }

        public static void CombineAlpha(this Texture2D texture, Texture2D alpha_texture, string path)
        {
			var result = TextureHelper.CombineAlpha(texture, alpha_texture);
			File.WriteAllBytes(path, result.EncodeToPNG());
		}

		/// <summary>
		/// 强制画布2次幂
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="path"></param>
		/// <param name="forceOption"></param>
		public static void ForceSquared(this Texture2D texture, string path, TextAnchor forceOption)
        {
			var result = TextureHelper.ForceSquared(texture, forceOption);
			File.WriteAllBytes(path, result.EncodeToPNG());
		}

		/// <summary>
		/// 画布缩放
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="path"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="forceOption"></param>
		public static void AdjustCanvas(this Texture2D texture, string path, int width, int height, TextAnchor forceOption)
		{
			var result = TextureHelper.AdjustCanvas(texture, width, height, forceOption);
			File.WriteAllBytes(path, result.EncodeToPNG());
		}

		/// <summary>
		/// 图像缩放
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="path"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public static void AdjustScale(this Texture2D texture, string path, int width, int height)
		{
			var result = TextureHelper.AdjustScale(texture, width, height);
			File.WriteAllBytes(path, result.EncodeToPNG());
		}

	}
}