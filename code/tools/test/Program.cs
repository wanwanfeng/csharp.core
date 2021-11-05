using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Library;
using Library.Extensions;
using Library.Helper;
using LitJson.P;

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

            [Category("Image/Base64"), Description("ImageConvertToBase64"), TypeValue(typeof(ImageConvertToBase64))] ImageConvertToBase64,
            [Category("Image/Base64"), Description("Base64ConvertToImage"), TypeValue(typeof(Base64ConvertToImage))] Base64ConvertToImage,

            [Category("Image/Splite"), Description("SpliteAdobeAnimateAtlas"), TypeValue(typeof(SpliteAdobeAnimateAtlas))] SpliteAdobeAnimateAtlas,
            [Category("Image/Splite"), Description("SpliteAtlas"), TypeValue(typeof(SpliteAtlas))] SpliteAtlas, 
            //[Category("Image"), Description("CheckImage"), TypeValue(typeof (CheckImage))] CheckImage,
            [Category("Image/ProgressiveJpeg"), Description("ImageProgressiveJpeg"), TypeValue(typeof(ImageProgressiveJpeg))] ImageProgressiveJpeg,
            [Category("Image/ProgressiveJpeg"), Description("ImageCmdProgressiveJpeg"), TypeValue(typeof(ImageCmdProgressiveJpeg))] ImageCmdProgressiveJpeg,
            [Category("Image/Find"), Description("ImageFind"), TypeValue(typeof(ImageFind))] ImageFind,
            [Category("Image/Find"), Description("ImageFindOrc(图像识别)"), TypeValue(typeof(ImageFindOrc))] ImageFindOrc,
        }

        private static void Main(string[] args)
        {
            SystemConsole.Run<MyEnum, Dos2.MyEnum>(columnsCount: 4);
        }

        internal class Dos2
        {
            internal enum MyEnum
            {
                [Description("dos2unix"), TypeValue(typeof(dos2unix))] dos2unix,
                [Description("unix2dos"), TypeValue(typeof(unix2dos))] unix2dos,
                [Description("mac2unix"), TypeValue(typeof(mac2unix))] mac2unix,
                [Description("unix2mac"), TypeValue(typeof(unix2mac))] unix2mac,
            }

            private static void Main(string[] args)
            {
                SystemConsole.Run<MyEnum>(group: "dos2");
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
                    Console.WriteLine(Encoding.UTF8.GetString(yyy));
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

        public static void JsonDataToDict(JsonData jsonData, Dictionary<string, object> cache, string keyName = "")
        {
            if (jsonData.IsObject)
            {
                if (jsonData.Keys.Count == 0)
                {
                    cache[keyName] = new Dictionary<string, object>();
                }
                else
                {
                    foreach (var key in jsonData.Keys)
                    {
                        JsonData value = jsonData[key];
                        Console.WriteLine(key + ":" + value.GetJsonType());
                        if (value.IsObject)
                        {
                            cache[key] = new Dictionary<string, object>();
                            JsonDataToDict(value, cache[key] as Dictionary<string, object>, key);
                        }
                        else
                        {
                            JsonDataToDict(value, cache, key);
                        }
                    }
                }
            }
            else if (jsonData.IsArray)
            {
                object[]  array = new object[jsonData.Count];
                cache[keyName] = array;
                int index = 0;
                foreach (JsonData value in jsonData)
                {
                    if (value.IsObject || value.IsArray)
                    {
                        array[index] = new Dictionary<string, object>();
                        JsonDataToDict(value, array[index] as Dictionary<string, object>, "");
                    }
                    else
                    {
                        if (value.IsString)
                        {
                            array[index] = (string)value;
                        }
                        else if (value.IsLong)
                        {
                            array[index] = (long)value;
                        }
                        else if (value.IsInt)
                        {
                            array[index] = (int)value;
                        }
                        else if (value.IsDouble)
                        {
                            array[index] = (double)value;
                        }
                        else if (value.IsBoolean)
                        {
                            array[index] = (bool)value;
                        }
                    }
                    index++;
                }
            }
            else
            {
                if (jsonData.IsString)
                {
                    cache[keyName] = (string)jsonData;
                }
                else if (jsonData.IsLong)
                {
                    cache[keyName] = (long)jsonData;
                }
                else if (jsonData.IsInt)
                {
                    cache[keyName] = (int)jsonData;
                }
                else if (jsonData.IsDouble)
                {
                    cache[keyName] = (double)jsonData;
                }
                else if (jsonData.IsBoolean)
                {
                    cache[keyName] = (bool)jsonData;
                }
            }
        }

        private static void Main(string[] args)
        {

            string content = File.ReadAllText("test.txt");
            var xx = JsonHelper.ToObject<JsonData>(content);

            Dictionary<string, object> cache = new Dictionary<string, object>();

            JsonDataToDict(xx, cache);

            Console.WriteLine(xx);

            File.WriteAllText("text.txt",JsonHelper.ToJson(xx));
            File.WriteAllText("text2.txt",JsonHelper.ToJson(cache));

            Console.WriteLine(cache);









            FileHelper.Serialize("xxx.txt", "dddddddddddddd","dssttttttttttttttttttttttttty5675756");
            string value = FileHelper.Deserialize<string>("xxx.txt", "dssttttttttttttttttttttttttty5675756");

            Console.WriteLine(value);










            WebHelper.Post("http://wanwanfeng.gitee.io/demo.assets/version.txt", callAction: (state, result) =>
            {
                Console.WriteLine(result);
                if (state)
                {
                }
                else
                {

                }
            });


            FileHelper.WriteAllText("test.txt", "上面这段程序运行了{0}秒", "ffffff");
            var  xxx = FileHelper.ReadAllText("test.txt", "ffffff");

            Console.WriteLine(xxx);


            int bossColor;

            //十六进制
            bossColor = 0xBBFF11;
            int r = (bossColor & 0xFF0000) >> 16;
            //十进制
            bossColor = 12320529;
            int g = (bossColor & 0xFF0000) >> 16;

            Console.WriteLine(r==g);

            JsonData json = new JsonData();
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
