using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UGUIMenu
{
    [MenuItem("CONTEXT/RectTransform/Reset Resolution")]
    public static void OnResolution()
    {
        var active = Selection.activeGameObject;
        var trans = active.GetComponent<RectTransform>();
        var cs = active.GetComponentInParent<CanvasScaler>();
        if (cs == null) return;
        trans.sizeDelta = cs.referenceResolution;
    }

    #region UGUI

    [MenuItem("GameObject/UI/Image")]
    static void CreatImage()
    {
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject go = new GameObject("Image", typeof(Image));
                go.GetComponent<Image>().raycastTarget = false;
                go.transform.SetParent(Selection.activeTransform);
            }
        }
    }

    [MenuItem("GameObject/UI/Text")]
    static void CreatText()
    {
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                var go = new GameObject("Text", typeof(Text));
                go.GetComponent<Text>().raycastTarget = false;
                go.transform.SetParent(Selection.activeTransform);
            }
        }
    }

    /// <summary>
    /// SetRaycastTarget
    /// 默认设为false
    /// </summary>
    [MenuItem("Assets/UGUI/SetRaycastTarget", false, 10)]
    public static void SetRaycastTarget()
    {
        var objects = new List<GameObject>(Selection.gameObjects);
        if (objects.Count == 0)
            return;
        for (var j = 0; j < objects.Count; j++)
        {
            var obj = objects[j];
            var filePath = AssetDatabase.GetAssetPath(obj);
            EditorUtility.DisplayProgressBar("操作中,请稍后...", filePath, (float) (j + 1)/objects.Count);
            if (!filePath.Contains("."))
                continue;
            GameObject go = obj;
            var components = go.transform.GetComponentsInChildren<Graphic>(true);
            foreach (var component in components)
            {
                component.raycastTarget = true;
                if (component.GetComponent<Button>()) continue;
                if (component.GetComponent<InputField>()) continue;
                if (component.GetComponent<Scrollbar>()) continue;
                if (component.GetComponent<Slider>()) continue;
                if (component.GetComponent<Dropdown>()) continue;
                if (component.name == "mask") continue;
                if (component.name == "drag") continue;
                component.raycastTarget = false;
            }
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    #endregion

    #region UIAtlasExport(裁剪九宫格)

    [MenuItem("Assets/UGUI/SpriteExport(裁剪九宫格)")]
    private static void UIAtlasExport()
    {
        var gos = Selection.GetFiltered(typeof (Texture2D), SelectionMode.Assets);
        if (gos.Length == 0)
            return;

        try
        {
            foreach (var item in gos)
            {
                string selectionPath = AssetDatabase.GetAssetPath(item);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(selectionPath);

                Texture2D texture2D = sprite.texture;
                if (texture2D == null) continue;

                //ClearSetting(texture2D);

                Debug.Log("Export Sprite：" + sprite.name);
                Texture2D tex = null;

                var borderTop = (int) sprite.border.w;
                var borderBottom = (int) sprite.border.y;
                var borderLeft = (int) sprite.border.x;
                var borderRight = (int) sprite.border.z;

                Debug.Log(string.Format("{0},{1},{2},{3}", borderLeft, borderRight, borderTop, borderBottom));

                if (borderTop > 0 && borderBottom > 0 && borderLeft > 0 && borderRight > 0)
                {
                    tex = GongJiu(texture2D, sprite);
                }
                else if (borderTop > 0 && borderBottom > 0 && borderLeft == 0 && borderRight == 0)
                {
                    tex = GongSanShu(texture2D, sprite);
                }
                else if (borderTop == 0 && borderBottom == 0 && borderLeft > 0 && borderRight > 0)
                {
                    tex = GongSanHeng(texture2D, sprite);
                }
                else
                {
                    return;
                }
                if (tex != null)
                {
                    File.WriteAllBytes(selectionPath, tex.EncodeToPNG());
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    /// 九宫格
    /// </summary>
    /// <param name="texture2D"></param>
    /// <param name="sprite"></param>
    private static Texture2D GongJiu(Texture2D texture2D, Sprite sprite)
    {
        var borderTop = (int)sprite.border.w;
        var borderBottom = (int)sprite.border.y;
        var borderLeft = (int)sprite.border.x;
        var borderRight = (int)sprite.border.z;

        if (borderTop == 0 || borderBottom == 0 || borderLeft == 0 || borderRight == 0)
            return null;

        var x = (int)sprite.rect.x;
        var y = (int)sprite.rect.y;
        var height = (int)sprite.rect.height;
        var width = (int)sprite.rect.width;

        Color[] lb = texture2D.GetPixels(
            x,
            texture2D.height - y - height,
            borderLeft,
            borderBottom);

        Color[] rb = texture2D.GetPixels(
            x + width - borderRight,
            texture2D.height - y - height,
            borderRight,
            borderBottom);

        Color[] lt = texture2D.GetPixels(
            x,
            texture2D.height - y - borderTop,
            borderLeft,
            borderTop);

        Color[] rt = texture2D.GetPixels(
            x + width - borderRight,
            texture2D.height - y - borderTop,
            borderRight,
            borderTop);

        var tex = new Texture2D(borderLeft + borderRight,
            borderTop + borderBottom, texture2D.format, false);
        tex.SetPixels(
            0,
            0,
            borderLeft,
            borderBottom, lb);
        tex.SetPixels(
            borderLeft,
            0,
            borderRight,
            borderBottom, rb);
        tex.SetPixels(
            0,
            borderBottom,
            borderLeft,
            borderTop, lt);
        tex.SetPixels(
            borderLeft,
            borderBottom,
            borderRight,
            borderTop, rt);

        tex.Apply();
        return tex;
    }

    /// <summary>
    /// 竖三宫
    /// </summary>
    /// <param name="texture2D"></param>
    /// <param name="sprite"></param>
    private static Texture2D GongSanShu(Texture2D texture2D, Sprite sprite)
    {
        var borderTop = (int) sprite.border.w;
        var borderBottom = (int) sprite.border.y;
        var borderLeft = (int) sprite.border.x;
        var borderRight = (int) sprite.border.z;

        if (borderBottom == 0 && borderTop == 0)
            return null;
        if (borderLeft != 0 || borderRight != 0)
            return null;

        var x = (int) sprite.rect.x;
        var y = (int) sprite.rect.y;
        var height = (int) sprite.rect.height;
        var width = (int) sprite.rect.width;

        Color[] b = texture2D.GetPixels(x, texture2D.height - y - height, width, borderBottom);
        Color[] t = texture2D.GetPixels(x, texture2D.height - y - borderTop, width, borderTop);
        
        var tex = new Texture2D(width, borderTop + borderBottom, texture2D.format, false);
        tex.SetPixels(0, 0, width, borderBottom, b);
        tex.SetPixels(0, borderBottom, width, borderTop, t);
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// 横三宫
    /// </summary>
    /// <param name="texture2D"></param>
    /// <param name="sprite"></param>
    private static Texture2D GongSanHeng(Texture2D texture2D, Sprite sprite)
    {
        var borderTop = (int) sprite.border.w;
        var borderBottom = (int) sprite.border.y;
        var borderLeft = (int) sprite.border.x;
        var borderRight = (int) sprite.border.z;

        if (borderLeft == 0 && borderRight == 0)
            return null;
        if (borderBottom != 0 || borderTop != 0)
            return null;

        var x = (int) sprite.rect.x;
        var y = (int) sprite.rect.y;
        var height = (int) sprite.rect.height;
        var width = (int) sprite.rect.width;

        Color[] l = texture2D.GetPixels(x, texture2D.height - y - height, borderLeft, height);
        Color[] r = texture2D.GetPixels(x + width - borderRight, texture2D.height - y - height, borderRight, height);

        var tex = new Texture2D(borderLeft + borderRight, height, texture2D.format, false);
        tex.SetPixels(0, 0, borderLeft, height, l);
        tex.SetPixels(borderLeft, 0, borderRight, height, r);
        tex.Apply();
        return tex;
    }

    #endregion
}
