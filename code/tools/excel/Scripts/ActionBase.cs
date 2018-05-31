using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Excel;
using Library.Extensions;
using Library.Helper;

namespace Script
{
    public class ActionBase
    {
        public enum SelectType
        {
            [Description("请拖入文件：")] File,
            [Description("请拖入文件夹：")] Folder,
            [Description("请拖入文件夹或文件：")] All
        }

        public static IDictionary<SelectType, string> CacheSelect;

        static ActionBase()
        {
            CacheSelect = AttributeHelper.GetCacheDescription<SelectType>();
        }

        public static string InputPath { get; set; }

        public static List<string> CheckPath(string exce, SelectType selectType = SelectType.All)
        {
            List<string> files = new List<string>();

            string path = SystemExtensions.GetInputStr(CacheSelect[selectType], "您选择的文件夹或文件：");
            if (string.IsNullOrEmpty(path))
                return files;

            switch (selectType)
            {
                case SelectType.File:
                    if (File.Exists(path))
                    {
                        files.Add(path);
                    }
                    break;
                case SelectType.Folder:
                    if (Directory.Exists(path))
                    {
                        files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();
                    }
                    break;
                case SelectType.All:
                    if (Directory.Exists(path))
                    {
                        files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();
                    }
                    else if (File.Exists(path))
                    {
                        files.Add(path);
                    }
                    break;
                default:
                    Console.WriteLine("path is not valid!");
                    return files;
            }

            InputPath = path;
            var exs = exce.AsStringArray(',', '|').Select(p => p.StartsWith(".") ? p : "." + p).ToList();
            files = files.Where(p => exs.Contains(Path.GetExtension(p))).ToList();
            files.Sort();
            return files;
        }

