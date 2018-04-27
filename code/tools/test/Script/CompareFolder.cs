using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace Script
{
    /// <summary>
    /// 比较文件夹并复制出不同文件
    /// </summary>
    public class CompareFolder : BaseClass
    {

        private string dir1, dir2;

        public CompareFolder()
        {
            dir1 = SystemExtensions.GetInputStr("请拖入选定文件夹:", "您选择的文件夹：");
            dir2 = SystemExtensions.GetInputStr("请拖入选定文件夹:", "您选择的文件夹：");
            var last1 = Directory.GetFiles(dir1, "*.*", SearchOption.AllDirectories).Select(p => p.Replace(dir1, ""));
            var last2 = Directory.GetFiles(dir2, "*.*", SearchOption.AllDirectories).Select(p => p.Replace(dir2, ""));
            var last = last1.Except(last2).Select(p => dir1 + p).ToList();
            RunList(last);
            new CreateExcelCell(dir1 + "res");
        }

        public override void RunListOne(string re)
        {
            var newName = re.Replace(dir1, dir1 + "res");
            FileHelper.CreateDirectory(newName);
            File.Copy(re, newName, true);
        }
    }
}