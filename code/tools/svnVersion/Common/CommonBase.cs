using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library;
using Library.Extensions;
using Library.Helper;

namespace FileVersion
{
    public class CommonBase : CmdHelp
    {
        /// <summary>
        /// 是否已经安装svn或git
        /// </summary>
        public bool isInstall = false;

        /// <summary>
        /// svn或git版本号
        /// </summary>
        public string softwareVersion { get; protected set; }

        protected static int PathToMd5Depth { get; private set; }
        protected static string KeyMd5 { get; private set; }
        protected static string Exclude { get; private set; }
        protected static string Platform { get; private set; }

        static CommonBase()
        {
            PathToMd5Depth = Config.IniReadValue("Config", "pathtomd5depth").Trim().AsInt();
            PathToMd5Depth = Math.Min(PathToMd5Depth, 2);
            KeyMd5 = Config.IniReadValue("Config", "md5key").Trim();
            Library.Encrypt.AES.Key = Config.IniReadValue("Config", "aeskey").Trim();
            Library.Encrypt.AES.Head = Config.IniReadValue("Config", "aeshead").Trim();
            Exclude = Config.IniReadValue("Config", "exclude").Trim();
            Platform = Config.IniReadValue("Config", "platform").Trim();
        }

        public CommonBase()
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("KeyMd5:" + KeyMd5);
            Console.WriteLine("AES.Key:" + Library.Encrypt.AES.Key);
            Console.WriteLine("AES.Head:" + Library.Encrypt.AES.Head);
            Console.WriteLine("Exclude:" + Exclude);
            Console.WriteLine("Platform:" + Platform);
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

        public string folder { get; protected set; }
        public int highVersion { get; protected set; }
        public int lowVersion { get; protected set; }

        public virtual void Run()
        {

        }

        public void Common(string targetDir, Dictionary<string, FileDetailInfo> cache)
        {
            Console.WriteLine("可更新文件总大小为{0}B！", cache.Values.Sum(p => p.content_size.AsInt()).ToString("N"));
            WriteToTxt(targetDir, cache);
            PathToMd5(folder, targetDir, cache);
            MakAESEncrypt(folder, targetDir, cache);
            MakeFolder(folder, targetDir);
            EndCmd();
        }

        /// <summary>
        /// 排除预定义
        /// </summary>
        /// <param name="cache"></param>
        protected bool ExcludeFile(Dictionary<string, FileDetailInfo> cache)
        {
            Console.WriteLine("");

            //排除预定义的后缀
            var array = Exclude.Split(',').Where(p => !string.IsNullOrEmpty(p)).ToArray();
            if (array.Length > 0)
            {
                var deleteKey = new List<string>();
                foreach (var s in cache)
                {
                    foreach (var s1 in array)
                    {
                        string extension = Path.GetExtension(s.Key);
                        if (string.IsNullOrEmpty(extension) || extension == s1)
                            deleteKey.Add(s.Key);
                    }
                }
                foreach (var key in deleteKey)
                {
                    cache.Remove(key);
                }
            }

            //排除非选择的平台
            array = Platform.Split('|', ',');
            if (array.Length > 1)
            {
                var deleteKey = new List<string>();
                foreach (var s in cache)
                {
                    foreach (var s1 in array.Skip(1).Where(p => !p.Equals(array.First())))
                    {
                        if (s.Key.ToLower().Contains(s1))
                            deleteKey.Add(s.Key);
                    }
                }
                foreach (var key in deleteKey)
                {
                    cache.Remove(key);
                }
            }

            var count = cache.Values.Count(p => !p.action.Equals("D"));
            if (count == 0)
            {
                Console.WriteLine("可更新文件数目为零！");
                Console.WriteLine("按任意键退出！");
                return false;
            }
            else
            {
                Console.WriteLine("可更新文件数目为{0}！", count);
                Console.WriteLine("按任意键继续！");
                Console.ReadKey();
                return true;
            }
        }

