using System;
using System.Collections.Generic;
using System.IO;
using Library.Excel;

namespace Script
{
    public class ActionJson : ActionBase
    {

        public class ToCsv : ActionJson
        {
            public ToCsv()
            {
                List<string> files = CheckPath(".json");
                if (files.Count == 0) return;
                files.ForEach(file =>
                {
                    Console.WriteLine(" is now : " + file);
                    ExcelByNpoi.ConvertDataTableToCsv(
                        ExcelByNpoi.ConvertListToDataTable(ExcelByNpoi.ConvertJsonToListByPath(file)));
                });
            }
        }

        public class ToExcel : ActionJson
        {
            public ToExcel()
            {
                List<string> files = CheckPath(".json");
                if (files.Count == 0) return;
                files.ForEach(file =>
                {
                    Console.WriteLine(" is now : " + file);
                    List<List<object>> vals = ExcelByNpoi.ConvertJsonToListByPath(file);
                    new ExcelByNpoi().WriteToExcel(Path.ChangeExtension(file, ".xls"), vals);
                });
            }
        }

        public class ToOneExcel : ActionJson
        {
            public ToOneExcel()
            {
                List<string> files = CheckPath(".json", true);
                if (files.Count == 0) return;

                var dic = new Dictionary<string, List<List<object>>>();
                files.ForEach(file =>
                {
                    Console.WriteLine(" is now : " + file);
                    dic[file] = ExcelByNpoi.ConvertJsonToListByPath(file);
                });
                new ExcelByNpoi().WriteToOneExcel(InputPath + ".xlsx", dic);
            }
        }
    }
}