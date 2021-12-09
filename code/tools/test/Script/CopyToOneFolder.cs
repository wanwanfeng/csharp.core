using System;
using System.Collections.Generic;
using System.IO;
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
                {"递归搜索文件重命名并复制到同一文件夹下：搜索", CopyToOne},
				{"递归搜索文件重命名并复制到同一文件夹下：还原", RevertCopyToOne},
                {"原结构复制", CopyTo},
            });
        }

        private void CopyToOne()
        {
            CheckPath(".png|.jpg|.bmp|.psd|.tga|.tif|.dds", searchOption: SearchOption.AllDirectories)
                .ForEachPaths(re =>
                {
                    var input = InputPath;
                    string newPath = input + "_merge/" + re.Replace(input, "").TrimStart('/').Replace("/", "..");
                    DirectoryHelper.CreateDirectory(newPath);
                    File.Copy(re, newPath, true);
                    var xxx = Path.ChangeExtension(re, ".plist");
                    if (File.Exists(xxx))
                        File.Copy(xxx, Path.ChangeExtension(newPath, ".plist"), true);
                });
        }

        /// <summary>
        /// 全路径
        /// </summary>
        /// <param name="re"></param>
        private void RevertCopyToOne()
        {
            CheckPath("*.*", searchOption: SearchOption.TopDirectoryOnly)
                .ForEachPaths(re =>
                {
                    string newPath = re.Replace("_merge", "_new").Replace("..", "/");
                    DirectoryHelper.CreateDirectory(newPath);
                    File.Copy(re, newPath, true);
                });
        }

		private void CopyTo()
		{
			CheckPath(".png|.jpg|.bmp|.psd|.tga|.tif|.dds", searchOption: SearchOption.AllDirectories)
				.ForEachPaths(re =>
				{
					var input = InputPath;
					string newPath = input + "_To/" + re.Replace(input, "").TrimStart('/');
					DirectoryHelper.CreateDirectory(newPath);
					File.Copy(re, newPath, true);
					var xxx = Path.ChangeExtension(re, ".plist");
					if (File.Exists(xxx))
						File.Copy(xxx, Path.ChangeExtension(newPath, ".plist"), true);
				});
		}
	}
}