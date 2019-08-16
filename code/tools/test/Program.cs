using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Library;
using Library.Extensions;

namespace Script
{
    internal class Program
    {
        internal enum MyEnum
        {
            //[Description("CompareFolder"), TypeValue(typeof (CompareFolder))] CompareFolder = 1,
            [Description("CopyToOneFolder"), TypeValue(typeof(CopyToOneFolder))] CopyToOneFolder,
            //[Description("CreatePhotoDir"), TypeValue(typeof (CreatePhotoDir))] CreatePhotoDir,
            //[Description("CreateExcelCell"), TypeValue(typeof (CreateExcelCell))] CreateExcelCell,
            [Description("GetLineCount"), TypeValue(typeof(GetLineCount))] GetLineCount,
            //[Description("DeleteFiles"), TypeValue(typeof (DeleteFiles))] DeleteFiles,

            [Category("Image"), Description("ImageConvertToBase64"), TypeValue(typeof(ImageConvertToBase64))] ImageConvertToBase64,
            [Category("Image"), Description("Base64ConvertToImage"), TypeValue(typeof(Base64ConvertToImage))] Base64ConvertToImage,

            [Category("Image"), Description("SpliteAtlas"), TypeValue(typeof(SpliteAtlas))] SpliteAtlas,
            //[Category("Image"), Description("CheckImage"), TypeValue(typeof (CheckImage))] CheckImage,
            [Category("Image"), Description("ImageProgressiveJpeg"), TypeValue(typeof(ImageProgressiveJpeg))] ImageProgressiveJpeg,
            [Category("Image"), Description("ImageCmdProgressiveJpeg"), TypeValue(typeof(ImageCmdProgressiveJpeg))] ImageCmdProgressiveJpeg,
            [Category("Image"), Description("ImageFind"), TypeValue(typeof(ImageFind))] ImageFind,
            [Category("Image"), Description("ImageFindOrc(图像识别)"), TypeValue(typeof(ImageFindOrc))] ImageFindOrc,

            [Category("dos2"), Description("dos2unix"), TypeValue(typeof(dos2unix))] dos2unix,
            [Category("dos2"), Description("unix2dos"), TypeValue(typeof(unix2dos))] unix2dos,
            [Category("dos2"), Description("mac2unix"), TypeValue(typeof(mac2unix))] mac2unix,
            [Category("dos2"), Description("unix2mac"), TypeValue(typeof(unix2mac))] unix2mac,
        }

        private static void Main(string[] args)
        {
            SystemConsole.Run<MyEnum>(columnsCount: 3);
        }

        internal class Dos2
        {
            private static void Main(string[] args)
            {
                SystemConsole.Run<MyEnum>(group: "dos2");
            }
        }

        internal class Image
        {
            private static void Main(string[] args)
            {
                SystemConsole.Run<MyEnum>(group: "Image");
            }
        }
    }

    internal static class TestYIHUO
    {
        private static void Main(string[] args)
        {
            string key = "fdgjdjgdgkjdhfjdyhjkfghk";

            string str = @"Python 是一个高层次的结合了解释性、编译性、互动性和面向对象的脚本语言。
            Python 的设计具有很强的可读性，相比其他语言经常使用英文关键字，其他语言的一些标点符号，它具有比其他语言更有特色语法结构。";
            //            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(str);
            //            Console.WriteLine(string.Join(",", byteArray.Select(p => p.ToString())));
            //            Console.WriteLine(str);
            //            Console.WriteLine(str.Length);
            //            Console.WriteLine(byteArray.Length);

            //            byte[] byteArray2 = byteArray.Select((p, i) => (byte)(p ^ keyArray[i % keyArray.Length])).ToArray();
            //            File.WriteAllText("sssssss.txt", str);
            //            File.WriteAllBytes("sssssss.bytes", byteArray2);
            //            Console.WriteLine(string.Join(",", byteArray2.Select(p => p.ToString())));
            //            Console.WriteLine(byteArray2.Length);

            //            byte[] byteArray3 = byteArray2.Select((p, i) => (byte)(p ^ keyArray[i % keyArray.Length])).ToArray();
            //            Console.WriteLine(string.Join(",", byteArray3.Select(p => p.ToString())));
            //            {
            //                string reault = System.Text.Encoding.UTF8.GetString(byteArray3);
            //                Console.WriteLine(reault);
            //                Console.WriteLine(reault.Length);
            //                Console.WriteLine(byteArray3.Length);
            //            }

            int length = 1000000;
            Console.WriteLine(str);
            {
                long time = DateTime.UtcNow.Ticks;
                for (int i = 0; i < length; i++)
                {
                    var xxx = EncryptExtensions.XOR(Encoding.UTF8.GetBytes(str), key);
                    var yyy = EncryptExtensions.XOR(xxx, key);
                    //Console.WriteLine(Encoding.UTF8.GetString(yyy));
                }

                Console.WriteLine("上面这段程序运行了{0}秒",(DateTime.UtcNow.Ticks - time) / 10000000.0);
            }

            Console.WriteLine(str);
            {
                long time = DateTime.UtcNow.Ticks;
                for (int i = 0; i < length; i++)
                {
                    var xxx = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
                    var yyy = Convert.FromBase64String(xxx);
                    //Console.WriteLine(Encoding.UTF8.GetString(yyy));
                }

                Console.WriteLine("上面这段程序运行了{0}秒", (DateTime.UtcNow.Ticks - time) / 10000000.0);
            }
            Console.ReadKey();
        }
    }

    internal static class Test
    {
        private static void Main(string[] args)
        {

            LitJson.JsonData json = new LitJson.JsonData();
            json["dddd"] = "ghdfgh";
            json["8987"] = "fghf";
            json["kjljk"] = "kkkk";
            json["7777"] = 90909;

            json = json.Remove("kjljk");

            Console.WriteLine(json);

            FileInfo fileInfo = new FileInfo("TextFile");
            Console.WriteLine("ex:{0}", fileInfo.Extension);
            Console.WriteLine("ex:{0}", fileInfo.Length);
            fileInfo = new FileInfo("hh");
            Console.WriteLine("ex:{0}", fileInfo.Extension);

            Console.ReadKey();

            Branch myEnum;

            myEnum = (Branch)(~(0 << -1));

            Console.WriteLine(myEnum);
            Console.WriteLine(!IsClose(Branch.None, Branch.None));


            new hahab();
            Console.ReadKey();
        }

        [Flags]
        public enum Branch
        {
            None = 1 << 0,
            Bs = 1 << 1,
            Cb1 = 1 << 2,
            Cb2 = 1 << 4,
        }

        /// <summary>
        /// 屏蔽的权限
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        public static bool IsClose(Branch branch = Branch.None, Branch target = Branch.Bs)
        {
            //全部权限
            Branch b = (Branch)~(0 << -1);
            //移除权限
            b = (b & ~branch);
            var cur = target;
            //判断是否有剩余权限
            if ((b & cur) == cur)
            {
                return false;
            }
            return true;
        }

    }
    public class hahaa
    {
        static hahaa()
        {
            Console.WriteLine("static hahaa");
        }

        public hahaa()
        {
            Console.WriteLine("public hahaa");
        }
    }

    public class hahab : hahaa
    {
        static hahab()
        {
            Console.WriteLine("static hahab");
        }

        public hahab()
        {
            Console.WriteLine("public hahab");
        }
    }
}
