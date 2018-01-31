using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Helper;

namespace svnVersion
{
    public class SvnFileInfo
    {
        public string path;
        public string action;
        public string version;
        public string content_md5;
        public string content_size;
        public string encrypt_md5;
        public string encrypt_size;

        public string path_md5
        {
            get { return Library.Encrypt.MD5.Encrypt(path); }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", path_md5, action, version, content_md5, content_size,
                encrypt_md5, encrypt_size, path);
        }
    }

    public class SvnPatchInfo
    {
        public string size;
        public string content_md5;
        public string path;
        public string path__md5;
        public string zip_md5;
        public string encrypt_md5;

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", path, size, content_md5, path__md5, encrypt_md5);
        }
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

        protected void DeleteInfo(string targetMd5Dir, bool onlyDir = false)
        {
            if (Directory.Exists(targetMd5Dir))
                Directory.Delete(targetMd5Dir, true);
            if (onlyDir)
                return;
            if (File.Exists(targetMd5Dir + ".txt"))
                File.Delete(targetMd5Dir + ".txt");
        }

        /// <summary>
        /// 路径MD5化
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="targetDir"></param>
        /// <param name="cache"></param>
        protected void PathToMd5(string folder, string targetDir, Dictionary<string, SvnFileInfo> cache)
        {
            if (folder == null) return;
            Console.Write("\n是否将路径MD5化（y/n），然后回车：");
            bool yes = Console.ReadLine() == "y";

            var targetMd5Dir = targetDir.Replace(folder, Library.Encrypt.MD5.Encrypt(folder + key_md5));
            DeleteInfo(targetMd5Dir);

            if (!yes) return;

            foreach (var s in cache)
            {
                string fullPath = targetDir + "/" + s.Key;
                string targetFullPath = targetMd5Dir + "/" + Library.Encrypt.MD5.Encrypt(s.Key + key_md5);
                if (!File.Exists(fullPath)) continue;
                FileHelper.CreateDirectory(targetFullPath);
                File.Copy(fullPath, targetFullPath, true);
            }

            DeleteInfo(targetDir);
            File.WriteAllLines(targetMd5Dir + ".txt", cache.Select(q => q.Value.ToString()).ToArray(),
                new UTF8Encoding(false));
        }

        /// <summary>
        /// 每一个文件进行加密
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="targetDir"></param>
        /// <param name="cache"></param>
        protected void MakAESEncrypt(string folder, string targetDir, Dictionary<string, SvnFileInfo> cache)
        {
            if (folder == null) return;
            Console.Write("\n是否对文件夹内每个文件进行加密（y/n），然后回车：");
            bool yes = Console.ReadLine() == "y";
            if (!yes) return;
            var targetMd5Dir = targetDir.Replace(folder, Library.Encrypt.MD5.Encrypt(folder + key_md5));
            bool md5 = Directory.Exists(targetMd5Dir);
            foreach (var s in cache)
            {
                string fullPath = "";
                if (!md5)
                    fullPath = targetDir + "/" + s.Key;
                else
                    fullPath = targetMd5Dir + "/" + Library.Encrypt.MD5.Encrypt(s.Key + key_md5);
                if (!File.Exists(fullPath)) continue;
                var bytes = Library.Encrypt.AES.Encrypt(File.ReadAllBytes(fullPath), key_aes);
                s.Value.encrypt_md5 = Library.Encrypt.MD5.Encrypt(fullPath + key_md5);
                s.Value.encrypt_size = bytes.Length.ToString();
                File.WriteAllBytes(fullPath, bytes);
            }
            File.WriteAllLines(targetMd5Dir + ".txt", cache.Select(q => q.Value.ToString()).ToArray(),
                new UTF8Encoding(false));
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
            DeleteInfo(folder, true);
        }
    }
}
