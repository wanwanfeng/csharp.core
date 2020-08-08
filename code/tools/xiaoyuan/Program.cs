using Library;
using Library.Extensions;
using System;
using System.ComponentModel;

namespace xiaoyuan
{
    internal class Program
    {
        private enum MyEnum
        {
            [Category("Check")] [Description("卡片检测"), TypeValue(typeof(checkcard.CheckCard))] CheckCard,
            [Category("Check")] [Description("剧情符号检测"), TypeValue(typeof(checkcard.CheckScenario))] CheckScenario,
            [Category("Check")] [Description("剧情导出"), TypeValue(typeof(checkcard.ExportScenario))] ExportScenario,
        }

        private static void Main(string[] args)
        {
            SystemConsole.Run<MyEnum>();
        }

        internal class Json
        {
            private static void Main(string[] args)
            {
                SystemConsole.Run<MyEnum>(group: "Json");
            }
        }

        internal class Check
        {
            private static void Main(string[] args)
            {
                SystemConsole.Run<MyEnum>(group: "Check");
            }
        }
    }

    internal class Test
    {
        private static void Main(string[] args)
        {
            //var root = @"D:\Work\magica\client\~public\tw\magica";
            //File.WriteAllText("test.txt",
            //    JsonHelper.ToJson(FileHelper.Path2Dictionary(root, (k, p) => File.ReadAllBytes(root + "\\" + p).MD516())));
            //File.WriteAllText("test1.txt",
            //    JsonHelper.ToJson(FileHelper.Path2List(root, (p) => File.ReadAllBytes(root + "\\" + p).MD516())));

            //BBB.Instance.Init();
            //Console.ReadKey();
        }


        public class AAA
        {
            public AAA()
            {
                Console.WriteLine("AAA");
            }
        }

        public class BBB : AAA
        {

            public static BBB Instance
            {
                get { return SingletonBase<BBB>.Instance; }
            }

            public BBB()
            {
                Console.WriteLine("BB");
            }

            public void Init()
            {

            }
        }
    }
}
