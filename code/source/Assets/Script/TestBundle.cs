using UnityEngine;
using System.Collections;
using System.IO;

public class TestBundle : MonoBehaviour
{
    public string[] assets;
    public int index = 0;

    public AssetBundle assetBundle;

    public void Init(string url)
    {
        if (assetBundle != null)
            assetBundle.Unload(true);

        StopCoroutine("Load");
        StartCoroutine("Load", url);
    }

    private IEnumerator Load(string url)
    {
        var path = assets[index];
        using (WWW www = new WWW(url + path + ".unity3d"))
        {
            yield return www;
            assetBundle = www.assetBundle;
            www.Dispose();

            AudioClip clip = assetBundle.LoadAsset<AudioClip>(GetPath(assetBundle, path));
            var go = new GameObject(clip.name).AddComponent<AudioSource>();
            go.clip = clip;
            go.Play();
        }
    }

    public string GetPath(AssetBundle bundle, string path)
    {
        Debug.Log(string.Join(",", bundle.GetAllAssetNames()));
        foreach (var allAssetName in bundle.GetAllAssetNames())
        {
            if (allAssetName.Contains(path))
            {
                return allAssetName;
            }
        }
        return null;
    }
}
