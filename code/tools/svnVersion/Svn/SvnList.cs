using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileVersion;
using Library;
using Library.Extensions;
using Library.Helper;
using Library.LitJson;

namespace SvnVersion
{
    public class SvnList : CommonBase
    {
        public string svnUrl { get; protected set; }
        public string svnUserName { get; protected set; }
        public string svnPassword { get; protected set; }

        public override string SaveName
        {
            get { return SaveDir + highRevision + "/c_resource.json"; }
        }

        public override string[] CmdReadAll(string input)
        {
            if (string.IsNullOrEmpty(svnUserName) || string.IsNullOrEmpty(svnPassword))
                return base.CmdReadAll(input);
            string newInput = string.Format("{0} --username {1} --password {2}", input, svnUserName, svnPassword);
            return base.CmdReadAll(newInput);
        }

        public SvnList()
        {
            svnUrl = Config.IniReadValue("Svn", "url", "").Trim();
            svnUserName = Config.IniReadValue("Svn", "username", "").Trim();
            svnPassword = Config.IniReadValue("Svn", "password", "").Trim();
            Console.WriteLine("url:" + svnUrl);
            Console.WriteLine("username:" + svnUserName);
            Console.WriteLine("password:" + svnPassword);
            Console.WriteLine("--------------------------------------");

            StartCmd();
            softwareVersion = CmdReadAll("svn --version --quiet").Last();
            isInstall = softwareVersion.Replace(".", "").AsInt() != 0;
            if (isInstall)
                Console.WriteLine("SVN版本：" + softwareVersion);
            else
                Console.WriteLine("未安装Svn命令行工具，请先安装！");
        }

        public override void Run()
        {
            try
            {
                svnUrl += svnUrl.EndsWith("/") ? "" : "/";
                Console.WriteLine("库地址：" + svnUrl);

                SaveDir = Path.GetDirectoryName(svnUrl).AsStringArray('\\').Last() + "/";

                Console.WriteLine("");
                highVersion = CmdReadAll("svn info --show-item last-changed-revision " + svnUrl).Last().AsInt();
                Console.WriteLine("最高版本号：" + highVersion);

                var logs = CmdReadAll(string.Format("svn log -r 0:{0} \"{1}@{0}\" -q -l2 --stop-on-copy", highVersion, svnUrl));
                lowVersion = logs.Skip(1).First().Split('|').First().Replace("r", "").Trim().AsInt();
                Console.WriteLine("最低版本号：" + lowVersion);
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                SystemConsole.QuitReadKey("请判断远程库连接是否正确！" + e.Message);
            }

            Dictionary<string, FileDetailInfo> oldCache = new Dictionary<string, FileDetailInfo>();

            if (!Directory.Exists(SaveDir))
            {
                Directory.CreateDirectory(SaveDir);
            }
            var list = Directory.GetDirectories(SaveDir, "*.*", SearchOption.TopDirectoryOnly).ToList();
            highRevision = list.Count == 0 ? 0 : list.Max(p => Path.GetFileName(p).AsLong());

            if (File.Exists(SaveName))
            {
                //根据已有查找到最高修订版本版本号以及列表
                var content = File.ReadAllText(SaveName).Trim('\0');
                oldCache = LitJsonHelper.ToObject<FileDetailInfo[]>(content).ToDictionary(p => p.path);
                //highRevision = oldCache.Count == 0 ? lowVersion : oldCache.Values.Max(p => p.revision);
            }
            highRevision = highVersion;
            File.WriteAllText(SaveRevisionName, highRevision.ToString());

            //获取当前最高版本号的list列表
            Dictionary<string, FileDetailInfo> newCache = GetListCache(highVersion);
            Dictionary<string, FileDetailInfo> cache = CompareCache(newCache, oldCache);

            if (OutUpdate(cache))
            {
                WriteToTxt(newCache);
                ExportFile(cache);
                Console.WriteLine("可更新文件总大小为{0}B！", cache.Values.Sum(p => p.content_size).ToString("N"));
                WriteToTxt(newCache);
                MakAESEncrypt(cache);
                WriteToTxt(newCache);
                PathToMd5(cache);
                WriteToTxt(newCache);
            }
            EndCmd();
        }

        protected bool OutUpdate(Dictionary<string, FileDetailInfo> cache)
        {
            var count = cache.Values.Count(p => !p.is_delete);
            if (count == 0)
            {
                Console.WriteLine("可更新文件数目为零！");
                SystemConsole.QuitReadKey();
                return false;
            }
            else
            {
                Console.WriteLine("可更新文件数目为{0}！", count);
                SystemConsole.ContinueReadKey();
            }
            return true;
        }

