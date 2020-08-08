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
        public static void Run(object o)
        {
            var commonBase = (CommonBase)o;
            if (commonBase == null) return;
            if (commonBase.isInstall)
                commonBase.Run();
        }

        private static void Main(string[] args)
        {
            SystemConsole.Run<SvnList.MyEnum, Svn.MyEnum, Git.MyEnum>(columnsCount: 4, callAction: Run);
        }

        internal class SvnList
        {
            public enum MyEnum
            {
                [TypeValue(typeof(FileVersion.SvnList))] SvnList,
            }

            private static void Main(string[] args)
            {
                SystemConsole.Run<MyEnum>(callAction: Run);
            }
        }

        internal class Svn
        {
            public enum MyEnum
            {
                [TypeValue(typeof(SvnMaster))] SvnMaster,
                [TypeValue(typeof(SvnPatch))] SvnPatch,
                [TypeValue(typeof(SvnUpdate))] SvnUpdate,
            }

            private static void Main(string[] args)
            {
                SystemConsole.Run<MyEnum>(callAction: Run);
            }
        }

        internal class Git
        {
            public enum MyEnum
            {
                [TypeValue(typeof(GitMaster))] GitMaster,
                [TypeValue(typeof(GitPatch))] GitPatch,
                [TypeValue(typeof(GitUpdate))] GitUpdate,
            }

            private static void Main(string[] args)
            {
                SystemConsole.Run<MyEnum>(callAction: Run);
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
