using Library.Extensions;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fileUtils
{
    public class FileMerge
    {
        /// <summary>
        /// 文件名字母排序的merge
        /// </summary>
        public class ByName : Down
        {
            public ByName()
            {
                var ex = SystemConsole.GetInputStr("请输入文件后缀(如\"cs,cpp\"：");
                var files = CheckPath("*.*", SelectType.Folder).Where(p => p.EndsWith(ex)).ToArray();
                var outFile = Path.ChangeExtension(InputPath.Trim('.'), ex);
                FileHelper.FileMerge(files.ToArray(), outFile);
            }
        }

        /// <summary>
        /// 文件名数字排序的merge
        /// </summary>
        public class ByNumber : Down
        {
            public ByNumber()
            {
                var files = CheckPath(".ts", SelectType.Folder);
                var outFile = Path.ChangeExtension(InputPath.Trim('.'), ".ts");
                FileHelper.FileMerge(files.ToArray(), outFile);
            }
        }
    }

    public class FileDown
    {
        public class DownFile : Down
        {
            public DownFile()
            {
                GetWebFile(SystemConsole.GetInputStr("请输入文件url:", regex: RegexUrl));
            }
        }

        public abstract class DownM3U8Base : Down
        {
            new protected string RegexUrl = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";

            public IEnumerable<string> GetM3U8(string url)
            {
                return GetWebFileContent(url).Where(p => !p.StartsWith("#"));
            }

            protected void GetValue(string url, string mergePath = null)
            {
                var uri = new Uri(url);

                Console.WriteLine(uri.AbsoluteUri);
                Console.WriteLine(uri.AbsolutePath);

                var fileList = GetM3U8(url).ToList();

                //fileList.AsParallel().ForAll(p =>
                fileList.ForEach((p, i, count) =>
                {
                    //http://v1.benbi123.com/20190409/yzqObCmT/index.m3u8
                    var temp = p.StartsWith("http") ? p : uri.AbsoluteUri.Replace(uri.AbsolutePath, p);
                    SystemConsole.SetProgress(temp, ((float)i / count));
                    GetWebFile(temp);
                });
                SystemConsole.ClearProgress();

                mergePath = string.IsNullOrEmpty(mergePath) ? uri.AbsolutePath : mergePath;

                FileHelper.FileMerge(fileList.Select(p => p.StartsWith("http") ? new Uri(p).AbsolutePath : p).ToArray(),
                    Path.ChangeExtension(mergePath, ".mp4"));

                var dic = FileHelper.String2Dictionary(fileList);
                //dic.Keys.ToList().ForEach(p =>
                //{
                //    Directory.Delete(p, true);
                //});
            }
        }

        public class DownM3U8 : DownM3U8Base
        {
            public DownM3U8()
            {
                var url = SystemConsole.GetInputStr("请输入m3u8文件url:", regex: RegexUrl /*".m3u8"*/);
                GetValue(url);
            }
        }

        public class DownIndexM3U8 : DownM3U8Base
        {
            public DownIndexM3U8()
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
    }
}
