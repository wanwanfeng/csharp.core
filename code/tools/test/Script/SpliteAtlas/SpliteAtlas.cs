using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace Script
{
    public class SpliteAtlas : BaseClass
    {
        private string cmd;
        public override string root { get; set; }


        /// <summary>
        /// 筛选过滤已存在图片
        /// </summary>
        public SpliteAtlas()
        {
            cmd = SystemConsole.GetInputStr("图集拆解(y)，图集合并(n)，文件夹删除(d):", "","y");
            root = SystemConsole.GetInputStr("请拖入选定（文件夹或文件）:");

            List<string> res = new List<string>();
            if (Directory.Exists(root))
            {
                res = Directory.GetFiles(root, "*", SearchOption.AllDirectories)
                    //.Where(p => p.Contains("cocos_Data"))
                    //.Where(p => p.Contains("Resources"))
                    .Where(p => p.EndsWith(".plist"))
                    //.Where(p => Directory.Exists(Path.ChangeExtension(p, ".plist").Replace(".plist","")))
                    .Select(p => p.Replace("\\", "/"))
                    .ToList();
            }
            else
            {
                //文件路径
                if (File.Exists(root) && Path.GetExtension(root) == ".plist")
                    res.Add(root);
            }
            if (res.Count == 0) return;
            res.Sort();
            RunList(res);
        }

        public override void RunListOne(string re)
        {
            PList plist = new PList();
            plist.Load(re);
            TextureInfo textureInfo = new TextureInfo(plist, re);
            if (!textureInfo.voild) return;

            var imagePath = Path.GetDirectoryName(re) + "/" + textureInfo.name;

            switch (cmd)
            {
                case "y":
                {
                    if (File.Exists(imagePath))
                        HaveImageAndRead(imagePath, textureInfo);
                    break;
                }
                case "n":
                {
                    if (File.Exists(imagePath))
                        HaveImageAndWrite(imagePath, textureInfo);
                    break;
                }
   
                case "d": 
                {
                    string dir = Path.GetDirectoryName(imagePath) + "/" + Path.GetFileNameWithoutExtension(imagePath);
                    if (Directory.Exists(dir))
                        Directory.Delete(dir, true);
                    break;
                }
            }
        }

        private static void HaveImageAndRead(string re, TextureInfo textureInfo)
        {
            using (FileStream fs = new FileStream(re, FileMode.Open, FileAccess.Read))
            {
                Image image = Image.FromStream(fs);
                foreach (var splite in textureInfo.list)
                {
                    var newName = Path.GetDirectoryName(re) + "/" +
                                  Path.GetFileNameWithoutExtension(textureInfo.name) + "/" + splite.name;
                    FileHelper.CreateDirectory(newName);

                    var bitmap = new Bitmap(splite.width, splite.height, PixelFormat.Format32bppArgb);
                    bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                    Graphics graphic = Graphics.FromImage(bitmap);
                    //graphic.Clear(Color.FromArgb(0, 255, 0, 0));
                     graphic.DrawImage(image, 0, 0, new Rectangle(splite.x, splite.y, splite.width, splite.height),GraphicsUnit.Pixel);
                    //bitmap.MakeTransparent(Color.Transparent);
                    bitmap.Save(newName, ImageFormat.Png);

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
            var bitmap = new Bitmap(textureInfo.width, textureInfo.height, PixelFormat.Format32bppArgb);
            Graphics graphic = Graphics.FromImage(bitmap);
            bitmap.SetResolution(textureInfo.resolution.X, textureInfo.resolution.Y);
            foreach (var splite in textureInfo.list)
            {
                var newName = Path.GetDirectoryName(re) + "/" +
                              Path.GetFileNameWithoutExtension(textureInfo.name) + "/" + splite.name;
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
                    var bm = new Bitmap(splite.width, splite.height, PixelFormat.Format32bppArgb);
                    bm.SetResolution(textureInfo.resolution.X, textureInfo.resolution.Y);
                    graphic.DrawImage(bm, splite.x, splite.y, new RectangleF(0, 0, splite.width, splite.height), GraphicsUnit.Pixel);
                    bm.Dispose();
                }
            }

            bitmap.Save(re, ImageFormat.Png);
            graphic.Dispose();
            bitmap.Dispose();

            GC.Collect();
        }
    }
}