﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Excel;
using Library.Extensions;

namespace checkcard
{
    public class CheckScenario : BaseSystemExcel
    {
        public CheckScenario()
        {
            SystemConsole.Run(config: new Dictionary<string, Action>()
            {
                {"读取", Read},
                {"还原", Write}
            });
        }

        public void Read()
        {
            GetListTables().AsParallel().ForAll(lt =>
            {
                Console.WriteLine(" is now : " + lt.TableName);
                List<string> res = new List<string>();

                foreach (List<object> list in lt.Rows)
                {
                    string haha = list[4].ToString();
                    if (haha.Count(p => p.Equals('[') || p.Equals(']'))%2 == 0) continue;
                    res.Add(string.Join("\n", list.Select(q => q.ToString())));
                    res.Add("");
                }
                File.WriteAllLines(InputPath + ".txt", res.ToArray());
            });
        }

        public void Write()
        {
            var dts = GetListTables().ToList();

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
                var row = dts[0].Rows.FirstOrDefault(q =>
                {
                    string[] p = q.Cast<string>().ToArray();
                    return p[0] == list[0] && p[1] == list[1] && p[2] == list[2] && p[3] == list[3];
                });
                Console.WriteLine(" is now : " + row[0]);
                row[4] = list[4];
            }

            ExcelUtils.ExportToExcel(dts.First().ToDataTable(), InputPath, ".xlsx");
        }
    }
}