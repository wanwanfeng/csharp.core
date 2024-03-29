﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;
using Library.Helper;

namespace Library.Excel
{
    /// <summary>
    /// DataTable与CSV
    /// </summary>
    public abstract partial class ExcelUtils
    {
        #region  Convert Csv and DataTable

        public enum CsvMode
        {
            CsvHelp,
            Normal,
        }

        public static CsvMode CurCsvMode = CsvMode.CsvHelp;

        public static DataTable ImportFromCsv(string path)
        {
            path = Path.ChangeExtension(path, ".csv");
            if (!File.Exists(path))
                throw new Exception("文件不存在!");
            if (path == null) return null;

            List<List<object>> list;

            switch (CurCsvMode)
            {
                case CsvMode.CsvHelp:
                    {
                        list = CsvHelper.ReadCSV(path);
                    }
                    break;
                case CsvMode.Normal:
                    {
                        string[] content = File.ReadAllLines(path);
                        list = content.Select(
                            q =>
                            {
                                return q.Split(',')
                                    .Select(p => p.StartsWith("\"") ? p.Substring(1, p.Length - 2) : p)
                                    .Cast<object>()
                                    .ToList();
                            })
                            .ToList();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (list.Count == 0) return null;
            return new ListTable()
            {
                TableName = Path.GetFileNameWithoutExtension(path),
                Columns = list.First().Cast<string>().ToList(),
                Rows = list.Skip(1).ToList()
            }.ToDataTable();
        }

        public static void ExportToCsv(DataTable dt, string file)
        {
            string newPath = CheckExport(dt, file, ".csv");

            var list = dt.ToListTable();
            switch (CurCsvMode)
            {
                case CsvMode.CsvHelp:
                    {
                        var res = new List<List<object>>(list.Rows);
                        res.Insert(0, list.Columns.Cast<object>().ToList());
                        CsvHelper.SaveCSV(res, newPath);
                    }
                    break;
                case CsvMode.Normal:
                    {

                        var contents = list.Rows.Select(p => string.Join(",", p.Select(q =>
                                {
                                    //var str = q.ToString().Replace("\"", "\"\""); //替换英文冒号 英文冒号需要换成两个冒号
                                    //if (str.Contains(',') || str.Contains('\"') || str.Contains('\r') || str.Contains('\n'))
                                    //{
                                    //    //含逗号 冒号 换行符的需要放到引号中
                                    //    str = string.Format("\"{0}\"", str);
                                    //}
                                    //return str;
                                    return string.Format("\"{0}\"", q);
                                    ;
                                })
                                .ToArray()))
                            .ToArray();
                        FileHelper.CreateDirectory(newPath);
                        File.WriteAllLines(newPath, contents, new UTF8Encoding(false));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion
        }
    }
}