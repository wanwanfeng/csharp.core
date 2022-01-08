using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace Script
{
    /// <summary>
    /// 比较文件夹并复制出不同文件
    /// </summary>
    public class CompareFolder : CreateExcelCell
    {
        protected override void CreateExcel()
        {
            var dir1 = SystemConsole.GetInputStr("请拖入选定文件夹:", "您选择的文件夹：");
            var dir2 = SystemConsole.GetInputStr("请拖入选定文件夹:", "您选择的文件夹：");
            var last1 = Directory.GetFiles(dir1, "*.*", SearchOption.AllDirectories).Select(p => p.Replace(dir1, ""));
            var last2 = Directory.GetFiles(dir2, "*.*", SearchOption.AllDirectories).Select(p => p.Replace(dir2, ""));
            last1.Except(last2)
                .Distinct()
                .Select(p => dir1 + p)
                .ToList()
                .ForEachPaths(re =>
                {
                    var newName = re.Replace(dir1, dir1 + "res");
                    DirectoryHelper.CreateDirectory(newName);
                    File.Copy(re, newName, true);
                });

            var root = dir1 + "res";
            RunList(Directory.GetFiles(root, "*.*", SearchOption.AllDirectories), root);
        }
    }

    public class CheckImage : BaseClass
    {
        public CheckImage()
        {
            //var dir1 = SystemConsole.GetInputStr("请拖入源文件夹:", "您选择的文件夹：");
            //var dir2 = SystemConsole.GetInputStr("请拖入目标文件夹:", "您选择的文件夹：");
            //var last1 = Directory.GetFiles(dir1, "*.*", SearchOption.AllDirectories).Select(p => p.Replace(dir1, ""));
            //var last2 = Directory.GetFiles(dir2, "*.*", SearchOption.AllDirectories).Select(p => p.Replace(dir2, ""));

            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //CheckPath(".png|.jpg|.bmp|.psd|.tga|.tif|.dds", searchOption: SearchOption.AllDirectories)
            //    .ForEachPaths((re) =>
            //    {
            //        using (Image source = Image.FromFile(re))
            //        {
            //            using (Image source2 = Image.FromFile(re))
            //            {

            //            }
            //            source.Width
            //        }
            //    });
            //WriteAllLines(dic, InputPath);
        }
    }

    public class FileGzip : BaseClass
    {
        public FileGzip()
        {
            var config = CheckPath(".txt", searchOption: SearchOption.AllDirectories).ToDictionary(p => p.Replace(InputPath, ""), p => new FileInfo(p));
            FileHelper.GZIP.Serialize(config, InputPath + ".bin");
        }
    }
}