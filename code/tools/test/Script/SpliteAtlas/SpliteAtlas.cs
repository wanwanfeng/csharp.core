using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Library.Extensions;
using Library.Helper;

namespace Script
{
    public class SpliteAtlas : BaseClass
    {
        /// <summary>
        /// 筛选过滤已存在图片
        /// </summary>
        public SpliteAtlas()
        {
            SystemConsole.Run(config: new Dictionary<string, Action>()
            {
                {
                    "图集拆解", () =>
                    {
                        GetPaths().ForEachPaths(re =>
                        {
                            PList plist = new PList();
                            plist.Load(re);
                            TextureInfo textureInfo = new TextureInfo(plist, re);
                            if (!textureInfo.voild) return;
                            var imagePath = Path.GetDirectoryName(re) + "/" + textureInfo.name;

                            if (File.Exists(imagePath))
                                HaveImageAndRead(imagePath, textureInfo);
                        });
                    }
                },
                {
                    "图集合并", () =>
                    {
                        GetPaths().ForEachPaths(re =>
                        {
                            PList plist = new PList();
                            plist.Load(re);
                            TextureInfo textureInfo = new TextureInfo(plist, re);
                            if (!textureInfo.voild) return;
                            var imagePath = Path.GetDirectoryName(re) + "/" + textureInfo.name;

                            if (File.Exists(imagePath))
                                HaveImageAndWrite(imagePath, textureInfo);
                        });
                    }
                },
                {
                    "文件夹删除", () =>
                    {
                        GetPaths().ForEachPaths(re =>
                        {
                            PList plist = new PList();
                            plist.Load(re);
                            TextureInfo textureInfo = new TextureInfo(plist, re);
                            if (!textureInfo.voild) return;
                            var imagePath = Path.GetDirectoryName(re) + "/" + textureInfo.name;

                            string dir = Path.GetDirectoryName(imagePath) + "/" +
                                         Path.GetFileNameWithoutExtension(imagePath);
                            if (Directory.Exists(dir))
                                Directory.Delete(dir, true);
                        });
                    }
                },
            });
        }

        private List<string> GetPaths()
        {
            var path = SystemConsole.GetInputStr("请拖入选定（文件夹或文件）:");

            List<string> res = new List<string>();
            if (Directory.Exists(path))
            {
                if (File.Exists(path + ".plist"))
                    res.Add(path + ".plist");
                else
                    res.AddRange(Directory.GetFiles(path, "*.plist", SearchOption.AllDirectories));
            }
            else
            {
                if (File.Exists(path) && Path.GetExtension(path) == ".plist")
                    res.Add(path);
            }
            return res;
        }

        private static void HaveImageAndRead(string re, TextureInfo textureInfo)
        {
            using (FileStream fs = new FileStream(re, FileMode.Open, FileAccess.Read))
            {
                Image image = Image.FromStream(fs);
                foreach (var splite in textureInfo.list)
                {
                    var newName = Path.Combine(Path.GetDirectoryName(re), Path.GetFileNameWithoutExtension(textureInfo.name), splite.name);
                    DirectoryHelper.CreateDirectory(newName);

                    var bitmap = new Bitmap(splite.width, splite.height, image.PixelFormat);
                    bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                    Graphics graphic = Graphics.FromImage(bitmap);
                    //graphic.Clear(Color.FromArgb(0, 255, 0, 0));
                    graphic.DrawImage(image, 0, 0, new Rectangle(splite.x, splite.y, splite.width, splite.height), GraphicsUnit.Pixel);
                    //bitmap.MakeTransparent(Color.Transparent);
                    bitmap.Save(newName);

                    /*Image saveImage = Image.FromHbitmap(bitmap.GetHbitmap());
                    //saveImage.Save(newName, image.RawFormat);
                    saveImage.Save(newName, ImageFormat.Png);

                        saveImage.Dispose();*/
                    graphic.Dispose();
                    bitmap.Dispose();
                }
                GC.Collect();
            }
        }

        private static void HaveImageAndWrite(string re, TextureInfo textureInfo)
        {
            using (var bitmap = new Bitmap(textureInfo.width, textureInfo.height, PixelFormat.Format32bppArgb))
            {
                bitmap.SetResolution(textureInfo.resolution.X, textureInfo.resolution.Y);
                using (Graphics graphic = Graphics.FromImage(bitmap))
                {
                    foreach (var splite in textureInfo.list)
                    {
                        var newName = Path.Combine(Path.GetDirectoryName(re), Path.GetFileNameWithoutExtension(textureInfo.name), splite.name);
                        if (File.Exists(newName))
                        {
                            using (FileStream fsT = new FileStream(newName, FileMode.Open, FileAccess.Read))
                            {
                                Image temp = Image.FromStream(fsT);
                                graphic.DrawImage(temp, splite.x, splite.y, new RectangleF(0, 0, splite.width, splite.height), GraphicsUnit.Pixel);
                            }
                        }
                        else
                        {
                            //特殊要求，绘制透明图片
                            using (var bm = new Bitmap(splite.width, splite.height, PixelFormat.Format32bppArgb))
                            {
                                bm.SetResolution(textureInfo.resolution.X, textureInfo.resolution.Y);
                                graphic.DrawImage(bm, splite.x, splite.y, new RectangleF(0, 0, splite.width, splite.height), GraphicsUnit.Pixel);
                            }
                        }
                    }
                }
                bitmap.Save(re, ImageFormat.Png);
            }
            GC.Collect();
        }
    }