        public static void ToXml(string exs, Func<string, List<DataTable>> convertToDataTable)
        {
            ExcelByNpoi.OnSheetAction = null;
            List<string> files = CheckPath(exs);
            if (files.Count == 0) return;
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                var dts = convertToDataTable.Invoke(file);
                if (dts.Count == 1)
                {
                    ExcelByBase.ConvertDataTableToXml(dts.First(), file);
                    return;
                }
                dts.ForEach(dt =>
                {
                    string newPath = file.Replace(Path.GetExtension(file), "\\" + dt.TableName.Replace("$", ""));
                    ExcelByBase.ConvertDataTableToXml(dt, Path.ChangeExtension(newPath, ".xml"));
                });
            });
        }

        public static void ToCsv(string exs, Func<string, List<DataTable>> convertToDataTable)
        {
            ExcelByNpoi.OnSheetAction = null;
            List<string> files = CheckPath(exs);
            if (files.Count == 0) return;
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                var dts = convertToDataTable.Invoke(file);
                if (dts.Count == 1)
                {
                    ExcelByBase.ConvertDataTableToCsv(dts.First(), file);
                    return;
                }
                dts.ForEach(dt =>
                {
                    string newPath = file.Replace(Path.GetExtension(file), "\\" + dt.TableName.Replace("$", ""));
                    ExcelByBase.ConvertDataTableToCsv(dt, Path.ChangeExtension(newPath, ".csv"));
                });
            });
        }

        public static void ToJson(string exs, Func<string, List<DataTable>> convertToDataTable)
        {
            ExcelByNpoi.OnSheetAction = null;
            List<string> files = CheckPath(exs);
            if (files.Count == 0) return;
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                var dts = convertToDataTable(file);
                if (dts.Count == 1)
                {
                    ExcelByBase.ConvertDataTableToJsonByPath(dts.First(), file);
                    return;
                }
                dts.ForEach(dt =>
                {
                    string newPath = file.Replace(Path.GetExtension(file), "\\" + dt.TableName.Replace("$", ""));
                    ExcelByBase.ConvertDataTableToJsonByPath(dt, Path.ChangeExtension(newPath, ".json"));
                });
            });
        }

        /// <summary>
        /// 同一文件作为多个DataTable保存在同一个excel中
        /// </summary>
        /// <param name="exs"></param>
        /// <param name="convertToDataTable"></param>
        public static void ToExcel(string exs, Func<string, List<DataTable>> convertToDataTable)
        {
            //ExcelByNpoi.OnSheetAction = null;
            List<string> files = CheckPath(exs);
            if (files.Count == 0) return;
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                var dts = convertToDataTable(file);

                if (dts.Count == 1)
                {
                    ExcelByNpoi.DataTableToExcel(Path.ChangeExtension(file, ".xlsx"), dts.First());
                    return;
                }

                dts.ForEach(dt =>
                {
                    string newPath = file.Replace(Path.GetExtension(file), "\\" + dt.TableName.Replace("$", ""));
                    ExcelByNpoi.DataTableToExcel(Path.ChangeExtension(newPath, ".xlsx"), dt);
                });
            });
        }

        /// <summary>
        /// 多个DataTable保存在同一文件
        /// </summary>
        /// <param name="exs"></param>
        /// <param name="convertToDataTable"></param>
        public static void ToOneExcel(string exs, Func<string, List<DataTable>> convertToDataTable)
        {
            ExcelByNpoi.OnSheetAction = null;
            List<string> files = CheckPath(exs, SelectType.Folder);
            if (files.Count == 0) return;
            var dts = new List<DataTable>();
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                var dt = convertToDataTable(file);
                dts.AddRange(dt);
            });

            if (dts.Count == 0)
                return;
            ExcelByNpoi.DataTableToExcel(Path.ChangeExtension(InputPath, ".xlsx"), dts.ToArray());
        }

        #region 键值对

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public static void ToKvExcel(string exs, Func<string, string, DataTable> convertToDataTable)
        {
            ExcelByNpoi.OnSheetAction = null;
            List<string> files = CheckPath(exs);
            if (files.Count == 0) return;
            var dts = new List<DataTable>();

            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                var dt = convertToDataTable.Invoke(file, null);
                dt.TableName = file.Replace(InputPath, "");
                string regex = "([\u4E00-\u9FA5]+)|([\u4E00-\u9FA5])|([\u30A0-\u30FF])|([\u30A0-\u30FF])";
                var list = ExcelByBase.ConvertDataTableToRowsList(dt)
                    .Select(p => string.Join("|", p.Cast<string>().ToArray()))
                    .Select(p => Regex.IsMatch(p, regex))
                    .ToList();
                //返回符合正则表达式的列
                var header =
                    ExcelByBase.GetHeaderList(dt).Where((p, i) => (i == 0 || list[i])).ToList();

                var resdt = dt.DefaultView.ToTable(false, header.ToArray());

                //foreach (object o in header.Skip(1))
                //{
                //    resdt.Columns.Add(o + "_zh_ch", typeof(string));
                //}
                if (header.Count > 1)
                    dts.Add(resdt);
            });

            if (dts.Count == 0)
                return;

            DataTable dd = new DataTable();
            dd.Columns.Add("path", typeof (string));
            dd.Columns.Add("id", typeof (string));
            dd.Columns.Add("key", typeof (string));
            dd.Columns.Add("value", typeof (string));
            dd.Columns.Add("value_zh_cn", typeof (string));
            foreach (DataTable dataTable in dts)
            {
                var header = ExcelByBase.GetHeaderList(dataTable);
                foreach (DataRow dr in dataTable.Rows)
                {
                    foreach (string s in header.Skip(1))
                    {
                        dd.Rows.Add(dataTable.TableName, dr[0], s, dr[s], dr[s]);
                    }
                }
            }
            ExcelByNpoi.DataTableToExcel(Path.ChangeExtension(InputPath, ".xlsx"), dd);
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public static void KvExcelTo(Action<DataTable, string> saveAction)
        {
            ExcelByNpoi.OnSheetAction = null;
            List<string> files = CheckPath(".xlsx", SelectType.File);
            if (files.Count == 0) return;
            var dts = new List<DataTable>();
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                dts.AddRange(ExcelByNpoi.ExcelToDataTable(file));
            });

            if (dts.Count == 0)
                return;

            string root = InputPath.Replace(".xlsx", "");

            foreach (DataTable dataTable in dts)
            {
                var cache =
                    ExcelByBase.ConvertDataTableToList(dataTable)
                        .ToLookup(p => p.First())
                        .ToDictionary(p => p.Key.ToString(), q => q.ToList());
                cache.Remove("path");

                foreach (KeyValuePair<string, List<List<object>>> pair in cache)
                {
                    string fullpath = root + pair.Key;

                    if (File.Exists(fullpath))
                    {
                        bool isSave = false;
                        Console.WriteLine(" is now : " + fullpath);
                        var dt = ExcelByBase.ConvertCsvToDataTable(fullpath);
                        var columns = dt.Columns;

                        foreach (List<object> objects in pair.Value)
                        {
                            object id = objects[1];
                            string key = objects[2].ToString();
                            object value = objects[3];
                            string value_zh_cn = objects[4].ToString();

                            foreach (DataRow dtr in dt.Rows)
                            {
                                if (columns.Contains(key))
                                {
                                    var idTemp = dtr[0];
                                    var keyTemp = dtr[key];
                                    if (idTemp.Equals(id) && keyTemp.Equals(value))
                                    {
                                        dtr[key] = value_zh_cn;
                                        isSave = true;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("不包含列 !\t" + fullpath + "\t" + key);
                                }
                            }
                        }
                        if (!isSave) continue;
                        File.Copy(fullpath, fullpath + ".bak", true);
                        saveAction.Invoke(dt, fullpath);
                    }
                    else
                    {
                        Console.WriteLine("文件不存在请检查路径!\t" + fullpath);
                    }
                }
            }
        }

        #endregion
    }
}