        private void ExportFile(Dictionary<string, FileDetailInfo> cache)
        {
            if (SystemConsole.GetInputStr("\n是否导出目标版本号文件（y/n）：", "", "y") == "y")
            {
                List<string> del = new List<string>();
                int index = 0;
                foreach (KeyValuePair<string, FileDetailInfo> pair in cache)
                {
                    Console.Clear();
                    Console.WriteLine("\n正在导出文件...{0}", highVersion);
                    Console.WriteLine("根据项目大小时间长短不定，请耐心等待...");
                    Console.WriteLine("正在导出中...{0}", ((float) (++index)/cache.Count).ToString("P"));
                    Console.WriteLine("is now: {0} v:{1} r:{2}", pair.Key, pair.Value.version, pair.Value.revision);
                    Console.WriteLine();

                    string fullPath = Environment.CurrentDirectory.Replace("\\", "/") + "/" + SaveDir +
                                      pair.Value.revision + "/" + pair.Key;
                    FileHelper.CreateDirectory(fullPath);

                    //拉取的文件版本号不会小于所在目录版本号，如若小于，说明文件所在目录曾经被移动过
                    CmdReadAll(string.Format("svn cat -r {0} \"{1}{2}@{0}\">\"{3}\"", highRevision, svnUrl, pair.Key,
                        fullPath));
                    //RunCmd(string.Format("svn cat -r {0} \"{1}/{2}@{0}\">\"{3}\"", s.Value.version, svnUrl, s.Key, fullPath));

                    if (File.Exists(fullPath))
                    {
                        var bytes = File.ReadAllBytes(fullPath);
                        pair.Value.content_size = bytes.Length;
                        pair.Value.content_hash = Encrypt.MD5(bytes);
                        Console.WriteLine();
                    }
                    else
                    {
                        del.Add(pair.Key);
                    }
                }
                foreach (string s in del)
                    cache.Remove(s);
            }
        }

        private Dictionary<string, FileDetailInfo> GetListCache(long version)
        {
            Console.WriteLine("\n正在获取目标版本号{0}文件详细信息...", version);

            var targetList =
                CmdReadAll(string.Format("svn list -r {0} {1}@{0} -R -v", version, svnUrl))
                    .Where(s => !s.EndsWith("/"))
                    .ToArray(); //去除文件夹

            Dictionary<string, FileDetailInfo> cache = new Dictionary<string, FileDetailInfo>();
            int index = 0;
            foreach (string s in targetList)
            {
                List<string> res = s.Split(' ').Where(s1 => !string.IsNullOrEmpty(s1)).ToList();
                var last = res.Skip(6).ToArray().JoinToString(" ").Replace("\\", "/").Trim();
                FileDetailInfo svnFileInfo = new FileDetailInfo()
                {
                    is_delete = false,
                    version = res.First().Trim().AsLong(),
                    content_size = res.Skip(2).First().Trim().AsLong(),
                    revision = version,
                    path = last,
                };
                cache[svnFileInfo.path] = svnFileInfo;
                Console.WriteLine("{0:D5}\t{1}", ++index, svnFileInfo);
            }
            ExcludeFile(cache);
            return cache;
        }

        private static Dictionary<string, FileDetailInfo> CompareCache(Dictionary<string, FileDetailInfo> newCache, Dictionary<string, FileDetailInfo> oldCache)
        {
            //变动列表
            Dictionary<string, FileDetailInfo> cache = new Dictionary<string, FileDetailInfo>();
            int index = 0;
            foreach (KeyValuePair<string, FileDetailInfo> pair in newCache)
            {
                FileDetailInfo oldFileDetailInfo = null;
                if (oldCache.TryGetValue(pair.Key, out oldFileDetailInfo))
                {
                    if (pair.Value.version == oldFileDetailInfo.version)
                    {
                        //未更新的
                        pair.Value.revision = oldFileDetailInfo.revision;
                    }
                    else
                    {
                        //更新的
                        cache[pair.Key] = pair.Value;
                    }
                }
                else
                {
                    //增加的
                    cache[pair.Key] = pair.Value;
                }
                Console.WriteLine("{0:D5}\t{1}", ++index, pair.Value);
            }
            return cache;
        }
    }
}
