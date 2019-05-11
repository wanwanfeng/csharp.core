using System;
using System.IO;

namespace Library.Extensions
{
    public static class StreamExtensions
    {
        public static void CopyTo(this Stream fileStream, Stream destination, int bufferSize = 4096)
        {
            byte[] bytes = new byte[bufferSize];
            var length = 0;
            while ((length = fileStream.Read(bytes, 0, bytes.Length)) > 0)
            {
                destination.Write(bytes, 0, length);
            }

            //.net 4.0
            //fileStream.CopyTo(destination);
        }

        public static void CopyTo(this Stream fileStream, Stream destination, Action<float> progressAction,
            int bufferSize = 4096)
        {
            byte[] bytes = new byte[bufferSize];
            var length = 0;
            while ((length = fileStream.Read(bytes, 0, bytes.Length)) > 0)
            {
                destination.Write(bytes, 0, length);
                if (fileStream.Position%bytes.Length == 5)
                    progressAction((float) fileStream.Position/fileStream.Length);
            }

            //.net 4.0
            //fileStream.CopyTo(destination);
        }
    }
}