using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace Script
{
    /// <summary>
    /// 拼合图像
    /// </summary>
    public class SpliceImage : BaseClass
    {
        public class ImageInfo
        {
            public float X, Y, Width, Height;
            public string path;
        }

        public SpliceImage()
        {
            bool isIndent = SystemConsole.GetInputStr("json文件是否进行格式化？(true:false)", def: "true").AsBool(false);
            var cache = CheckPath(".png|.jpg|.bmp|.psd|.tga|.tif|.dds", searchOption: SearchOption.TopDirectoryOnly)
                .GroupBy(p => Path.GetExtension(p))
                .ToDictionary(p => p.Key, p => GetInfo(p.Select(q =>
                {
                    using (Image source = Image.FromFile(q))
                    {
                        return new ImageInfo()
                        {
                            Width = source.Width,
                            Height = source.Height,
                            path = q,
                        };
                    }
                }).ToList()));

            //  cache.ForEach(p =>
            //  {
            //      p.Value
            //  });


            //BaseClassE.ForEachPaths(
            //    , re =>
            //    {


            //        var key = re.Replace(InputPath, "");
            //        using (Image source = Image.FromFile(re))
            //        {
            //            cache[key] = new Info
            //            {
            //                width = source.Width,
            //                height = source.Height,
            //                background_image = key,
            //                background_position = ""
            //            };
            //        }
            //    });
            //File.WriteAllText(Path.ChangeExtension(InputPath, ".json"), JsonHelper.ToJson(cache, indentLevel: 2));
        }

        public List<ImageInfo> GetInfo(List<ImageInfo> list)
        {
            return list.OrderByDescending(q => q.Width*q.Height).ThenByDescending(p => p.Width).ToList();
            return list.OrderByDescending(q => q.Width).ThenByDescending(p => p.Height).ToList();
            return list.OrderByDescending(q => q.Height).ThenByDescending(p => p.Width).ToList();
        }
    }
}