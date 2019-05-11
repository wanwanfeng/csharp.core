using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Compress;
using Library.Extensions;
using Library.Helper;

namespace fileUtils
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SystemConsole.Run(config: new Dictionary<string, Action>()
            {
                {"FileDown", new FileDown().Run},
                {"FileMerge", new FileMerge().Run},
                {"DownM3U8", new DownM3U8().Run},
                //{"Test", new Test().Run}
            });
        }

        public class FileDown : Down
        {
            public override void Run()
            {
                Console.Write("请输入文件url:");
                DownLoad(Console.ReadLine() ?? "");
            }
        }

        public class FileMerge : Down
        {
            public override void Run()
            {
                var files = CheckPath(".ts", SelectType.Folder);
                var outFile = Path.ChangeExtension(InputPath.Trim('.'), ".ts");
                FileHelper.FileMerge(files.ToArray(), outFile);
            }
        }

        public class DownM3U8 : Down
        {
            public override void Run()
            {
                Console.Write("请输入m3u8文件url:");
                var url = Console.ReadLine() ?? "";

                var uri = new Uri(url);
                Console.WriteLine(uri.AbsoluteUri);
                Console.WriteLine(uri.AbsolutePath);

                var path = DownLoad(url);

                var fileList =
                    File.ReadAllLines(path)
                        .Where(p => !p.StartsWith("#"))
                        .ToList();

                var dic = FileHelper.String2Dictionary(fileList);
                //fileList.AsParallel().ForAll(p =>

                fileList.ForEach(p =>
                {
                    DownLoad(uri.AbsoluteUri.Replace(uri.AbsolutePath, p));
                });

                FileHelper.FileMerge(fileList.ToArray(), Path.ChangeExtension(path, ".mp4"));

                dic.Keys.ToList().ForEach(p =>
                {
                    Directory.Delete(p, true);
                });
            }

        }

        public class Test : Down
        {
            public override void Run()
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
}
