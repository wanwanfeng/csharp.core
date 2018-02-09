using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof (TestBundle))]
public class TestBundleEditor : Editor
{
    private TestBundle testBundle;

    private void OnEnable()
    {
        testBundle = (TestBundle) target;
        testBundle.assets = UnityEditor.AssetDatabase.GetAllAssetBundleNames();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);
        this.Horizontal(() =>
        {
            this.SetButton("+", () =>
            {
                testBundle.index = Mathf.Clamp(++testBundle.index, 0, testBundle.assets.Length);
            });
            this.SetButton("-", () =>
            {
                testBundle.index = Mathf.Clamp(--testBundle.index, 0, testBundle.assets.Length);
            });
        });
        this.SetButton("Load", () =>
        {
            if (EditorApplication.isPlaying != true)
                EditorApplication.isPlaying = true;
            testBundle.Init(BundleHelper.GetBundleRoot());
        });
    }
}
