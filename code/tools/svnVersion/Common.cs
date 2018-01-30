using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Helper;

namespace svnVersion
{
    public class SvnCommon : CmdHelp
    {
        public virtual void Run()
        {

        }

        protected void PathToMd5(string folder, string targetDir, List<List<string>> cao)
        {
            if (folder == null) return;
            Console.Write("\n是否将路径MD5化（y/n），然后回车：");
            bool yes = Console.ReadLine() == "y";
            if (!yes) return;
            var targetMd5Dir = targetDir.Replace(folder, Library.Encrypt.MD5.Encrypt(folder));
            if (Directory.Exists(targetMd5Dir))
                Directory.Delete(targetMd5Dir, true);
            foreach (var s in cao)
            {
                string fullPath = targetDir + "/" + s.Last();
                string targetFullPath = targetMd5Dir + "/" + Library.Encrypt.MD5.Encrypt(s.Last());
                if (!File.Exists(fullPath)) continue;
                FileHelper.CreateDirectory(targetFullPath);
                File.Copy(fullPath, targetFullPath, true);
            }
            if (Directory.Exists(targetDir))
                Directory.Delete(targetDir, true);
            if (File.Exists(targetDir + ".txt"))
            {
                if (File.Exists(targetMd5Dir + ".txt"))
                    File.Delete(targetMd5Dir + ".txt");
                File.Move(targetDir + ".txt", targetMd5Dir + ".txt");
            }
        }

        protected void MakeFolder(string folder, string targetDir)
        {
            if (folder == null) return;
            Console.Write("\n是否将文件夹压缩（y/n），然后回车：");
            bool yes = Console.ReadLine() == "y";
            if (!yes) return;

            var targetMd5Dir = targetDir.Replace(folder, Library.Encrypt.MD5.Encrypt(folder));
            MakeZip(targetDir);
            MakeZip(targetMd5Dir);
        }


        private void MakeZip(string folder)
        {
            if (!Directory.Exists(folder)) return;
            string message = Library.Compress.DecompressUtils.MakeZipFile(folder);
            if (string.IsNullOrEmpty(message))
                Console.WriteLine("文件压缩成功！");
            else
                Console.WriteLine("文件压缩失败！" + message);
        }
    }
}
