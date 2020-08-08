using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Extensions
{
    public static partial class PathExtensions
    {
        public static string GetFilePathWithoutExtension(this FileInfo fileInfo)
        {
            return Path.Combine(fileInfo.DirectoryName, fileInfo.Name);
        }
    }
}