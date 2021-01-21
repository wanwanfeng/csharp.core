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

        public static Texture2D SaveRGB(Texture2D texture)
        {
            var srcPixels = texture.GetPixels();
            Texture2D target = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
            target.SetPixels(srcPixels.Select(p => new Color(p.r, p.g, p.b)).ToArray());
            target.Apply();
			return target;
		}

		public static Texture2D SaveAlpha(Texture2D texture)
        {
            var srcPixels = texture.GetPixels();
            if (!HasAlpha(srcPixels))
                throw new Exception("!srcPixels.HasAlpha()");
            Texture2D target = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
            target.SetPixels(srcPixels.Select(p => new Color(p.a, p.a, p.a)).ToArray());
            target.Apply();
			return target;
		}

		public static Texture2D CombineAlpha(Texture2D texture, string alpha_path)
        {
			return CombineAlpha(texture, GetRawTexture(alpha_path));
        }

        public static Texture2D CombineAlpha(Texture2D texture, Texture2D alpha_texture)
        {
            var alphaPixels = alpha_texture.GetPixels();
            var srcPixels = texture.GetPixels();
            if (srcPixels.Length != alphaPixels.Length)
                throw new Exception("srcPixels.Length != alphaPixels.Length");
            Texture2D target = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            target.SetPixels(srcPixels.Select((p, i) => new Color(p.r, p.g, p.b, alphaPixels[i].r)).ToArray());
            target.Apply();
			return target;
		}

		/// <summary>
		/// 强制画布2次幂
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="forceOption"></param>
		/// <returns></returns>
		public static Texture2D ForceSquared(Texture2D texture, TextAnchor forceOption)
		{
			int w = Mathf.NextPowerOfTwo(Mathf.Max(texture.width, texture.height));
			Texture2D target = AdjustCanvas(texture, w, w, forceOption);
			target.Apply();
			return target;
		}

		/// <summary>
		/// 画布缩放
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="forceOption"></param>
		/// <returns></returns>
		public static Texture2D AdjustCanvas(Texture2D texture, int width, int height, TextAnchor forceOption)
		{
			Texture2D target = new Texture2D(width, height, texture.format, false);
			var srcPixels = texture.GetPixels();
			var color = srcPixels.Select((p, i) => new Color(p.r, p.g, p.b, p.a)).ToArray();
			switch (forceOption)
			{
				default:
				case TextAnchor.LowerLeft:
					target.SetPixels(0, 0, texture.width, texture.height, color);
					break;
				case TextAnchor.LowerCenter:
					target.SetPixels((width - texture.width) / 2, 0, texture.width, texture.height, color);
					break;
				case TextAnchor.LowerRight:
					target.SetPixels(width - texture.width, 0, texture.width, texture.height, color);
					break;
				case TextAnchor.UpperLeft:
					target.SetPixels(0, height - texture.height, texture.width, texture.height, color);
					break;
				case TextAnchor.UpperCenter:
					target.SetPixels((width - texture.width) / 2, height - texture.height, texture.width, texture.height, color);
					break;
				case TextAnchor.UpperRight:
					target.SetPixels(width - texture.width, height - texture.height, texture.width, texture.height, color);
					break;
				case TextAnchor.MiddleLeft:
					target.SetPixels(0, (height - texture.height) / 2, texture.width, texture.height, color);
					break;
				case TextAnchor.MiddleCenter:
					target.SetPixels((width - texture.width) / 2, (height - texture.height) / 2, texture.width, texture.height, color);
					break;
				case TextAnchor.MiddleRight:
					target.SetPixels(width - texture.width, (height - texture.height) / 2, texture.width, texture.height, color);
					break;
			}
			return target;
		}

		/// <summary>
		/// 图像缩放
		/// </summary>
		/// <param name="source"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static Texture2D AdjustScale(Texture2D source, int width, int height)
		{
			var newWidth = (float)width;
			var newHeight = (float)source.width / source.height * height;

			var result = new Texture2D((int)newWidth, (int)newHeight, TextureFormat.RGB24, false);

			Debug.Log(result.width + ":" + result.height);

			for (int i = 0; i < result.width; ++i)
			{
				for (int j = 0; j < result.height; ++j)
				{
					Color newColor = source.GetPixelBilinear((float)i / result.width, (float)j / result.height);
					result.SetPixel(i, j, newColor);
				}
			}
			result.Apply();
			return result;
		}
	}
}
