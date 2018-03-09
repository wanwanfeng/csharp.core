using Library.LitJson;
using LitJson;
using System;
using System.Linq;
using Library.Extensions;
using UnityEngine;

public class LitJsonHelper : Library.LitJson.LitJsonHelper
{
    static LitJsonHelper()
    {
        JsonMapper.RegisterImporter<string, Vector2>(input =>
        {
            var array = input.AsStringArray(',');
            return new Vector2(array.Length > 0 ? array[0].AsFloat() : 0, array.Length > 1 ? array[1].AsFloat() : 0);
        });
        JsonMapper.RegisterImporter<string, Vector3>(input =>
        {
            var array = input.AsStringArray(',');
            return new Vector3(array.Length > 0 ? array[0].AsFloat() : 0, array.Length > 1 ? array[1].AsFloat() : 0,
                array.Length > 2 ? array[2].AsFloat() : 0);
        });

        JsonMapper.RegisterExporter<Vector2>(delegate(Vector2 v, JsonWriter w)
        {
            w.WriteObjectStart();
            w.WritePropertyName("x");
            w.Write(v.x);
            w.WritePropertyName("y");
            w.Write(v.y);
            w.WriteObjectEnd();
        });
        JsonMapper.RegisterExporter<Vector3>(delegate(Vector3 v, JsonWriter w)
        {
            w.WriteObjectStart();
            w.WritePropertyName("x");
            w.Write(v.x);
            w.WritePropertyName("y");
            w.Write(v.y);
            w.WritePropertyName("z");
            w.Write(v.z);
            w.WriteObjectEnd();
        });
    }
}
