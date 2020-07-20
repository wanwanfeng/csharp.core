using System.IO;
using System.Linq;
using Library;
using Library.Extensions;
using Library.Helper;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.Library
{
    class TextureMenu : BaseMenu
    {
        [MenuItem("Assets/Texture/SpliteSprite", true)]
        [MenuItem("Assets/Texture/SplitAlpha", true)]
        [MenuItem("Assets/Texture/SaveAlpha", true)]
        [MenuItem("Assets/Texture/ForceSquared", true)]
        [MenuItem("Assets/Texture/SpriteJoin(Max1024)", true)]
        [MenuItem("Assets/Texture/SpriteJoin(Max2048)", true)]
        private static bool VTexture2D()
        {
            return Selection.GetFiltered<Texture2D>(SelectionMode.Assets).Length != 0;
        }

        [MenuItem("Assets/Texture/SpliteSprite", false)]
        private static void SpliteSprite()
        {
            Texture2D[] go = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
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

        [MenuItem("Assets/Texture/SpriteJoin(Max1024)", false)]
        static void CerateGrid1024()
        {
            CerateGrid(1024);
        }

        [MenuItem("Assets/Texture/SpriteJoin(Max2048)", false)]
        static void CerateGrid2048()
        {
            CerateGrid(2048);
        }

        static void CerateGrid(int max = 1024)
        {
            var paths = Selection.GetFiltered<Texture2D>(SelectionMode.TopLevel).Select(AssetDatabase.GetAssetPath).OrderBy(p => p).ToArray();
            if (paths.Length == 0) return;

            string[] outPath = null;

            Texture2D target = new Texture2D(max, max, TextureFormat.RGBA32, false);
            int x = 0, y = 0;
            for (int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
               
                outPath = outPath == null ? path.Split('/') : path.Split('/').Intersect(outPath).ToArray();
                Texture2D texture = TextureHelper.GetRawTexture(path);
                if (x > target.width - texture.width)
                {
                    x = 0;
                    y += texture.height;
                }

                try
                {
                    Debug.LogFormat("{0},{1},{2},{3},{4}", path, x, y, texture.width, texture.height);
                    target.SetPixels(x, y, texture.width, texture.height, texture.GetPixels());
                    x += texture.width;
                }
                catch (System.Exception e)
                {
                    Debug.LogFormat("{0},{1},{2},{3},{4}", path, x, y, texture.width, texture.height);
                    Debug.LogError(e.Message);
                    continue;
                }
            }

            target.Apply();
            File.WriteAllBytes(string.Join("/", outPath) + ".png", target.EncodeToPNG());

            AssetDatabase.Refresh();
        }


        [MenuItem("Assets/Texture/SplitAlpha", false)]
        private static void SplitAlpha()
        {
            Texture2D[] go = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
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

        [MenuItem("Assets/Texture/SaveAlpha", false)]
        private static void SaveAlpha()
        {
            Texture2D[] go = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
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

        [MenuItem("Assets/Texture/ForceSquared", false)]
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
            Texture2D[] go = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
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
