using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Library
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
    }
}
