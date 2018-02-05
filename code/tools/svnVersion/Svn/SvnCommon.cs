using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library;
using Library.Extensions;
using Library.Helper;

namespace SvnVersion
{
    public class SvnCommon : CmdHelp
    {
        protected static string KeyMd5 { get; private set; }
        protected static string Exclude { get; private set; }

        static SvnCommon()
        {
            KeyMd5 = Config.IniReadValue("Config", "md5key").Trim();
            Library.Encrypt.AES.Key = Config.IniReadValue("Config", "aeskey").Trim();
            Library.Encrypt.AES.Head = Config.IniReadValue("Config", "aeshead").Trim();
            Exclude = Config.IniReadValue("Config", "exclude").Trim();
        }

        public SvnCommon()
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("KeyMd5:" + KeyMd5);
            Console.WriteLine("AES.Key:" + Library.Encrypt.AES.Key);
            Console.WriteLine("AES.Head:" + Library.Encrypt.AES.Head);
            Console.WriteLine("Exclude:" + Exclude);
            Console.WriteLine("--------------------------------------");
        }

        public virtual string Name
        {
            get { return ""; }
        }

        public string Extension
        {
            get { return ".txt"; }
        }

        public Encoding TxTEncoding
        {
            get { return new UTF8Encoding(false); }
        }

        public string svnVersion { get; protected set; }
        public string svnUrl { get; protected set; }
        public string folder { get; protected set; }
        public int highVersion { get; protected set; }
        public int lowVersion { get; protected set; }

        public virtual void Run()
        {
            svnVersion = RunCmd("svn --version --quiet").Last();
            Console.WriteLine("SVN版本：" + svnVersion);

            bool yes = false;
            while (yes == false)
            {
                Console.Write("请输入目标目录，然后回车：");
                folder = Console.ReadLine();
                if (folder != null && !Directory.Exists(folder))
                {
                    Console.WriteLine("未输入目录或不存在!");
                    //Console.Write("\n是否将本目录作为目标目录（y/n）：");
                    //yes = Console.ReadLine() == "y";
                }
                else
                {
                    break;
                }
            }

            svnUrl = RunCmd("svn info --show-item url").Last();
            svnUrl += "/" + folder;
            Console.WriteLine("库地址：" + svnUrl);

            Console.WriteLine("");
            highVersion = RunCmd("svn info --show-item last-changed-revision " + folder).Last().AsInt();
            Console.WriteLine("最高版本号：" + highVersion);

            var logs = RunCmd(string.Format("svn log -r 0:{0} \"{1}@{0}\" -q -l1 --stop-on-copy", highVersion, svnUrl));
            lowVersion = logs.Skip(1).First().Split('|').First().Replace("r", "").Trim().AsInt();
            Console.WriteLine("最低版本号：" + lowVersion);

            Console.WriteLine("");
        }

        public void Common(string targetDir, Dictionary<string, SvnFileInfo> cache)
        {
            Console.WriteLine("");
            ExcludeFile(folder, targetDir, cache);
            var count = cache.Values.Count(p => !p.action.Equals("D"));
            if (count == 0)
            {
                Console.WriteLine("可更新文件数目为零！");
                Console.WriteLine("按任意键退出！");
                DeleteInfo(targetDir);
            }
            else
            {
                Console.WriteLine(string.Format("可更新文件数目为{0}！", count));
                Console.WriteLine(string.Format("大小为{0}B！", cache.Values.Sum(p => p.content_size.AsInt()).ToString("N")));
                WriteToTxt(targetDir, cache);
                PathToMd5(folder, targetDir, cache);
                MakAESEncrypt(folder, targetDir, cache);
                MakeFolder(folder, targetDir);
                EndCmd();
            }
        }

