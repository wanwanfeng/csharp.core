﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FileVersion
{
    internal class Program
    {
        private static bool isRuning = true;

        private static void Main(string[] args)
        {
            // 添加程序集解析事件
            AppDomain.CurrentDomain.AssemblyResolve += (sender, assembly) => LoadFromResource(assembly.Name);
            

            while (isRuning)
            {
                string mes =
                    @"1,SvnMaster [输入sm]
2,SvnPatch [输入sp]
3,SvnUpdate [输入sl]
";
//1,GitMaster [输入gm]
//2,GitPatch [输入gp]
//3,GitUpdate [输入gl]
//";
                Console.WriteLine(mes);
                Console.Write("请输入选择，然后回车：");
                CommonBase commonBase = null;
                var haha = Console.ReadLine();
                switch (haha)
                {
                    case "sm":
                        commonBase = new SvnMaster();
                        break;
                    case "sp":
                        commonBase = new SvnPatch();
                        break;
                    case "sl":
                        commonBase = new SvnUpdate();
                        break;
                    case "gm":
                        commonBase = new GitMaster();
                        break;
                    case "gp":
                        commonBase = new GitPatch();
                        break;
                    case "gl":
                        commonBase = new GitUpdate();
                        break;
                }

                if (commonBase == null) return;
                if (commonBase.isInstall)
                    commonBase.Run();
                Console.WriteLine("按y键继续,按其余键退出......");
                isRuning = Console.ReadLine() == "y";
                Console.Clear();
            }
        }


        /// <summary>
        /// 根据要加载的资源项名，加载对应的程序集。
        /// </summary>
        /// <param name="argsName">要解析的项的名称。</param>
        /// <returns>返回对应项的程序集。</returns>
        private static Assembly LoadFromResource(string argsName)
        {
            string dllName = new AssemblyName(argsName).Name + ".dll";

            var assem = Assembly.GetExecutingAssembly();
            var resourceName = assem.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith(dllName));
            if (resourceName == null) return null; // 没找到程序集。

            using (Stream stream = assem.GetManifestResourceStream(resourceName))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }
}
