using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Library.Extensions
{
    /// <summary>
    /// AudioClip扩展
    /// </summary>
    public static partial class AudioClipExtensions
    {
        public static byte[] ToArray(this AudioClip clip)
        {
            float[] data = new float[clip.samples];
            clip.GetData(data, 0);
            int rescaleFactor = 32767; //to convert float to Int16
            byte[] outData = new byte[data.Length * 2];
            for (int i = 0; i < data.Length; i++)
            {
                short temshort = (short)(data[i] * rescaleFactor);
                byte[] temdata = BitConverter.GetBytes(temshort);
                outData[i * 2] = temdata[0];
                outData[i * 2 + 1] = temdata[1];
            }
            return outData;
        }
    }
}