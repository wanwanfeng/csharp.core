using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Library.Extensions;

namespace FileVersion
{
    internal class Program
    {
        private static bool isRuning = true;

        private static void Main(string[] args)
        {
            // 添加程序集解析事件
            AppDomain.CurrentDomain.AssemblyResolve += (sender, assembly) => LoadFromResource(assembly.Name);

            string msg =
                @"
------------第一种----------
步骤1,SvnMaster [输入sm]
步骤2,SvnPatch [输入sp]
步骤3,SvnUpdate [输入su]

------------第二种----------
SvnList [输入sl]
";
//1,GitMaster [输入gm]
//2,GitPatch [输入gp]
//3,GitUpdate [输入gl]
//";
            do
            {
                Console.WriteLine(msg);
                CommonBase commonBase = null;
                switch (SystemExtensions.GetInputStr("请输入选择，然后回车："))
                {
                    case "sm":
                        commonBase = new SvnMaster();
                        break;
                    case "sp":
                        commonBase = new SvnPatch();
                        break;
                    case "su":
                        commonBase = new SvnUpdate();
                        break;
                    case "sl":
                        commonBase = new SvnList();
                        break;
                    case "gm":
                        commonBase = new GitMaster();
                        break;
                    case "gp":
                        commonBase = new GitPatch();
                        break;
                    case "gu":
                        commonBase = new GitUpdate();
                        break;
                }

                if (commonBase == null) return;
                if (commonBase.isInstall)
                    commonBase.Run();

                Console.Clear();
            } while (SystemExtensions.ContinueY("按y键继续,按其余键退出......"));
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
