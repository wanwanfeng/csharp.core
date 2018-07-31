using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileVersion;
using Library;
using Library.Extensions;
using Library.Helper;
using Library.LitJson;

namespace SvnVersion
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

        public string SaveDir { get; set; }

        public virtual string SaveName
        {
            get { return SaveDir + "c_resource.json"; }
        }
        public string SaveRevisionName
        {
            get { return SaveDir + "c_revision.json"; }
        }
        public string Extension
        {
            get { return ".txt"; }
        }

        public Encoding TxTEncoding
        {
            get { return new UTF8Encoding(false); }
        }

        public long highRevision { get; protected set; }
        public long highVersion { get; protected set; }
        public long lowVersion { get; protected set; }

        public virtual void Run()
        {

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

            //OutUpdate(cache);
        }

        protected string GetPathHash(string path)
        {
            return Path.GetDirectoryName(path).MD5(KeyMd5) + "/" + Path.GetFileName(path).MD5(KeyMd5);
        }

        /// <summary>
        /// 每一个文件进行加密
        /// </summary>
        /// <param name="cache"></param>
        protected void MakAESEncrypt(Dictionary<string, FileDetailInfo> cache)
        {
            if (SystemConsole.GetInputStr("\n是否对文件夹内每个文件进行加密（y/n）：", "", "y") == "y")
            {
                int index = 0;
                foreach (var pair in cache)
                {
                    Console.Clear();
                    Console.WriteLine("\n正在加密文件...");
                    Console.WriteLine("根据项目大小时间长短不定，请耐心等待...");
                    Console.WriteLine("正在加密中...{0}", ((float) (++index)/cache.Count).ToString("P"));
                    Console.WriteLine("is now: {0}", pair.Key);
                    Console.WriteLine();

                    //文件后缀忽略
                    var array = EncryptExclude.Split(',').Where(p => !string.IsNullOrEmpty(p)).ToArray();
                    if (array.Contains(Path.GetExtension(pair.Key))) continue;

                    //文件根目录忽略
                    array = EncryptRootDir.Split(',').Where(p => !string.IsNullOrEmpty(p)).ToArray();
                    if (array.Any(p => pair.Key.StartsWith(p))) continue;

                    string targetDir = Environment.CurrentDirectory.Replace("\\", "/") + "/" + SaveDir +
                                       pair.Value.revision;
                    string fullPath = targetDir + "/" + pair.Key;
                    if (!File.Exists(fullPath)) continue;

                    var content = File.ReadAllText(fullPath).AES_Encrypt(AESKey);
                    pair.Value.encrypt_hash = content.MD5(KeyMd5);
                    pair.Value.encrypt_size = content.Length;
                    File.WriteAllText(fullPath, content);
                }
            }
        }

        /// <summary>
        /// 路径MD5化
        /// </summary>
        /// <param name="cache"></param>
        protected void PathToMd5(Dictionary<string, FileDetailInfo> cache)
        {
            if (SystemConsole.GetInputStr("\n是否将路径MD5化（y/n）：", "", "y") == "y")
            {
                int index = 0;
                foreach (var s in cache)
                {
                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine("正在转化中...{0}", ((float)(++index) / cache.Count).ToString("P"));
                    Console.WriteLine("is now: {0}", s.Key);
                    Console.WriteLine();

                    string targetDir = Environment.CurrentDirectory.Replace("\\", "/") + "/" + SaveDir +
                                       s.Value.revision;
                    string fullPath = targetDir + "/" + s.Key;
                    string targetFullPath = targetDir + "/" + GetPathHash(s.Key);

                    if (!File.Exists(fullPath)) continue;
                    FileHelper.CreateDirectory(targetFullPath);
                    File.Copy(fullPath, targetFullPath, true);
                }
            }
        }

        #region

        protected virtual void WriteToTxt(Dictionary<string, FileDetailInfo> cache)
        {
            var fileName = SaveName;
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

        #endregion
    }
}
