using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library;
using Library.Extensions;
using Library.Helper;
using Library.LitJson;

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

        protected static string KeyMd5 { get; private set; }
        protected static string AESKey { get; private set; }
        protected static string AESHead { get; private set; }
        protected static string Exclude { get; private set; }
        protected static string Platform { get; private set; }
        protected static string EncryptExclude { get; private set; }
        protected static string EncryptRootDir { get; private set; }

        static CommonBase()
        {
            KeyMd5 = Config.IniReadValue("Config", "md5key", "").Trim();
            AESKey = Config.IniReadValue("Config", "aeskey", "YmbEV0FVzZN/SvKCCoJje/jSpM").Trim();
            AESHead = Config.IniReadValue("Config", "aeshead", "JKRihFwgicIzkBPEyyEn9pnpoANbyFuplHl").Trim();
            Exclude = Config.IniReadValue("Config", "exclude", ".meta").Trim();
            Platform = Config.IniReadValue("Config", "platform", "ios|android,ios,pc").Trim();

            EncryptExclude = Config.IniReadValue("Encrypt", "excludeFile", ".meta,.acb").Trim();
            EncryptRootDir = Config.IniReadValue("Encrypt", "excludeDir", "cri/").Trim();
        }

        public CommonBase()
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("KeyMd5:" + KeyMd5);
            Console.WriteLine("AES.Key:" + AESKey);
            Console.WriteLine("AES.Head:" + AESHead);
            Console.WriteLine("Exclude:" + Exclude);
            Console.WriteLine("Platform:" + Platform);
            Console.WriteLine("EncryptExclude:" + EncryptExclude);
            Console.WriteLine("EncryptRootDir:" + EncryptRootDir);
            Console.WriteLine("--------------------------------------");
        }

        public string SaveDir
        {
            get { return "Version/"; }
        }

        public virtual string Name
        {
            get { return SaveDir; }
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
        public long highVersion { get; protected set; }
        public long lowVersion { get; protected set; }
        public long startVersion { get; protected set; }
        public long endVersion { get; protected set; }

        public virtual void Run()
        {

        }

        public virtual void Common(string targetDir, Dictionary<string, FileDetailInfo> cache)
        {
            Console.WriteLine("可更新文件总大小为{0}B！", cache.Values.Sum(p => p.content_size).ToString("N"));
            WriteToTxt(targetDir, cache);
            MakAESEncrypt(targetDir, cache);
            WriteToTxt(targetDir, cache);
            PathToMd5(folder, targetDir, cache);
            WriteToTxt(targetDir, cache);
            MakeFolder(folder, targetDir);
            WriteToTxt(targetDir, cache);
            EndCmd();
        }

        /// <summary>
        /// 排除预定义
        /// </summary>
        /// <param name="cache"></param>
        protected void ExcludeFile(Dictionary<string, FileDetailInfo> cache)
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

            OutUpdate(cache);
        }

        protected void OutUpdate(Dictionary<string, FileDetailInfo> cache)
        {
            var count = cache.Values.Count(p => !p.is_delete);
            if (count == 0)
            {
                Console.WriteLine("可更新文件数目为零！");
                SystemConsole.QuitReadKey();
            }
            else
            {
                Console.WriteLine("可更新文件数目为{0}！", count);
                SystemConsole.ContinueReadKey();
            }
        }

        /// <summary>
        /// 获取文件大小以及MD5
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="fileDetailInfo"></param>
        protected void SetContent(string fullPath, FileDetailInfo fileDetailInfo)
        {
            var bytes = File.ReadAllBytes(fullPath);
            fileDetailInfo.content_size = bytes.Length;
            fileDetailInfo.content_hash = bytes.MD5(KeyMd5);
        }

        protected static bool TargetMd5Dir(out string targetMd5Dir, string dir, string targetDir)
        {
            targetMd5Dir = targetDir.Replace(dir, dir.MD5(KeyMd5));
            bool md5 = Directory.Exists(targetMd5Dir);
            return md5;
        }

        protected string GetPathHash(string path)
        {
            return Path.GetDirectoryName(path).MD5(KeyMd5) + "/" + Path.GetFileName(path).MD5(KeyMd5);
        }

        /// <summary>
        /// 路径MD5化
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="targetDir"></param>
        /// <param name="cache"></param>
        protected void PathToMd5(string dir, string targetDir, Dictionary<string, FileDetailInfo> cache)
        {
            if (SystemConsole.GetInputStr("\n是否将路径MD5化（y/n）：", "", "y") == "y")
            {
                if (dir == null) return;
                string targetMd5Dir;
                var isHaveMd5Dir = TargetMd5Dir(out targetMd5Dir, dir, targetDir);
                DeleteInfo(targetMd5Dir);

                int index = 0;
                foreach (var s in cache)
                {
                    Console.WriteLine();
                    Console.WriteLine("正在转化中...{0}", ((float) (++index)/cache.Count).ToString("P"));
                    Console.WriteLine("is now: {0}", s.Key);
                    Console.WriteLine();

                    string fullPath = targetDir + "/" + s.Key;
                    string targetFullPath = targetMd5Dir + "/" + GetPathHash(s.Key);
                    if (!File.Exists(fullPath)) continue;
                    FileHelper.CreateDirectory(targetFullPath);
                    File.Copy(fullPath, targetFullPath, true);
                }

                DeleteInfo(targetDir);
                WriteToTxt(targetMd5Dir, cache);
            }
        }

        /// <summary>
        /// 每一个文件进行加密
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="cache"></param>
        protected void MakAESEncrypt(string targetDir, Dictionary<string, FileDetailInfo> cache)
        {
            if (SystemConsole.GetInputStr("\n是否对文件夹内每个文件进行加密（y/n）：", "", "y") == "y")
            {
                int index = 0;
                foreach (var s in cache)
                {
                    Console.Clear();
                    Console.WriteLine("\n正在加密文件...");
                    Console.WriteLine("根据项目大小时间长短不定，请耐心等待...");
                    Console.WriteLine("正在加密中...{0}", ((float) (++index)/cache.Count).ToString("P"));
                    Console.WriteLine("is now: {0}", s.Key);
                    Console.WriteLine();

                    string fullPath = targetDir + "/" + s.Key;
                    if (!File.Exists(fullPath)) continue;

                    var array = EncryptExclude.Split(',').Where(p => !string.IsNullOrEmpty(p)).ToArray();
                    if (array.Contains(Path.GetExtension(s.Key))) continue;

                    array = EncryptRootDir.Split(',').Where(p => !string.IsNullOrEmpty(p)).ToArray();
                    if (array.Any(p => s.Key.StartsWith(p))) continue;

                    var content = File.ReadAllText(fullPath).AES_Encrypt(AESKey);
                    s.Value.encrypt_hash = content.MD5(KeyMd5);
                    s.Value.encrypt_size = content.Length;
                    File.WriteAllText(fullPath, content);
                }
            }
        }

        /// <summary>
        /// 文件夹压缩
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="targetDir"></param>
        protected void MakeFolder(string dir, string targetDir)
        {
            if (dir == null) return;
            bool yes = SystemConsole.GetInputStr("\n是否将文件夹压缩（y/n）：", "", "y") == "y";
            if (!yes) return;

            string targetMd5Dir;
            var isHaveMd5Dir = TargetMd5Dir(out targetMd5Dir, dir, targetDir);
            var dirZip = (!isHaveMd5Dir ? targetDir : targetMd5Dir);

            string message = Library.Compress.DecompressUtils.MakeZipFile(dirZip, 9);
            if (string.IsNullOrEmpty(message))
                Console.WriteLine("文件压缩成功！");
            else
                Console.WriteLine("文件压缩失败！" + message);
            DeleteInfo(dirZip, true);
        }

        #region

        protected void WriteToTxt(string fileName, Dictionary<string, FileDetailInfo> cache)
        {
            fileName += string.IsNullOrEmpty(Path.GetExtension(fileName)) ? Extension : "";
            FileHelper.CreateDirectory(fileName);
            File.WriteAllText(fileName, LitJsonHelper.ToJson(cache.Values.ToArray()), TxTEncoding);
        }

        protected void WriteToTxt(string fileName, VersionInfo versionInfo)
        {
            fileName += string.IsNullOrEmpty(Path.GetExtension(fileName)) ? Extension : "";
            FileHelper.CreateDirectory(fileName);
            File.WriteAllText(fileName, LitJsonHelper.ToJson(versionInfo), TxTEncoding);
        }

        protected void EncryptFile(string fileName)
        {
            fileName += string.IsNullOrEmpty(Path.GetExtension(fileName)) ? Extension : "";
            File.WriteAllText(fileName, File.ReadAllText(fileName).AES_Encrypt(AESKey), TxTEncoding);
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
            var array = Directory.GetFileSystemEntries(Environment.CurrentDirectory.Replace("\\", "/") + "/" + SaveDir,
                "*-*-*-*");
            if (array.Length == 0) return;
            var dic = array.ToLookup(Path.GetFileNameWithoutExtension)
                .ToDictionary(p => p.Key, q => new List<string>(q));
            if (dic.Count == 0) return;
            List<FilePatchInfo> svnPatchInfos = new List<FilePatchInfo>();

            bool yes = SystemConsole.GetInputStr("\n是否对文件进行加密（y/n），然后回车：", "", "y") == "y";

            foreach (KeyValuePair<string, List<string>> pair in dic)
            {
                FilePatchInfo filePatchInfo = new FilePatchInfo();
                svnPatchInfos.Add(filePatchInfo);
                filePatchInfo.path = pair.Key;

                var txt = pair.Value.FirstOrDefault(p => p.EndsWith(".txt"));
                var zip = pair.Value.FirstOrDefault(p => p.EndsWith(".zip"));

                if (txt != null && File.Exists(txt))
                {
                    filePatchInfo.content_hash = File.ReadAllBytes(txt).MD5(KeyMd5);
                    filePatchInfo.content_size = new FileInfo(txt).Length;
                    if (yes) EncryptFile(txt);
                    filePatchInfo.encrypt_hash = File.ReadAllBytes(txt).MD5(KeyMd5);
                    filePatchInfo.encrypt_size = new FileInfo(txt).Length;
                }
                if (zip != null && File.Exists(zip))
                {
                    filePatchInfo.zip_hash = File.ReadAllBytes(zip).MD5(KeyMd5);
                    filePatchInfo.zip_size = new FileInfo(zip).Length;
                }

                var xx = pair.Key.Split('-');
                filePatchInfo.group = xx.First();
                filePatchInfo.firstVersion = xx.Skip(1).First().AsInt();
                filePatchInfo.lastVersion = xx.Skip(2).First().AsInt();
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
