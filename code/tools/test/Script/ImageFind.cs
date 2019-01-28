using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Core;
using Library.Extensions;
using Library.Helper;

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
                .ForEachPaths((re) =>
                {
                    dic[re.Replace(InputPath, "")] = GetExcelCell(re);
                });
            WriteAllLines(dic, InputPath);
        }
    }


    /// <summary>
    /// 渐进式jpeg(progressive jpeg)图片
    /// </summary>
    public class ImageProgressiveJpeg : BaseClass
    {
        public ImageProgressiveJpeg()
        {
            CheckPath(".jpg", searchOption: SearchOption.AllDirectories)
                .ForEachPaths(re =>
                {
                    if (new FileInfo(re).Length < 10 * 1024) return;
                    using (Image source = Image.FromFile(re))
                    {
                        ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().First(c => c.MimeType == "image/jpeg");

                        EncoderParameters parameters = new EncoderParameters(3);
                        parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                        parameters.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.ScanMethod, (int)EncoderValue.ScanMethodInterlaced);
                        parameters.Param[2] = new EncoderParameter(System.Drawing.Imaging.Encoder.RenderMethod, (int)EncoderValue.RenderProgressive);

                        source.Save(re + ".temp", codec, parameters);
                    }

                    File.Delete(re);
                    File.Move(re + ".temp", re);
                });
        }
    }

    /// <summary>
    /// 渐进式jpeg(progressive jpeg)图片
    /// </summary>
    public class ImageCmdProgressiveJpeg : BaseClass
    {
        public ImageCmdProgressiveJpeg()
        {
            CheckPath(".jpg", searchOption: SearchOption.AllDirectories)
                .ForEachPaths(re =>
                {
                    if (new FileInfo(re).Length < 10*1024) return;

                    CmdReadLine(string.Format("jpegtran -copy none -progressive {0} {0}.temp", re));
                    File.Delete(re);
                    File.Move(re + ".temp", re);
                });
        }
    }


    /// <summary>
    /// 交错png图片
    /// </summary>
    public class ImageProgressivePng : BaseClass
    {
        public ImageProgressivePng()
        {
            CheckPath(".png", searchOption: SearchOption.AllDirectories)
                .ForEachPaths(re =>
                {
                    if (new FileInfo(re).Length < 10 * 1024) return;
                    using (Image source = Image.FromFile(re))
                    {
                        ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().First(c => c.MimeType == "image/jpeg");

                        EncoderParameters parameters = new EncoderParameters(3);
                        parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                        parameters.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.ScanMethod, (int)EncoderValue.ScanMethodInterlaced);
                        parameters.Param[2] = new EncoderParameter(System.Drawing.Imaging.Encoder.RenderMethod, (int)EncoderValue.RenderProgressive);

                        source.Save(re + ".temp", codec, parameters);
                    }

                    File.Delete(re);
                    File.Move(re + ".temp", re);
                });
        }
    }

    /// <summary>
    /// 图片转为雪碧图
    /// </summary>
    public class ImageConvertToSprite : BaseClass
    {
        private class Info
        {
            public float width, height;
            public string background_image, background_repeat, background_position;
        }

        public ImageConvertToSprite()
        {
            bool isIndent = SystemConsole.GetInputStr("json文件是否进行格式化？(true:false)", def: "true").AsBool(false);
            Dictionary<string, Info> cache = new Dictionary<string, Info>();
            BaseClassE.ForEachPaths(
                CheckPath(".png|.jpg|.bmp|.psd|.tga|.tif|.dds", searchOption: SearchOption.AllDirectories), re =>
                {
                    var key = re.Replace(InputPath, "");
                    using (Image source = Image.FromFile(re))
                    {
                        cache[key] = new Info
                        {
                            width = source.Width,
                            height = source.Height,
                            background_image = key,
                            background_position = ""
                        };
                    }
                });
            File.WriteAllText(Path.ChangeExtension(InputPath, ".json"), JsonHelper.ToJson(cache, indentLevel: 2));
        }
    }

    /// <summary>
    /// 图片转为Base64
    /// </summary>
    public class ImageConvertToBase64 : BaseClass
    {
        public ImageConvertToBase64()
        {
            bool createFile = SystemConsole.GetInputStr("是否生成碎文件？(true:false)", def: "false").AsBool(true);
            bool isIndent = SystemConsole.GetInputStr("json文件是否进行格式化？(true:false)", def: "true").AsBool(false);
            Dictionary<string, string> cache = new Dictionary<string, string>();
            BaseClassE.ForEachPaths(
                CheckPath(".png|.jpg|.bmp|.psd|.tga|.tif|.dds", searchOption: SearchOption.AllDirectories), re =>
                {
                    var content = Convert.ToBase64String(File.ReadAllBytes(re));
                    cache[re.Replace(InputPath, "")] = content;
                    if (createFile)
                    {
                        re = Path.ChangeExtension(re, "txt");
                        File.WriteAllText(re, content);
                    }
                });
            File.WriteAllText(Path.ChangeExtension(InputPath, ".json"), JsonHelper.ToJson(cache, indentLevel: 2));
        }
    }

    /// <summary>
    /// Base64转为图片
    /// </summary>
    public class Base64ConvertToImage : BaseClass
    {
        public Base64ConvertToImage()
        {
            var tag = "data:image/png;base64,";
            BaseClassE.ForEachPaths(
                CheckPath(".png|.jpg|.bmp|.psd|.tga|.tif|.dds", searchOption: SearchOption.AllDirectories), re =>
                {
                    var content = File.ReadAllText(re);
                    var index = content.IndexOf(tag, StringComparison.Ordinal);
                    if (index >= 0)
                        content = content.Substring(index + tag.Length);
                    var bts = Convert.FromBase64String(content);
                    File.WriteAllBytes(re, bts);
                });
        }
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    public class DeleteFiles : BaseClass
    {
        public DeleteFiles()
        {

            var path = SystemConsole.GetInputStr("请拖入选定文件:", "您选择的文件：", def: "new.txt");
            if (!File.Exists(path)) return;

            var files = File.ReadAllLines(path);

            CheckPath("*.*", searchOption: SearchOption.AllDirectories)
                .ForEachPaths(re =>
                {
                    var xx = re.Replace(InputPath, "").TrimStart('/');
                    if (files.Contains(xx)) return;
                    File.Delete(re);
                });
        }
    }
}