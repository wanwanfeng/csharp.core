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

namespace System.IO.Extensions
{
    public static class Path
    {
        public static string GetFullPathWithoutExtension(string path)
        {
            return IO.Path.Combine(IO.Path.GetDirectoryName(path), IO.Path.GetFileNameWithoutExtension(path));
        }
    }


    public static class Directory
    {
        
    }
}