using System.IO;
using Library;
using Library.Extensions;
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

    }
}
