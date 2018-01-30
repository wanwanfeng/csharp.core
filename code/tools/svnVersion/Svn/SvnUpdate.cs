using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;

namespace svnVersion
{
    public class SvnUpdate : SvnCommon
    {
        public override void Run()
        {
            List<string> folder = new List<string>();
            folder.AddRange(Directory.GetDirectories(Environment.CurrentDirectory, "svn-*-master", SearchOption.TopDirectoryOnly));
            folder.AddRange(Directory.GetDirectories(Environment.CurrentDirectory, "svn-*-patch", SearchOption.TopDirectoryOnly));
            if (folder.Count == 0) return;
            List<List<string>> cao = new List<List<string>>();
            foreach (string s in folder)
            {
                List<string> res = new List<string>();
                var txt = s + ".txt";
                var zip = s + ".zip";

                if (File.Exists(txt))
                {
                    res.Add(new FileInfo(txt).Length.ToString());
                    res.Add(Library.Encrypt.MD5.Encrypt(File.ReadAllBytes(txt)));
                }
                else
                {
                    res.Add("");
                    res.Add("");
                }
                if (File.Exists(zip))
                {
                    res.Add(new FileInfo(zip).Length.ToString());
                    res.Add(Library.Encrypt.MD5.Encrypt(File.ReadAllBytes(zip)));
                    res.Add(Path.GetFileName(zip));
                }
                else
                {
                    res.Add("");
                    res.Add("");
                    res.Add(Path.GetFileNameWithoutExtension(s));
                }
                cao.Add(res);
            }

            File.WriteAllLines("patch-list.txt", cao.Select(q => q.ToArray().JoinToString(",")).ToArray(),
                new UTF8Encoding(false));
        }
    }
}
