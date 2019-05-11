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
                {"FileMergeByName", new FileMergeByName().Run},
                {"FileMergeByNumber", new FileMergeByNumber().Run},
                {"DownM3U8", new DownM3U8().Run},
                {"DownIndexM3U8", new DownIndexM3U8().Run},
                //{"Test", new Test().Run}
            });
        }

        public class FileDown : Down
        {
            public override void Run()
            {
                GetWebFile(SystemConsole.GetInputStr("请输入文件url:", regex: RegexUrl));
            }
        }

        /// <summary>
        /// 文件名字母排序的merge
        /// </summary>
        public class FileMergeByName : Down
        {
            public override void Run()
            {
                var files = CheckPath(".ts", SelectType.Folder);
                var outFile = Path.ChangeExtension(InputPath.Trim('.'), ".ts");
                FileHelper.FileMerge(files.ToArray(), outFile);
            }
        }

        /// <summary>
        /// 文件名数字排序的merge
        /// </summary>
        public class FileMergeByNumber : Down
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
                var url = SystemConsole.GetInputStr("请输入m3u8文件url:", regex: RegexUrl /*".m3u8"*/);
                GetValue(url);
            }

            protected void GetValue(string url, string mergePath = null)
            {
                var uri = new Uri(url);

                Console.WriteLine(uri.AbsoluteUri);
                Console.WriteLine(uri.AbsolutePath);

                var fileList = GetM3U8(url);

                fileList.AsParallel().ForAll(p =>
                {
                    if (p.StartsWith("http"))
                    {
                        GetWebFile(p);
                    }
                    else
                    {
                        GetWebFile(uri.AbsoluteUri.Replace(uri.AbsolutePath, p));
                    }
                });

                mergePath = string.IsNullOrEmpty(mergePath) ? uri.AbsolutePath : mergePath;

                FileHelper.FileMerge(fileList.Select(p => new Uri(p).AbsolutePath).ToArray(),
                    Path.ChangeExtension(mergePath, ".mp4"));

                var dic = FileHelper.String2Dictionary(fileList);
                //dic.Keys.ToList().ForEach(p =>
                //{
                //    Directory.Delete(p, true);
                //});
            }
        }

        public class DownIndexM3U8 : DownM3U8
        {
            public override void Run()
            {
                var url = SystemConsole.GetInputStr("请输入index.m3u8文件url:", regex: RegexUrl /*"index.m3u8"*/);

                var uri = new Uri(url);

                //获取index.m3u8
                var firstPath = GetM3U8(url)
                    .Select(p =>
                    {
                        if (p.StartsWith("http")) return p;
                        return uri.AbsoluteUri.Replace(uri.AbsolutePath, p);
                    }).First();

                GetValue(firstPath, uri.AbsolutePath);
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
