using System.IO;

namespace Library.Extensions
{
    public static class PathExtensions
    {
        public static string GetFilePathWithoutExtension(this FileInfo fileInfo)
        {
            return Path.Combine(fileInfo.DirectoryName, fileInfo.Name);
        }
    }
}