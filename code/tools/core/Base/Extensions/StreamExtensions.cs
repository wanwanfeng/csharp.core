using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Library.Extensions
{
    public static partial class StreamExtensions
    {
        public static void CopyTo(this Stream stream, Stream destination, int bufferSize = 4096)
        {
            byte[] bytes = new byte[bufferSize];
            var length = 0;
            while ((length = stream.Read(bytes, 0, bytes.Length)) > 0)
            {
                destination.Write(bytes, 0, length);
            }

            //.net 4.0
            //stream.CopyTo(destination);
        }

        public static void CopyTo(this Stream stream, Stream destination, Action<float> progressAction,
            int bufferSize = 4096)
        {
            byte[] bytes = new byte[bufferSize];
            var length = 0;
            while ((length = stream.Read(bytes, 0, bytes.Length)) > 0)
            {
                destination.Write(bytes, 0, length);
                if (stream.Position % bytes.Length == 5)
                    progressAction((float) stream.Position / stream.Length);
            }

            //.net 4.0
            //stream.CopyTo(destination);
        }

        public static void Write(this Stream stream, byte[] buffer, int bufferSize = 4096)
        {
            var res = buffer.GroupBuffer(bufferSize);
            foreach (var arr in res)
            {
                stream.Write(arr, 0, arr.Length);
            }
        }
    }


    public static class ArrayExtensions
    {
        #region byte

        public static IEnumerable<byte[]> GroupBuffer(this byte[] source, int bufferSize = 4096)
        {
            byte[] bytes = new byte[bufferSize];
            int offset = 0, length = 0;
            while ((length = source.Read(bytes, offset)) > 0)
            {
                var res = new byte[length];
                Buffer.BlockCopy(bytes, 0, res, 0, length);
                yield return res;
                offset += length;
            }
        }

        private static int Read(this byte[] source, byte[] destination, int offset)
        {
            var length = Math.Min(destination.Length, source.Length - offset);
            if (length <= 0) return 0;
            Buffer.BlockCopy(source, offset, destination, 0, length);
            return length;
        }

        #endregion

        #region byte

        public static IEnumerable<T[]> Group<T>(this T[] source, int bufferSize = 4096)
        {
            T[] bytes = new T[bufferSize];
            int offset = 0, length = 0;
            while ((length = source.Read(bytes, offset)) > 0)
            {
                //var res = new T[length];
                //Array.Copy(bytes, res, length);
                //yield return res;
                yield return bytes.Take(length).ToArray();
                offset += length;
            }
        }

        private static int Read<T>(this T[] source, T[] destination, int offset)
        {
            var length = Math.Min(destination.Length, source.Length - offset);
            if (length <= 0) return 0;
            Array.Copy(source, offset, destination, 0, length);
            return length;
        }

        #endregion
    }
}