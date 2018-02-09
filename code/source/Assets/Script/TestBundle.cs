using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;

public class TestBundle : MonoBehaviour
{
    public string[] assets;
    public int index = 0;

    public AssetBundle assetBundle;

    public void Init(string url)
    {
        Application.runInBackground = true;
        if (assetBundle != null)
            assetBundle.Unload(true);

        StopCoroutine("Load");
        StartCoroutine("Load", url);
    }

    private IEnumerator Load(string url)
    {
        var path = assets[index];
        //using (WWW www = new WWW("file:///" + url + path + ".unity3d"))
        {
            //yield return www;
            //assetBundle = www.assetBundle;
            //www.Dispose();

            assetBundle = AssetBundle.LoadFromFile(url + path + ".unity3d");
            Debug.Log(assetBundle.name);
            Debug.Log(string.Join(",", assetBundle.GetAllAssetNames()));
            var main = assetBundle.GetAllAssetNames().First();
            string ex = Path.GetExtension(main);
            string real = main; //GetPath(assetBundle, path, ref ex);
            if (string.IsNullOrEmpty(ex))
            {
                yield break;
            }

            switch (ex)
            {
                case ".mp3":
                case ".wav":
                {
                    AudioClip clip = assetBundle.LoadAsset<AudioClip>(real);
                    var go = FindObjectOfType<AudioSource>() ?? new GameObject("audioClip").AddComponent<AudioSource>();
                    go.clip = clip;
                    go.loop = true;
                    go.Play();
                }
                    break;
                case "":
                {

                }
                    break;
            }
        }
    }

    public string GetPath(AssetBundle bundle, string path,ref string ex)
    {
        Debug.Log(string.Join(",", bundle.GetAllAssetNames()));
        foreach (var allAssetName in bundle.GetAllAssetNames())
        {
            if (allAssetName.Contains(path))
            {
                ex = Path.GetExtension(allAssetName);
                return allAssetName;
            }
        }
        return null;
    }
}
