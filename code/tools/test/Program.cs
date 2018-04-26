using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Script;

namespace test
{
    internal class Program
    {
        [Flags]
        public enum Branch
        {
            None = 1 << 0,
            Bs = 1 << 1,
            Cb1 = 1 << 2,
            Cb2 = 1 << 4,
        }

        private static void Main(string[] args)
        {
            string cmd;
            Console.WriteLine("-------命令列表-------");
            Console.WriteLine("1:CompareFolder");
            Console.WriteLine("2:ImageFind");
            Console.WriteLine("3:CopyToOneFolder");
            Console.WriteLine("4:CreatePhotoDir");
            Console.WriteLine("5:CreateExcelCell");
            Console.WriteLine("6:SpliteAtlas");
            Console.WriteLine("----------------------");
            Console.WriteLine("e:exit");
            Console.Write("请输入命令编号：");
            cmd = Console.ReadLine() ?? "e";

            switch (cmd)
            {
                case "1":
                    new CompareFolder();
                    break;
                case "2":
                    new ImageFind();
                    break;
                case "3":
                    new CopyToOneFolder();
                    break;
                case "4":
                    new CreatePhotoDir();
                    break;
                case "5":
                    new CreateExcelCell();
                    break;
                case "6":
                    new SpliteAtlas();
                    break;
                case "e":
                default:
                    Environment.Exit(0);
                    break;
            }
            new SpliteAtlas();
            return;

            //FileInfo fileInfo = new FileInfo("TextFile");
            //Console.WriteLine("ex:{0}", fileInfo.Extension);
            //Console.WriteLine("ex:{0}", fileInfo.Length);
            //fileInfo = new FileInfo("hh");
            //Console.WriteLine("ex:{0}", fileInfo.Extension);
            //Console.WriteLine("ex:{0}", fileInfo.);

            Console.ReadKey();

            Branch myEnum;

            myEnum = (Branch) (~(0 << -1));

            Console.WriteLine(myEnum);
            Console.WriteLine(!IsClose(Branch.None, Branch.None));


            new hahab();
            Console.ReadKey();
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
