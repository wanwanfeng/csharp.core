﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Library.Extensions;
using LitJson;
using UnityEngine;

public static class LitJsonHelper
{
    static LitJsonHelper()
    {
        JsonMapper.RegisterImporter<double, float>(input => (float) input);
        JsonMapper.RegisterImporter<string, Vector3>(
            input =>
                input.Split(',')
                    .Select(p => new Vector3(p.Length < 1 ? 0 : p[0], p.Length < 2 ? 0 : p[1], p.Length < 3 ? 0 : p[2]))
                    .FirstOrDefault());
        JsonMapper.RegisterImporter<string, Vector2>(
            input =>
                input.Split(',')
                    .Select(p => new Vector2(p.Length < 1 ? 0 : p[0], p.Length < 2 ? 0 : p[1]))
                    .FirstOrDefault());

        JsonMapper.RegisterExporter<Vector2>((v, w) =>
        {
            w.WriteObjectStart();
            w.WritePropertyName("x");
            w.Write(v.x);
            w.WritePropertyName("y");
            w.Write(v.y);
            w.WriteObjectEnd();
        });
        JsonMapper.RegisterExporter<Vector3>((v, w) =>
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
        JsonMapper.RegisterExporter<float>((v, w) =>
        {
            w.Write(v);
        });
    }

    public static T ToObject<T>(string res)
    {
        return JsonMapper.ToObject<T>(res);
    }

    public static JsonData ToObject(string res)
    {
        return JsonMapper.ToObject(res);
    }

    public static string ToJson<T>(T t)
    {
        return JsonMapper.ToJson(t);
    }
}