using Library;
using Library.Compress;
using Library.Extensions;
using System;
using System.ComponentModel;

namespace fileUtils
{
    internal class Program
    {
        internal enum MyEnum
        {
            [Category("FileMerge"), Description("ByName"), TypeValue(typeof(fileUtils.FileMerge.ByName))] FileMergeByName,
            [Category("FileMerge"), Description("ByNumber"), TypeValue(typeof(fileUtils.FileMerge.ByNumber))] FileMergeByNumber,

            [Category("FileDown"), Description("DownFile"), TypeValue(typeof(FileDown.DownFile))] FileDown,
            [Category("FileDown"), Description("DownM3U8"), TypeValue(typeof(FileDown.DownM3U8))] DownM3U8,
            [Category("FileDown"), Description("DownIndexM3U8"), TypeValue(typeof(FileDown.DownIndexM3U8))] DownIndexM3U8,
        }

        private static void Main(string[] args)
        {
            SystemConsole.Run<MyEnum>();
        }
        internal class FileMerge
        {
            private static void Main(string[] args)
            {
                SystemConsole.Run<MyEnum>(group: "FileMerge");
            }
        }
    }

    public class Test
    {
        private static void Main(string[] args)
        {
            var xxx = Console.ReadLine() ?? "";
            //DecompressUtils.CompressFile(xxx, Path.ChangeExtension(xxx, ".zip"), (p, q) =>
            //{
            //    Console.WriteLine(p, q);
            //});

            DecompressUtils.UnMakeZipFile(xxx);
        }
    }
}