using System;
using System.Collections.ObjectModel;
using System.Linq;
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
            [Description("Json文件检测"), TypeValue(typeof (CheckJson))] CheckJson,
            [Description("卡片检测"), TypeValue(typeof (CheckCard))] CheckCard,
            [Description("剧情符号检测"), TypeValue(typeof (CheckScenario))] CheckScenario,
            [Description("剧情导出"), TypeValue(typeof (ExportScenario))] ExportScenario,
        }

        private static void Main(string[] args)
        {
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
