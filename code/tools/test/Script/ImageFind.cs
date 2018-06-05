using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;

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
            var eList = ".png|.jpg|.bmp|.psd|.tga|.tif|.dds".AsStringArray('|').ToList();
            var res = Directory.GetFiles(root, "*", SearchOption.AllDirectories)
                //.Where(p => p.Contains("cocos_Data"))
                //.Where(p => p.Contains("Resources"))
                .Where(p => eList.Contains(Path.GetExtension(p)))
                .Select(p => p.Replace("\\", "/"))
                .ToList();
            res.Sort();
            if (res.Count == 0) return;
            RunList(res);
            WriteAllLines(dic);
        }

        public override void RunListOne(string re)
        {
            dic[re.Replace(root, "")] = GetExcelCell(re);
        }
    }
}