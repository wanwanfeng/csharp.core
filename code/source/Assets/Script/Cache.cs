using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public interface ICacheBase
{
    string path { get; set; }
}

public abstract class CacheBase<TS, T> : ICacheBase
{
    public TS source { get; set; }
    public T target { get; set; }
    public string path { get; set; }
}

public class CacheAudioClip : CacheBase<AssetBundle, AudioClip>
{

}
public class CacheGameObject : CacheBase<AssetBundle, GameObject>
{

}

public class CacheTexture2D : CacheBase<byte[], Texture2D>
{

}

public class CacheString : CacheBase<byte[], TextAsset>
{

}

public class Cache : MonoBehaviour
{

    public Dictionary<string, ICacheBase> cache { get; set; }

    public void Awake()
    {
        cache = new Dictionary<string, ICacheBase>();
    }

    public UnityEngine.Object Get<T>(string path)
    {
        ICacheBase obj;
        if (cache.TryGetValue(path, out obj))
        {
            if (obj is CacheAudioClip)
            {
                return (obj as CacheAudioClip).target;
            }
            else if (obj is CacheGameObject)
            {
                return (obj as CacheGameObject).target;
            }
            else if (obj is CacheTexture2D)
            {
                return (obj as CacheTexture2D).target;
            }
            else if (obj is CacheString)
            {
                return (obj as CacheString).target;
            }
        }
        else
        {



            //VersionMgr.Instance.Load(path, callAction =>
            //{

            //});
        }
        return null;
    }
}
