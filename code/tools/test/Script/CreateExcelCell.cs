using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Helper;

namespace Script
{
    /// <summary>
    /// 生成excel插入用的格式
    /// </summary>
    public class CreateExcelCell : BaseClass
    {
        private Dictionary<string, string> dic = new Dictionary<string, string>();

        public CreateExcelCell()
        {
            //root = "D:/Work/mfxy/资料/小圆/xy/mfxy/res/";
            //root = "D:/Work/yuege/res/app/assets/app/daifanyi/";
            //root = "D:/Work/yuege/res/app/assets/app/daifanyi/image_new/";

            {
                //root = @"D:\Work\mfxy\ron_mfsn2\banshu\madomagi_native\Resources\package".Replace("\\", "/");
                var newarray = ReadAllLines("new");
                var oldarray = ReadAllLines("old");
                var res = newarray.Except(oldarray).Where(p => !string.IsNullOrEmpty(p)).Select(p => root + p).ToList();
                res.Sort();
                RunList(res);
            }

            WriteAllLines(dic);
        }

        public CreateExcelCell(string root)
        {
            this.root = root;
            var res =
                Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                    .Where(p => !string.IsNullOrEmpty(p))
                    .Select(p => p.Replace("\\","/"))
                    .ToList();
            res.Sort();
            RunList(res);
            WriteAllLines(dic);
        }


        public override void RunListOne(string re)
        {
            try
            {
                dic[re.Replace(root, "")] = GetExcelCell(re);
                //var newName = @"D:\Work\mfxy\资料\小圆\xy\mfxy\res" + re.Replace(root, "");
                //FileHelper.CreateDirectory(newName);
                //File.Copy(re, newName);
            }
            catch (Exception e)
            {
                Console.WriteLine(re + "\t" + e.Message);
            }
        }
    }
}