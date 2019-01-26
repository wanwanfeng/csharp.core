using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace Script
{
    /// <summary>
    /// 生成excel插入用的格式
    /// </summary>
    public class CreateExcelCell : BaseClass
    {
        public CreateExcelCell()
        {
            CreateExcel();
        }

        protected virtual void CreateExcel()
        {
            var dir1 = SystemConsole.GetInputStr("请拖入选定文件:", "您选择的文件：", def: "new.txt");
            var dir2 = SystemConsole.GetInputStr("请拖入选定文件:", "您选择的文件：", def: "old.txt");
            var last1 = File.ReadAllLines(dir1);
            var last2 = File.ReadAllLines(dir2);
            last1.Except(last2)
                .Distinct()
                .Select(p => dir1 + p)
                .ToList()
                .ForEachPaths(re =>
                {
                    var newName = re.Replace(dir1, dir1 + "res");
                    DirectoryHelper.CreateDirectory(newName);
                    File.Copy(re, newName, true);
                });

            var root = dir1 + "res";
            RunList(Directory.GetFiles(root, "*.*", SearchOption.AllDirectories), root);
        }

        protected void RunList(IEnumerable<string> res, string root)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            res.Where(p => !string.IsNullOrEmpty(p))
                .ToList()
                .ForEachPaths(re =>
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
                });
            WriteAllLines(dic, root);
        }
    }
}