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
    /// 比较Excel找出差异并导出到文件
    /// </summary>
    public class CompareExcel
    {
        public CompareExcel()
        {
            var dir1 = SystemConsole.GetInputStr("请拖入目标文件(.xls|xlsx):", "您选择的文件：");
            var dir2 = SystemConsole.GetInputStr("请拖入比较文件(.xls|xlsx):", "您选择的文件：");

            var vals1 = ExcelByBase.Data.ImportToListTable(dir1).ToDictionary(p => p.TableName);
            var vals2 = ExcelByBase.Data.ImportToListTable(dir2).ToDictionary(p => p.TableName);

            foreach (var keyValuePair in vals1)
            {
                ListTable lst;
                if (vals2.TryGetValue(keyValuePair.Key, out lst))
                {
                    var exceptList = keyValuePair.Value.List.Except(lst.List).ToList();
                    var intersectList = keyValuePair.Value.List.Intersect(lst.List).ToList();
                    var newName = Path.ChangeExtension(dir1, "").Trim('.') + "-" + Path.GetFileNameWithoutExtension(dir2);
                    keyValuePair.Value.List = exceptList;
                    ExcelByBase.Data.ExportToExcel(keyValuePair.Value, string.Format("{0}-except.xlsx", newName));
                    keyValuePair.Value.List = intersectList;
                    ExcelByBase.Data.ExportToExcel(keyValuePair.Value, string.Format("{0}-intersect.xlsx", newName));
                }
                else
                {
                    Console.WriteLine(dir2 + " 不存在的sheet：" + keyValuePair.Key);
                }
            }
            Console.WriteLine("完毕");
        }
    }
}