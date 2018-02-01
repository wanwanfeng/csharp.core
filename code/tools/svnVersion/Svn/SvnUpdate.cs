using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;

namespace SvnVersion
{
    public class SvnUpdate : SvnCommon
    {
        public override string Name
        {
            get { return "patch-list"; }
        }

        public override void Run()
        {
            StartCmd();
            svnVersion = RunCmd("svn --version --quiet").Last();
            Console.WriteLine("SVN版本：" + svnVersion);
            EndCmd();

            var array = Directory.GetFileSystemEntries(Environment.CurrentDirectory, "*-*-*-*");
            if (array.Length == 0) return;
            var dic = array.ToLookup(Path.GetFileNameWithoutExtension)
                .ToDictionary(p => p.Key, q => new List<string>(q));
            if (dic.Count == 0) return;
            List<SvnPatchInfo> svnPatchInfos = new List<SvnPatchInfo>();

            Console.Write("\n是否对文件进行加密（y/n），然后回车：");
            bool yes = Console.ReadLine() == "y";

            foreach (KeyValuePair<string, List<string>> pair in dic)
            {
                SvnPatchInfo svnPatchInfo = new SvnPatchInfo();
                svnPatchInfos.Add(svnPatchInfo);
                svnPatchInfo.path = pair.Key;

                var txt = pair.Value.FirstOrDefault(p => p.EndsWith(".txt"));
                var zip = pair.Value.FirstOrDefault(p => p.EndsWith(".zip"));

                if (txt != null && File.Exists(txt))
                {
                    if (yes) EncryptFile(txt);
                    svnPatchInfo.content_hash = Library.Encrypt.MD5.Encrypt(File.ReadAllBytes(txt));
                    svnPatchInfo.content_size = new FileInfo(txt).Length.ToString();
                }
                if (zip != null && File.Exists(zip))
                {
                    svnPatchInfo.zip_hash = Library.Encrypt.MD5.Encrypt(File.ReadAllBytes(zip));
                    svnPatchInfo.zip_size = new FileInfo(zip).Length.ToString();
                }

                var xx = pair.Key.Split('-');
                svnPatchInfo.group = xx.First();
                svnPatchInfo.firstVersion = xx.Skip(1).First().AsInt();
                svnPatchInfo.lastVersion = xx.Skip(2).First().AsInt();
            }

            WriteToTxt(Name, new SvnInfo()
            {
                svnVersion = svnVersion,
                svnInfos = svnPatchInfos.OrderBy(p => p.group).ThenBy(p => p.firstVersion).ToList()
            });
            if (yes) EncryptFile(Name);
        }
    }
}