    /// <summary>
    /// <!-- Created with Adobe Animate version 15.2.1.95 -->
    /// </summary>
    public class SpliteAdobeAnimateAtlas : BaseClass
    {
        public struct Sprite
        {
            public string name;
            public int x, y, width, height;
            public float pivotX, pivotY;
        }

        public SpliteAdobeAnimateAtlas()
        {
            SystemConsole.Run(config: new Dictionary<string, Action>()
            {
                {
                    "图集拆解", () =>
                    {
                        GetPaths().ForEachPaths(re =>
                        {
                            XDocument xDoc = XDocument.Load(re);
                            var list = xDoc.Element("TextureAtlas").Elements("SubTexture").Select(p=>{
                                return new Sprite{
                                    name=(string)p.Attribute("name")+".png",
                                    x=(int)p.Attribute("x"),
                                    y=(int)p.Attribute("y"),
                                    width=(int)p.Attribute("width"),
                                    height=(int)p.Attribute("height"),
                                    pivotX=p.Attribute("pivotX")!=null?(float)p.Attribute("pivotX"):0,
                                    pivotY=p.Attribute("pivotY")!=null?(float)p.Attribute("pivotY"):0,
                                };
                            }).OrderBy(p=>p.name).ToList();

                            var imagePath = re.Replace("Xml.bin",".png");
                            if (File.Exists(imagePath))
                                HaveImageAndRead(imagePath, imagePath,list);
                        });
                    }
                },
                //{
                //    "图集合并", () =>
                //    {
                //        GetPaths().ForEachPaths(re =>
                //        {
                //            //PList plist = new PList();
                //            //plist.Load(re);
                //            //TextureInfo textureInfo = new TextureInfo(plist, re);
                //            //if (!textureInfo.voild) return;
                //            //var imagePath = Path.GetDirectoryName(re) + "/" + textureInfo.name;

                //            //if (File.Exists(imagePath))
                //            //    HaveImageAndWrite(imagePath, textureInfo);
                //        });
                //    }
                //},
                //{
                //    "文件夹删除", () =>
                //    {
                //        GetPaths().ForEachPaths(re =>
                //        {
                //            PList plist = new PList();
                //            plist.Load(re);
                //            TextureInfo textureInfo = new TextureInfo(plist, re);
                //            if (!textureInfo.voild) return;
                //            var imagePath = Path.GetDirectoryName(re) + "/" + textureInfo.name;

                //            string dir = Path.GetDirectoryName(imagePath) + "/" +
                //                         Path.GetFileNameWithoutExtension(imagePath);
                //            if (Directory.Exists(dir))
                //                Directory.Delete(dir, true);
                //        });
                //    }
                //},
            });
        }


        private List<string> GetPaths()
        {
            var path = SystemConsole.GetInputStr("请拖入选定（文件夹或文件）:");

            List<string> res = new List<string>();
            if (Directory.Exists(path))
            {
                if (File.Exists(path + ".bin"))
                    res.Add(path + ".bin");
                else
                    res.AddRange(Directory.GetFiles(path, "*.bin", SearchOption.AllDirectories));
            }
            else
            {
                if (File.Exists(path) && Path.GetExtension(path) == ".bin")
                    res.Add(path);
            }
            return res;
        }

        private static void HaveImageAndRead(string re, string imagePath, List<Sprite> list)
        {
            using (FileStream fs = new FileStream(re, FileMode.Open, FileAccess.Read))
            {
                Image image = Image.FromStream(fs);
                foreach (var splite in list)
                {
                    var newName = Path.Combine(Path.GetDirectoryName(re), Path.GetFileNameWithoutExtension(imagePath), splite.name);
                    DirectoryHelper.CreateDirectory(newName);
                    using (var bitmap = new Bitmap(splite.width, splite.height, image.PixelFormat))
                    {
                        bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                        using (Graphics graphic = Graphics.FromImage(bitmap))
                        {
                            graphic.DrawImage(image, 0, 0, new Rectangle(splite.x, splite.y, splite.width, splite.height), GraphicsUnit.Pixel);
                        }
                        bitmap.Save(newName);
                    }
                }
                GC.Collect();
            }
        }

        private static void HaveImageAndWrite(string re, TextureInfo textureInfo)
        {
            using (var bitmap = new Bitmap(textureInfo.width, textureInfo.height, PixelFormat.Format32bppArgb))
            {
                bitmap.SetResolution(textureInfo.resolution.X, textureInfo.resolution.Y);
                using (Graphics graphic = Graphics.FromImage(bitmap))
                {
                    foreach (var splite in textureInfo.list)
                    {
                        var newName = Path.Combine(Path.GetDirectoryName(re), Path.GetFileNameWithoutExtension(textureInfo.name), splite.name);
                        if (File.Exists(newName))
                        {
                            using (FileStream fsT = new FileStream(newName, FileMode.Open, FileAccess.Read))
                            {
                                Image temp = Image.FromStream(fsT);
                                graphic.DrawImage(temp, splite.x, splite.y, new RectangleF(0, 0, splite.width, splite.height), GraphicsUnit.Pixel);
                            }
                        }
                        else
                        {
                            //特殊要求，绘制透明图片
                            using (var bm = new Bitmap(splite.width, splite.height, PixelFormat.Format32bppArgb))
                            {
                                bm.SetResolution(textureInfo.resolution.X, textureInfo.resolution.Y);
                                graphic.DrawImage(bm, splite.x, splite.y, new RectangleF(0, 0, splite.width, splite.height), GraphicsUnit.Pixel);
                            }
                        }
                    }
                }
                bitmap.Save(re, ImageFormat.Png);
            }
            GC.Collect();
        }
    }
}