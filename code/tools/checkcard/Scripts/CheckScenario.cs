﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Excel;
using Library.Extensions;

namespace checkcard.Scripts
{
    public class CheckScenario : BaseSystemConsole
    {
        public CheckScenario()
        {
            string read = SystemConsole.GetInputStr("请输入（读取:d/还原:h）", "", "v");
            if (read == "v") return;
            if (read == "d")
            {
                Read();
                return;
            }
            if (read == "h")
            {
                Write();
                return;
            }
        }

        public void Read()
        {
            List<string> files = CheckPath(".xlsx", SelectType.File);
            if (files.Count == 0) return;

            var dts = new List<ListTable>();
            files.ForEach(file =>
            {
                Console.WriteLine(" from : " + file);
                dts.AddRange(ExcelByBase.Data.ImportToListTable(file));
            });

            if (dts.Count == 0)
                return;

            foreach (ListTable lt in dts)
            {
                Console.WriteLine(" is now : " + lt.FullName);
                if (!lt.IsArray) continue;
                List<string> res = new List<string>();

                foreach (List<object> list in lt.List)
                {
                    string haha = list[4].ToString();
                    if (haha.Count(p => p.Equals('[') || p.Equals(']'))%2 == 0) continue;
                    res.Add(string.Join("\n", list.Select(q => q.ToString())));
                    res.Add("");
                }
                File.WriteAllLines(InputPath + ".txt", res.ToArray());
            }
        }

        public void Write()
        {
            List<string> files = CheckPath(".xlsx", SelectType.File);
            if (files.Count == 0) return;

            var dts = new List<ListTable>();
            files.ForEach(file =>
            {
                Console.WriteLine(" from : " + file);
                dts.AddRange(ExcelByBase.Data.ImportToListTable(file));
            });

            if (dts.Count == 0)
                return;

            string[] content = File.ReadAllLines(InputPath + ".txt");
            List<List<string>> res = new List<List<string>>();
            for (int c = 0; c < content.Length; c += 6)
            {
                res.Add(content.Skip(c).Take(6).ToList());
            }

            foreach (List<string> list in res)
            {
                var row = dts[0].List.FirstOrDefault(q =>
                {
                    string[] p = q.Cast<string>().ToArray();
                    return p[0] == list[0] && p[1] == list[1] && p[2] == list[2] && p[3] == list[3];
                });
                Console.WriteLine(" is now : " + row[0]);
                row[4] = list[4];
            }

            ExcelByBase.Data.ExportToExcel(dts[0], InputPath + ".xlsx");
        }
    }
}