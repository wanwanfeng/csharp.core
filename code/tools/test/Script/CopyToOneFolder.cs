using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace Script
{
    /// <summary>
    /// 把图片搜集到一个文件夹方便剔除
    /// </summary>
    public class CopyToOneFolder : BaseClass
    {
        public CopyToOneFolder()
        {
            SystemConsole.Run(config: new Dictionary<string, Action>()
            {
                {"递归搜索文件重命名并复制到同一文件夹下", CopyToOne},
                {"还原", RevertCopyToOne},
            });
        }

        private void CopyToOne()
        {
            CheckPath(".png|.jpg|.bmp|.psd|.tga|.tif|.dds", SelectType.Folder,
                searchOption: SearchOption.AllDirectories)
                .ForEachPaths(re =>
                {
                    var input = InputPath;
                    string newPath = input + "_merge/" + re.Replace(input, "").TrimStart('/').Replace("/", "..");
                    DirectoryHelper.CreateDirectory(newPath);
                    File.Copy(re, newPath, true);
                });
        }

        /// <summary>
        /// 全路径
        /// </summary>
        /// <param name="re"></param>
        private void RevertCopyToOne()
        {
            CheckPath("*.*", SelectType.Folder, searchOption: SearchOption.TopDirectoryOnly)
                .ForEachPaths(re =>
                {
                    string newPath = re.Replace("_merge", "_new").Replace("..", "/");
                    DirectoryHelper.CreateDirectory(newPath);
                    File.Copy(re, newPath, true);
                });
        }
    }
}