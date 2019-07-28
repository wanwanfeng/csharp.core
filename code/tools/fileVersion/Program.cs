using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Library;
using Library.Extensions;

namespace FileVersion
{
    internal class Program
    {
        public enum MyEnum
        {
            [Category("Svn命令")] [TypeValue(typeof(FileVersion.SvnList))] SvnList = 1,
            [Category("\nSvn【命令2->3->4】")] [TypeValue(typeof(SvnMaster))] SvnMaster,
            [Category("\nSvn【命令2->3->4】")] [TypeValue(typeof(SvnPatch))] SvnPatch,
            [Category("\nSvn【命令2->3->4】")] [TypeValue(typeof(SvnUpdate))] SvnUpdate,
            [Category("\nGit【命令5->6->7】")] [TypeValue(typeof(GitMaster))] GitMaster,
            [Category("\nGit【命令5->6->7】")] [TypeValue(typeof(GitPatch))] GitPatch,
            [Category("\nGit【命令5->6->7】")] [TypeValue(typeof(GitUpdate))] GitUpdate,
        }

        public static void Run(object o)
        {
            var commonBase = (CommonBase)o;
            if (commonBase == null) return;
            if (commonBase.isInstall)
                commonBase.Run();
        }

        private static void Main(string[] args)
        {
            SystemConsole.Run<MyEnum>(columnsCount: 4, callAction: Run);
        }

        internal class SvnList
        {
            private static void Main(string[] args)
            {
                SystemConsole.Run<MyEnum>(group: "Svn命令", callAction: Run);
            }
        }

        internal class Svn
        {
            private static void Main(string[] args)
            {
                SystemConsole.Run<MyEnum>(group: "\nSvn【命令2->3->4】", callAction: Run);
            }
        }

        internal class Git
        {
            private static void Main(string[] args)
            {
                SystemConsole.Run<MyEnum>(group: "\nGit【命令5->6->7】", callAction: Run);
            }
        }
    }
    internal class Test
    {
        private static void Main(string[] args)
        {
            // 添加程序集解析事件
            AppDomain.CurrentDomain.AssemblyResolve += (sender, assembly) => LoadFromResource(assembly.Name);
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
