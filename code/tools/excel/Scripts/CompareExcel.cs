using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Excel;
using Library.Extensions;

namespace Script
{
    /// <summary>
    /// 比较文件夹并复制出不同文件
    /// </summary>
    public class CompareExcel
    {

        private string dir1, dir2;

        public CompareExcel()
        {
            dir1 = SystemConsole.GetInputStr("请拖入选定文件(.xls|xlsx):", "您选择的文件：");
            dir2 = SystemConsole.GetInputStr("请拖入选定文件(.xls|xlsx):", "您选择的文件：");

            var vals1 = new ExcelByNpoi().ReadFromExcels(dir1);
            var vals2 = new ExcelByNpoi().ReadFromExcels(dir2);

            var index = 0;
            foreach (var keyValuePair in vals1)
            {
                var lst = new ListTable();
                if (vals2.TryGetValue(keyValuePair.Key, out lst))
                {
                    keyValuePair.Value.List = keyValuePair.Value.List.Except(lst.List).ToList();
                    var newName = Path.GetDirectoryName(dir1) + "/" + Path.GetFileNameWithoutExtension(dir1) + "-" +
                                  Path.GetFileNameWithoutExtension(dir2) + ".xlsx";
                    new ExcelByNpoi().WriteToExcel(newName, lst);
                }
                else
                {
                    Console.WriteLine(dir2 + " 不存在的sheet：" + keyValuePair.Key);
                }
            }
        }
    }
}