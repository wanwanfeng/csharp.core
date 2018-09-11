using System.Collections.Generic;
using System.IO;
using Library.Helper;

namespace Script
{
    /// <summary>
    /// 图片查找形成excel格式并复制出来
    /// </summary>
    public class ImageFind : BaseClass
    {
        private Dictionary<string, string> dic = new Dictionary<string, string>();

        public ImageFind()
        {
            var res = DirectoryHelper.GetFiles(root, ".png|.jpg|.bmp|.psd|.tga|.tif|.dds", SearchOption.AllDirectories);
            if (res.Count == 0) return;
            res.Sort();
            RunList(res);
            WriteAllLines(dic);
        }

        public override void RunListOne(string re)
        {
            dic[re.Replace(root, "")] = GetExcelCell(re);
        }
    }
}