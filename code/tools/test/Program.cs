using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

    internal static class Test
    {
        private static void Main(string[] args)
        {
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
