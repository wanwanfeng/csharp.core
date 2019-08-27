using System.IO;
using Library;
using Library.Extensions;
using Library.Helper;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.Library
{
    class TextureMenu : BaseMenu
    {
        [MenuItem("Assets/Texture/SpliteSprite")]
        private static void SpliteSprite()
        {
            Object[] go = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
            foreach (var o in go)
            {
                string selectionPath = AssetDatabase.GetAssetPath(o);
                string outPath = Path.ChangeExtension(selectionPath, ".Sprite");
                Directory.CreateDirectory(outPath);
                Object[] all = AssetDatabase.LoadAllAssetsAtPath(selectionPath);

                foreach (var temp in all)
                {
                    Texture2D tx = TextureHelper.GetRawTexture(selectionPath);
                    Sprite sprite = temp as Sprite;
                    if (sprite != null)
                    {
                        Debug.Log("Export Sprite：" + sprite.name);
                        Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
                        tex.SetPixels(sprite.texture.GetPixels((int)sprite.rect.xMin, (int)sprite.rect.yMin, (int)sprite.rect.width, (int)sprite.rect.height));
                        tex.Apply();
                        File.WriteAllBytes(outPath + "/" + sprite.name + ".png", tex.EncodeToPNG());
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        //[MenuItem("Assets/Texture/SpriteJoin")]
        //private static void TextureJoin()
        //{
        //    var go = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets).Cast<Texture2D>().OrderBy(p => p.name).ToList();
        //    var tex = new Texture2D(go.Sum(p => p.width), go.Max(p => p.height), go.First().format, false);
        //    var cursum = 0;
        //    var maxHeight = go.Max(p => p.height);
        //    foreach (var texture in go)
        //    {
        //        tex.SetPixels(cursum, maxHeight - texture.height, texture.width, texture.height,
        //            texture.GetPixels(0, 0, texture.width, texture.height));
        //        cursum += texture.width;
        //        tex.Apply();
        //    }
        //    string path = EditorUtility.SaveFilePanel("", "", "texture", "png");
        //    File.WriteAllBytes(path, tex.EncodeToPNG());
        //    AssetDatabase.Refresh();
        //}


        [MenuItem("Assets/Texture/SplitAlpha")]
        private static void SplitAlpha()
        {
            Object[] go = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
            foreach (var o in go)
            {
                string selectionPath = AssetDatabase.GetAssetPath(o);
                string outPath = Path.ChangeExtension(selectionPath, ".alpha.png");
                Texture2D tx = TextureHelper.GetRawTexture(selectionPath);
                if (tx != null)
                    tx.SplitAlpha(selectionPath, outPath);
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Texture/SaveAlpha")]
        private static void SaveAlpha()
        {
            Object[] go = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
            foreach (var o in go)
            {
                string selectionPath = AssetDatabase.GetAssetPath(o);
                string outPath = Path.ChangeExtension(selectionPath, ".alpha.png");

                Texture2D tx = TextureHelper.GetRawTexture(selectionPath);
                if (tx != null)
                    tx.SaveAlpha(outPath);
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Texture/ForceSquared")]
        //[MenuItem("Assets/Texture/ForceSquared/UpperLeft %UpperLeft")]
        //[MenuItem("Assets/Texture/ForceSquared/UpperCenter")]
        //[MenuItem("Assets/Texture/ForceSquared/UpperRight")]
        //[MenuItem("Assets/Texture/ForceSquared/MiddleLeft")]
        //[MenuItem("Assets/Texture/ForceSquared/MiddleCenter")]
        //[MenuItem("Assets/Texture/ForceSquared/MiddleRight")]
        //[MenuItem("Assets/Texture/ForceSquared/LowerLeft")]
        //[MenuItem("Assets/Texture/ForceSquared/LowerCenter")]
        //[MenuItem("Assets/Texture/ForceSquared/LowerRight")]
        private static void ForceSquared()
        {
            Object[] go = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
            foreach (var o in go)
            {
                string selectionPath = AssetDatabase.GetAssetPath(o);
                string outPath = Path.ChangeExtension(selectionPath, ".fs.png");

                Texture2D tx = TextureHelper.GetRawTexture(selectionPath);
                if (tx != null)
                    tx.ForceSquared(outPath, TextAnchor.UpperLeft);
            }
            AssetDatabase.Refresh();
        }
    }
}
