using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Library.Extensions;

namespace Script
{
    public class TextureInfo
    {
        public bool voild
        {
            get
            {
                var isbool = !(width == 0 || height == 0);
                if (isbool == false)
                {
                    Console.WriteLine("无效图片:" + fullPath);
                }
                return isbool;
            }
        }


        public string name;
        public int width, height;
        public PointF resolution = new PointF(96, 96);

        public List<Splite> list;
        public string fullPath;

        public TextureInfo(PList plist, string re)
        {
            list = new List<Splite>();

            fullPath = re;

            if (plist.ContainsKey("textureFileName"))
            {
                name = plist.GetValue<string>("textureFileName");
                var imagestr = plist.GetValue<string>("textureImageData");
                var newName = Path.GetDirectoryName(fullPath) + "/" + name;

                //File.WriteAllBytes(newName, Convert.FromBase64String(imagestr));

                //byte[] imageBytes = Convert.FromBase64String(imagestr);
                //Stream stream = new MemoryStream(imageBytes, 0, imageBytes.Length);
                //stream.Write(imageBytes, 0, imageBytes.Length);
                //Image image = Image.FromStream(stream);
                //image.Save(newName, ImageFormat.Png);
                //image.Dispose();
                //stream.Dispose();
            }
            else if (plist.ContainsKey("metadata/textureFileName"))
            {
                name = plist.GetValue<string>("metadata/textureFileName");

                if (plist.ContainsKey("metadata/width"))
                {
                    width = plist.GetValue<int>("texture/width");
                    height = plist.GetValue<int>("texture/height");
                }
                else if (plist.ContainsKey("metadata/textureFileName"))
                {
                    name = plist.GetValue<string>("metadata/textureFileName");
                    var arr = GetValue(plist, "metadata/size");
                    width = arr.First().AsInt();
                    height = arr.Skip(1).First().AsInt();
                }
                AddList(plist);
            }
            else
            {
                Console.WriteLine("Splite:" + fullPath);
            }

            var imagePath = Path.GetDirectoryName(re) + "/" + name;
            if (File.Exists(imagePath))
            {
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    Image image = Image.FromStream(fs);
                    width = image.Width;
                    height = image.Height;
                    resolution = new PointF(image.HorizontalResolution, image.VerticalResolution);
                }
            }
            else
            {
                Console.WriteLine("图片不存在:" + imagePath);
            }
        }

        private void AddList(PList plist)
        {
            var obj = plist.GetValue<Dictionary<string, object>>("frames");
            foreach (var bb in obj)
            {
                list.Add(new Splite(plist, bb.Key));
            }
        }

        public static string[] GetValue(PList plist, string key)
        {
            return plist.GetValue<string>(key).Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Split(',');
        }

        public class Splite
        {
            public string name;
            public int width, height, x, y, originalWidth, originalHeight;
            public float offsetX, offsetY;
            public bool rotated = false;
            public Splite(PList plist, string key)
            {
                name = "frames/" + key;

                if (plist.ContainsKey(name + "/width"))
                {
                    width = plist.GetValue<int>(name + "/width");
                    height = plist.GetValue<int>(name + "/height");
                    x = plist.GetValue<int>(name + "/x");
                    y = plist.GetValue<int>(name + "/y");

                    offsetX = plist.GetValue<float>(name + "/offsetX");
                    offsetY = plist.GetValue<float>(name + "/offsetY");

                    originalWidth = plist.GetValue<int>(name + "/originalWidth");
                    originalHeight = plist.GetValue<int>(name + "/originalHeight");
                }
                else if (plist.ContainsKey(name + "/frame") && plist.ContainsKey(name + "/rotated"))
                {
                    var arr = GetValue(plist, name + "/frame");
                    x = arr.First().AsInt();
                    y = arr.Skip(1).First().AsInt();
                    width = arr.Skip(2).First().AsInt();
                    height = arr.Skip(3).First().AsInt();

                    arr = GetValue(plist, name + "/offset");
                    offsetX = arr.First().AsFloat();
                    offsetY = arr.Skip(1).First().AsFloat();

                    arr = GetValue(plist, name + "/sourceSize");
                    originalWidth = arr.First().AsInt();
                    originalHeight = arr.Skip(1).First().AsInt();

                    rotated = plist.GetValue<bool>(name + "/rotated");
                }
                else if (plist.ContainsKey(name + "/aliases") && plist.ContainsKey(name + "/textureRotated"))
                {
                    var arr = GetValue(plist, name + "/textureRect");
                    x = arr.First().AsInt();
                    y = arr.Skip(1).First().AsInt();
                    width = arr.Skip(2).First().AsInt();
                    height = arr.Skip(3).First().AsInt();

                    arr = GetValue(plist, name + "/spriteOffset");
                    offsetX = arr.First().AsFloat();
                    offsetY = arr.Skip(1).First().AsFloat();

                    arr = GetValue(plist, name + "/spriteSourceSize");
                    originalWidth = arr.First().AsInt();
                    originalHeight = arr.Skip(1).First().AsInt();

                    rotated = plist.GetValue<bool>(name + "/textureRotated");
                }
                else
                {
                    Console.WriteLine("Splite:" + name);
                }
                name = Path.GetFileName(name);


                if (rotated)
                {
                    var t = width;
                    width = height;
                    height = t;
                }

                Console.WriteLine(ToString());
            }

            public override string ToString()
            {
                return string.Format("OriginalHeight: {0}, OriginalWidth: {1}, Y: {2}, X: {3}, Height: {4}, Width: {5}, Name: {6}", originalHeight, originalWidth, y, x, height, width, name);
            }
        }
    }
}