        /// <summary>
        /// 获取文件大小以及MD5
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="svnFileInfo"></param>
        protected void SetContent(string fullPath, FileDetailInfo svnFileInfo)
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
        protected void PathToMd5(string dir, string targetDir, Dictionary<string, FileDetailInfo> cache)
        {
            if (dir == null) return;
            Console.Write("\n是否将路径MD5化（y/n）：");
            bool yes = Console.ReadLine() == "y";

            var targetMd5Dir = targetDir.Replace(dir, Library.Encrypt.MD5.Encrypt(dir + KeyMd5));
            DeleteInfo(targetMd5Dir);

            if (!yes) return;

            int index = 0;
            foreach (var s in cache)
            {
                Console.WriteLine();
                Console.WriteLine("正在转化中...{0}", ((float) (++index)/cache.Count).ToString("P"));
                Console.WriteLine("is now: {0}", s.Key);
                Console.WriteLine();

                string fullPath = targetDir + "/" + s.Key;
                string targetFullPath = targetMd5Dir + "/";
                if (PathToMd5Depth == 0)
                {
                    s.Value.path_md5 = Library.Encrypt.MD5.Encrypt(s.Key + KeyMd5);
                }
                else
                {
                    var dirD = Path.GetDirectoryName(s.Key);
                    var nameD = Path.GetFileName(s.Key);
                    s.Value.path_md5 = Library.Encrypt.MD5.Encrypt(dirD + KeyMd5);
                    s.Value.path_md5 += "/";
                    s.Value.path_md5 += Library.Encrypt.MD5.Encrypt(nameD + KeyMd5);
                }
                targetFullPath += s.Value.path_md5;
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
        protected void MakAESEncrypt(string dir, string targetDir, Dictionary<string, FileDetailInfo> cache)
        {
            if (dir == null) return;
            Console.Write("\n是否对文件夹内每个文件进行加密（y/n）：");
            bool yes = Console.ReadLine() == "y";
            if (!yes) return;
            var targetMd5Dir = targetDir.Replace(dir, Library.Encrypt.MD5.Encrypt(dir + KeyMd5));
            bool md5 = Directory.Exists(targetMd5Dir);

            int index = 0;
            foreach (var s in cache)
            {
                Console.Clear();
                Console.WriteLine("\n正在加密文件...");
                Console.WriteLine("根据项目大小时间长短不定，请耐心等待...");
                Console.WriteLine("正在加密中...{0}", ((float) (++index)/cache.Count).ToString("P"));
                Console.WriteLine("is now: {0}", s.Key);
                Console.WriteLine();

                string fullPath = "";
                if (!md5)
                    fullPath = targetDir + "/" + s.Key;
                else
                    fullPath = targetMd5Dir + "/" + Library.Encrypt.MD5.Encrypt(s.Key + KeyMd5);
                if (!File.Exists(fullPath)) continue;
                var content = Library.Encrypt.AES.Encrypt(File.ReadAllText(fullPath));
                s.Value.encrypt_hash = Library.Encrypt.MD5.Encrypt(content);
                s.Value.encrypt_size = new System.IO.FileInfo(fullPath).Length.ToString();
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
            string message = Library.Compress.DecompressUtils.MakeZipFile(dir, 9);
            if (string.IsNullOrEmpty(message))
                Console.WriteLine("文件压缩成功！");
            else
                Console.WriteLine("文件压缩失败！" + message);
            DeleteInfo(dir, true);
        }

        #region

        protected void WriteToTxt(string fileName, Dictionary<string, FileDetailInfo> cache)
        {
            fileName += string.IsNullOrEmpty(Path.GetExtension(fileName)) ? Extension : "";
            File.WriteAllText(fileName, LitJson.JsonMapper.ToJson(cache.Values.ToArray()), TxTEncoding);
        }

        protected void WriteToTxt(string fileName, VersionInfo svnInfo)
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
            try
            {
                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);
            }
            catch (Exception)
            {
                Console.WriteLine("删除文件夹失败！{0}", dir);
                throw;
            }
            if (onlyDir)
                return;
            try
            {
                if (File.Exists(dir + ".txt"))
                    File.Delete(dir + ".txt");
            }
            catch (Exception)
            {
                Console.WriteLine("删除文件失败！{0}", dir + ".txt");
                throw;
            }

        }

        #endregion

        /// <summary>
        /// 最后一步形成补丁列表
        /// </summary>
        protected void UpdatePathList()
        {
            var array = Directory.GetFileSystemEntries(Environment.CurrentDirectory, "*-*-*-*");
            if (array.Length == 0) return;
            var dic = array.ToLookup(Path.GetFileNameWithoutExtension)
                .ToDictionary(p => p.Key, q => new List<string>(q));
            if (dic.Count == 0) return;
            List<FilePatchInfo> svnPatchInfos = new List<FilePatchInfo>();

            Console.Write("\n是否对文件进行加密（y/n），然后回车：");
            bool yes = Console.ReadLine() == "y";

            foreach (KeyValuePair<string, List<string>> pair in dic)
            {
                FilePatchInfo svnPatchInfo = new FilePatchInfo();
                svnPatchInfos.Add(svnPatchInfo);
                svnPatchInfo.path = pair.Key;

                var txt = pair.Value.FirstOrDefault(p => p.EndsWith(".txt"));
                var zip = pair.Value.FirstOrDefault(p => p.EndsWith(".zip"));

                if (txt != null && File.Exists(txt))
                {
                    if (yes) EncryptFile(txt);
                    svnPatchInfo.content_hash = Library.Encrypt.MD5.Encrypt(File.ReadAllBytes(txt));
                    svnPatchInfo.content_size = new System.IO.FileInfo(txt).Length.ToString();
                }
                if (zip != null && File.Exists(zip))
                {
                    svnPatchInfo.zip_hash = Library.Encrypt.MD5.Encrypt(File.ReadAllBytes(zip));
                    svnPatchInfo.zip_size = new System.IO.FileInfo(zip).Length.ToString();
                }

                var xx = pair.Key.Split('-');
                svnPatchInfo.group = xx.First();
                svnPatchInfo.firstVersion = xx.Skip(1).First().AsInt();
                svnPatchInfo.lastVersion = xx.Skip(2).First().AsInt();
            }

            WriteToTxt(Name, new VersionInfo()
            {
                softwareVersion = softwareVersion,
                pathInfos = svnPatchInfos.OrderBy(p => p.group).ThenBy(p => p.firstVersion).ToList()
            });
            if (yes) EncryptFile(Name);
        }
    }
}
