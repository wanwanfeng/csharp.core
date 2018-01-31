using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Helper;

namespace svnVersion
{
    public class SvnFileInfo
    {
        public string action;
        public int version;
        public string size;
        public string value_md5;
        public string path;
        public string path_md5;
        public bool isEncrypt = false;
        public string encrypt_md5;
    }

    public class SvnPatchInfo
    {
        public string size;
        public string content_md5;
        public string path;
        public string path__md5;
        public bool isZip = false;
        public string zip_md5;
        public bool isEncrypt = false;
        public string encrypt_md5;
    }

    public class SvnCommon : CmdHelp
    {

        private string key_md5
        {
            get { return ""; }
        }

        private string key_aes
        {
            get { return ""; }
        }
        public virtual void Run()
        {

        }

        /// <summary>
        /// 路径MD5化
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="targetDir"></param>
        /// <param name="cao"></param>
        protected void PathToMd5(string folder, string targetDir, List<List<string>> cao)
        {
            if (folder == null) return;
            Console.Write("\n是否将路径MD5化（y/n），然后回车：");
            bool yes = Console.ReadLine() == "y";
            if (!yes) return;
            var targetMd5Dir = targetDir.Replace(folder, Library.Encrypt.MD5.Encrypt(folder + key_md5));
            if (Directory.Exists(targetMd5Dir))
                Directory.Delete(targetMd5Dir, true);
            foreach (var s in cao)
            {
                string fullPath = targetDir + "/" + s.Last();
                string targetFullPath = targetMd5Dir + "/" + Library.Encrypt.MD5.Encrypt(s.Last() + key_md5);
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

        /// <summary>
        /// 每一个文件进行加密
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="targetDir"></param>
        protected void MakAESEncrypt(string folder, string targetDir)
        {
            if (folder == null) return;
            Console.Write("\n是否对文件夹内每个文件进行加密（y/n），然后回车：");
            bool yes = Console.ReadLine() == "y";
            if (!yes) return;

            var targetMd5Dir = targetDir.Replace(folder, Library.Encrypt.MD5.Encrypt(folder + key_md5));
            var target = Directory.Exists(targetMd5Dir) ? targetMd5Dir : targetDir;
            var file = FileHelper.GetFiles(target, SearchOption.AllDirectories);
            foreach (string s in file)
            {
                File.WriteAllBytes(s, Library.Encrypt.AES.Encrypt(File.ReadAllBytes(s), key_aes));
            }
        }

        /// <summary>
        /// 文件夹压缩
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="targetDir"></param>
        protected void MakeFolder(string folder, string targetDir)
        {
            if (folder == null) return;
            Console.Write("\n是否将文件夹压缩（y/n），然后回车：");
            bool yes = Console.ReadLine() == "y";
            if (!yes) return;

            var targetMd5Dir = targetDir.Replace(folder, Library.Encrypt.MD5.Encrypt(folder + key_md5));
            MakeZip(targetDir);
            MakeZip(targetMd5Dir);
        }

        /// <summary>
        /// 文件夹压缩
        /// </summary>
        /// <param name="folder"></param>
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
