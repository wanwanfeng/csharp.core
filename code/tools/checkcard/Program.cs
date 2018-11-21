using System;
using System.ComponentModel;
using checkcard.Scripts;
using Library;
using Library.Extensions;

namespace checkcard
{
    internal class Program
    {
        private enum MyEnum
        {
            [Description("文件拷贝"), TypeValue(typeof (CopyFile))] CopyFile,

            [Description("Json文件检测"), TypeValue(typeof (CheckJson))] CheckJson,
            [Description("Json文件缩进"), TypeValue(typeof (IndentJson))] IndentJson,
            [Description("Json文件取消缩进"), TypeValue(typeof (CancelIndentJson))] CancelIndentJson,

            [Description("卡片检测"), TypeValue(typeof (CheckCard))] CheckCard,
            [Description("剧情符号检测"), TypeValue(typeof (CheckScenario))] CheckScenario,
            [Description("剧情导出"), TypeValue(typeof (ExportScenario))] ExportScenario,
        }

        private static void Main(string[] args)
        {
            //var root = @"D:\Work\magica\client\~public\tw\magica";
            //File.WriteAllText("test.txt",
            //    JsonHelper.ToJson(FileHelper.Path2Dictionary(root, (k, p) => File.ReadAllBytes(root + "\\" + p).MD516())));
            //File.WriteAllText("test1.txt",
            //    JsonHelper.ToJson(FileHelper.Path2List(root, (p) => File.ReadAllBytes(root + "\\" + p).MD516())));

            //BBB.Instance.Init();
            //Console.ReadKey();
            SystemConsole.Run<MyEnum>();
        }


        public class AAA 
        {
            public AAA()
            {
                Console.WriteLine("AAA");
            }
        }

        public class BBB :AAA
        {

            public static BBB Instance
            {
                get { return SingletonBase<BBB>.Instance; }
            }

            public BBB()
            {
                Console.WriteLine("BB");
            }

            public  void Init()
            {
            
            }
        }
    }
}
