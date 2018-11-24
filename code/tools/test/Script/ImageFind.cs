using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Script
{
    /// <summary>
    /// 图片查找形成excel格式并复制出来
    /// </summary>
    public class ImageFind : BaseClass
    {
        public ImageFind()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            CheckPath(".png|.jpg|.bmp|.psd|.tga|.tif|.dds", searchOption: SearchOption.AllDirectories)
                .OrderBy(p => p).ToList().ForEachPaths((re) =>
                {
                    dic[re.Replace(InputPath, "")] = GetExcelCell(re);
                });
            WriteAllLines(dic, InputPath);
        }
    }
}