        /// <summary>
        /// 排除预定义的文件类型
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="targetDir"></param>
        /// <param name="cache"></param>
        protected void ExcludeFile(string dir, string targetDir, Dictionary<string, SvnFileInfo> cache)
        {
            var array = Exclude.Split(',');
            if (array.Length == 0) return;
            var deleteKey = new List<string>();
            foreach (var s in cache)
            {
                foreach (var s1 in array)
                {
                    if (!s.Key.EndsWith(s1)) continue;
                    deleteKey.Add(s.Key);
                    try
                    {
                        if (File.Exists(targetDir + "/" + s.Key))
                        {
                            File.Delete(targetDir + "/" + s.Key);
                            Console.WriteLine("删除成功..." + s.Key);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("删除失败..." + s.Key);
                        throw;
                    }
                }
            }
            foreach (var key in deleteKey)
            {
                cache.Remove(key);
            }
        }

        /// <summary>
        /// 获取文件大小以及MD5
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="svnFileInfo"></param>
        protected void SetContent(string fullPath, SvnFileInfo svnFileInfo)
        {
            var bytes = File.ReadAllBytes(fullPath);
            svnFileInfo.content_size = bytes.Length.ToString();
            svnFileInfo.content_hash = Library.Encrypt.MD5.Encrypt(bytes);
        }

        /// <summary>
        /// 路径MD5化
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="targetDir"></param>
        /// <param name="cache"></param>
        protected void PathToMd5(string dir, string targetDir, Dictionary<string, SvnFileInfo> cache)
        {
            if (dir == null) return;
            Console.Write("\n是否将路径MD5化（y/n）：");
            bool yes = Console.ReadLine() == "y";

            var targetMd5Dir = targetDir.Replace(dir, Library.Encrypt.MD5.Encrypt(dir + KeyMd5));
            DeleteInfo(targetMd5Dir);

            if (!yes) return;

            foreach (var s in cache)
            {
                string fullPath = targetDir + "/" + s.Key;
                string targetFullPath = targetMd5Dir + "/" + Library.Encrypt.MD5.Encrypt(s.Key + KeyMd5);
                if (!File.Exists(fullPath)) continue;
                FileHelper.CreateDirectory(targetFullPath);
                File.Copy(fullPath, targetFullPath, true);
            }

            DeleteInfo(targetDir);
            WriteToTxt(targetMd5Dir, cache);
        }

        /// <summary>
        /// 每一个文件进行加密
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="targetDir"></param>
        /// <param name="cache"></param>
        protected void MakAESEncrypt(string dir, string targetDir, Dictionary<string, SvnFileInfo> cache)
        {
            if (dir == null) return;
            Console.Write("\n是否对文件夹内每个文件进行加密（y/n）：");
            bool yes = Console.ReadLine() == "y";
            if (!yes) return;
            var targetMd5Dir = targetDir.Replace(dir, Library.Encrypt.MD5.Encrypt(dir + KeyMd5));
            bool md5 = Directory.Exists(targetMd5Dir);
            foreach (var s in cache)
            {
                string fullPath = "";
                if (!md5)
                    fullPath = targetDir + "/" + s.Key;
                else
                    fullPath = targetMd5Dir + "/" + Library.Encrypt.MD5.Encrypt(s.Key + KeyMd5);
                if (!File.Exists(fullPath)) continue;
                var content = Library.Encrypt.AES.Encrypt(File.ReadAllText(fullPath));
                s.Value.encrypt_hash = Library.Encrypt.MD5.Encrypt(content);
                s.Value.encrypt_size = new FileInfo(fullPath).Length.ToString();
                File.WriteAllText(fullPath, content);
            }
            WriteToTxt(targetMd5Dir, cache);
        }

        /// <summary>
        /// 文件夹压缩
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="targetDir"></param>
        protected void MakeFolder(string dir, string targetDir)
        {
            if (dir == null) return;
            Console.Write("\n是否将文件夹压缩（y/n）：");
            bool yes = Console.ReadLine() == "y";
            if (!yes) return;

            var targetMd5Dir = targetDir.Replace(dir, Library.Encrypt.MD5.Encrypt(dir + KeyMd5));
            MakeZip(targetDir);
            MakeZip(targetMd5Dir);
        }

        /// <summary>
        /// 文件夹压缩
        /// </summary>
        /// <param name="dir"></param>
        private void MakeZip(string dir)
        {
            if (!Directory.Exists(dir)) return;
            string message = Library.Compress.DecompressUtils.MakeZipFile(dir);
            if (string.IsNullOrEmpty(message))
                Console.WriteLine("文件压缩成功！");
            else
                Console.WriteLine("文件压缩失败！" + message);
            DeleteInfo(dir, true);
        }

        #region

        protected void WriteToTxt(string fileName, Dictionary<string, SvnFileInfo> cache)
        {
            fileName += string.IsNullOrEmpty(Path.GetExtension(fileName)) ? Extension : "";
            File.WriteAllText(fileName, LitJson.JsonMapper.ToJson(cache.Values.ToArray()), TxTEncoding);
        }

        protected void WriteToTxt(string fileName, SvnInfo svnInfo)
        {
            fileName += string.IsNullOrEmpty(Path.GetExtension(fileName)) ? Extension : "";
            File.WriteAllText(fileName, LitJson.JsonMapper.ToJson(svnInfo), TxTEncoding);
        }

        protected void EncryptFile(string fileName)
        {
            fileName += string.IsNullOrEmpty(Path.GetExtension(fileName)) ? Extension : "";
            File.WriteAllText(fileName, Library.Encrypt.AES.Encrypt(File.ReadAllText(fileName)), TxTEncoding);
        }

        protected void DeleteInfo(string dir, bool onlyDir = false)
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            if (onlyDir)
                return;
            if (File.Exists(dir + ".txt"))
                File.Delete(dir + ".txt");
        }

        #endregion
    }
}
