using Library.Compress;
using UnityEngine;
using UnityEditor;

public class Cmd : Editor
{
    [MenuItem("Tools/MakeZipFile")]
    public static void MakeZipFile()
    {
        Debug.LogError(DecompressUtils.MakeZipFile("E:\\Git\\yg\\trunk\\潘之琳"));
    }

    [MenuItem("Tools/UnMakeZipFile")]
    public static void UnMakeZipFile()
    {
        Debug.LogError(DecompressUtils.UnMakeZipFile("E:\\Git\\yg\\trunk\\潘之琳3.zip"));
    }
}
