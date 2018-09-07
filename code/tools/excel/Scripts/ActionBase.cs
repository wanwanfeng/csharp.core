﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library;
using Library.Excel;
using Library.Extensions;
using DataTable = Library.Excel.DataTable;

namespace Script
{
    public class ActionBase : BaseSystemConsole
    {
        public static string regex =
            // "([\u4E00-\u9FA5]+)|([\u30A0-\u30FF])";
            "([\u0800-\u4E00]+)|([\u4E00-\u9FA5])|([\u30A0-\u30FF])";
        //"[\\x{3041}-\\x{3096}\\x{30A0}-\\x{30FF}\\x{3400}-\\x{4DB5}\\x{4E00}-\\x{9FCB}\\x{F900}-\\x{FA6A}\\x{2E80}-\\x{2FD5}\\x{FF5F}-\\x{FF9F}\\x{3000}-\\x{303F}\\x{31F0}-\\x{31FF}\\x{3220}-\\x{3243}\\x{3280}-\\x{337F}\\x{FF01}-\\x{FF5E}].+";

        protected ActionBase()
        {
            //Func<DataTable, DataTable> onSheetBeforeAction = (dt) =>
            //{
            //    var cache = ExcelByBase.Data.ConvertToRowsDictionary(dt)
            //        .ToDictionary(p => p.Key, q => string.Join("|", q.Value.Cast<string>().ToArray()))
            //        .ToDictionary(p => p.Key, q => Regex.IsMatch(q.Value, regex));
            //    if (cache.Values.All(p => p == false))
            //    {
            //        //忽略掉不匹配正则的
            //        return null;
            //    }

            //    int i = 0;
            //    foreach (KeyValuePair<string, bool> pair in cache)
            //    {
            //        var show = pair.Value;
            //        if (show)
            //        {
            //            var o = pair.Key + "_zh_cn";
            //            if (!dt.Columns.Contains(o))
            //            {
            //                //增加列
            //                dt.Columns.Add(o.ToString(), typeof(string));
            //                dt.Columns[o].SetOrdinal(i + 1);
            //                i++;
            //                //列复制
            //                foreach (DataRow dr in dt.Rows)
            //                    dr[o] = dr[pair.Key];
            //            }
            //        }
            //        i++;
            //    }
            //    return dt;
            //};

            //Func<ISheet, DataTable, DataTable> onSheetAction = (sheet, dt) =>
            //{
            //    var cache = ExcelByBase.Data.ConvertToRowsDictionary(dt)
            //        .ToDictionary(p => p.Key, q => string.Join("|", q.Value.Cast<string>().ToArray()))
            //        .ToDictionary(p => p.Key, q => Regex.IsMatch(q.Value, regex));
            //    if (cache.Values.All(p => p == false))
            //    {
            //        //忽略掉不匹配正则的
            //        return null;
            //    }

            //    int i = 0;
            //    foreach (KeyValuePair<string, bool> pair in cache)
            //    {
            //        var show = pair.Value;

            //        var style = sheet.GetColumnStyle(i);
            //        style.IsLocked = !pair.Key.Contains("_zh_cn");
            //        sheet.SetColumnHidden(i, !show);
            //        sheet.SetDefaultColumnStyle(i, style);
            //        if (show)
            //            sheet.AutoSizeColumn(i);
            //        i++;
            //    }
            //    return dt;
            //};
        }

        private static void ToCommon(string exs, Func<string, List<DataTable>> import, Action<DataTable, string> export)
        {
            List<string> files = CheckPath(exs);
            if (files.Count == 0) return;
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                var dts = import.Invoke(file).Where(p => p != null).ToList();
                if (dts.Count == 1)
                {
                    export.Invoke(dts.First(), file);
                    return;
                }
                dts.ForEach(dt =>
                {
                    string newPath = file.Replace(Path.GetExtension(file), "\\" + dt.TableName.Replace("$", ""));
                    export.Invoke(dt, newPath);
                });
            });
        }

        public static void ToXml(string exs, Func<string, List<DataTable>> import)
        {
            ToCommon(exs, import, ExcelByBase.Data.ExportToXml);
        }

        public static void ToCsv(string exs, Func<string, List<DataTable>> import)
        {
            ToCommon(exs, import, ExcelByBase.Data.ExportToCsv);
        }

        public static void ToJson(string exs, Func<string, List<DataTable>> import)
        {
            ToCommon(exs, import, ExcelByBase.Data.ExportToJson);
        }

        public static void ToExcel(string exs, Func<string, List<DataTable>> import)
        {
            ToCommon(exs, import, ExcelByBase.Data.ExportToExcel);
        }

        /// <summary>
        /// 多个DataTable保存在同一文件
        /// </summary>
        /// <param name="exs"></param>
        /// <param name="import"></param>
        public static void ToOneExcel(string exs, Func<string, List<DataTable>> import)
        {
            List<string> files = CheckPath(exs, SelectType.Folder);
            if (files.Count == 0) return;
            var dts = new List<DataTable>();
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                var dt = import(file).Where(p => p != null).ToList();
                if (dt.Count <= 0) return;
                dt.ForEach(q => q.TableName = Path.GetFileNameWithoutExtension(file));
                dts.AddRange(dt);
            });

            if (dts.Count == 0)
                return;
            ExcelByBase.Data.ExportToOneExcel(dts, InputPath);
        }

        #region 键值对

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public static void ToKvExcel(string exs, Func<string, List<DataTable>> import)
        {
            List<string> files = CheckPath(exs);
            if (files.Count == 0) return;

            var dtArray = new List<System.Data.DataTable>();
            var dtObject = new List<DataTable>();

            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                var dts = import(file).Where(p => p != null).ToList();
                if (dts.Count <= 0) return;
                dts.ForEach(dt =>
                {
                    dt.TableName = file.Replace(InputPath, "");
                    if (dt.IsArray)
                    {
                        var list = ExcelByBase.Data.ConvertToRowsList(dt)
                            .Select(p => string.Join("|", p.Cast<string>().ToArray()))
                            .Select(p => Regex.IsMatch(p, regex))
                            .ToList();
                        //返回符合正则表达式的列
                        var header = ExcelByBase.Data.GetHeaderList(dt).Where((p, i) => (i == 0 || list[i])).ToList();

                        var resdt = dt.DefaultView.ToTable(false, header.ToArray());

                        //foreach (object o in header.Skip(1))
                        //    resdt.Columns.Add(o + "_zh_ch", typeof(string));

                        if (header.Count > 1)
                            dtArray.Add(resdt);
                    }
                    else
                    {
                        var lt = ExcelByBase.Data.ConvertToListTable(dt);
                        lt.List = lt.List
                            .ToDictionary(p => p, q => string.Join("|", q.Cast<string>().ToArray()))
                            .Where(p => Regex.IsMatch(p.Value, regex))
                            .Select(p => p.Key)
                            .ToList();
                        //返回符合正则表达式的行
                        dtObject.Add(ExcelByBase.List.ConvertToDataTable(lt));
                    }
                });
            });

            if (dtArray.Count == 0 && dtObject.Count == 0) return;

            DataTable dd = new DataTable();
            dd.Columns.Add("path", typeof(string));
            dd.Columns.Add("id", typeof(string));
            dd.Columns.Add("key", typeof(string));
            dd.Columns.Add("value", typeof(string));
            dd.Columns.Add("value_zh_cn", typeof(string));

            if (dtArray.Count != 0)
            {
                foreach (System.Data.DataTable dataTable in dtArray)
                {
                    var header = ExcelByBase.Data.GetHeaderList(dataTable);
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        foreach (string s in header.Skip(1))
                        {
                            if (string.IsNullOrEmpty(dr[s].ToString())) continue;
                            dd.Rows.Add(dataTable.TableName, dr[0], s, dr[s], dr[s]);
                        }
                    }
                }
            }

            if (dtObject.Count != 0)
            {
                foreach (DataTable dataTable in dtObject)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        if (string.IsNullOrEmpty(dr[1].ToString())) continue;
                        dd.Rows.Add(dataTable.TableName, dr[0],
                            dr[0].ToString().Substring(dr[0].ToString().LastIndexOf("/", StringComparison.Ordinal) + 1),
                            dr[1], dr[1]);
                    }
                }
            }
            
            ExcelByBase.Data.ExportToExcel(dd, InputPath);
        }

        public class DataTableModel
        {
            public Func<string, DataTable> loadAction;
            public Action<DataTable, string> saveAction;
            public Func<string, List<List<object>>, string> isCustomAction = null;
        }

        public class ListTableModel
        {
            public Func<string, ListTable> loadAction;
            public Action<ListTable, string> saveAction;
            public Func<string, List<List<object>>, string> isCustomAction = null;
        }


        /// <summary>
        /// 还原键值对
        /// </summary>
        public static void KvExcelTo(DataTableModel tableModel)
        {
            List<string> files = CheckPath(".xlsx", SelectType.File);
            if (files.Count == 0) return;

            var isBak = SystemConsole.GetInputStr("是否每一个备份文件？(true:false)").AsBool();

            var dts = new List<DataTable>();
            files.ForEach(file =>
            {
                Console.WriteLine(" from : " + file);
                dts.AddRange(ExcelByBase.Data.ImportToDataTable(file));
            });

            if (dts.Count == 0)
                return;

            string root = InputPath.Replace(".xlsx", "");

            foreach (DataTable table in dts)
            {
                var cache =
                    ExcelByBase.Data.ConvertToListTable(table)
                        .List
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
                        var dt = tableModel.loadAction(fullpath);
                        var columns = dt.Columns;

                        if (dt.IsArray)
                        {
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
                                        var idTemp = dtr[0].ToString();
                                        var keyTemp = dtr[key];
                                        if (idTemp.Equals(id) /*&& keyTemp.Equals(value)*/)
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
                            if (isBak)
                                File.Copy(fullpath, fullpath + ".bak", true);
                            tableModel.saveAction.Invoke(dt, fullpath);
                        }
                        else
                        {
                            if (tableModel.isCustomAction == null) continue;
                            if (isBak)
                                File.Copy(fullpath, fullpath + ".bak", true);
                            File.WriteAllText(fullpath, tableModel.isCustomAction.Invoke(fullpath, pair.Value));
                        }
                    }
                    else
                    {
                        Console.WriteLine("文件不存在请检查路径!\t" + fullpath);
                    }
                }
            }
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public static void KvExcelTo(ListTableModel tableModel)
        {
            List<string> files = CheckPath(".xlsx", SelectType.File);
            if (files.Count == 0) return;

            var isBak = SystemConsole.GetInputStr("是否每一个备份文件？(true:false)").AsBool();

            var dts = new List<ListTable>();
            files.ForEach(file =>
            {
                Console.WriteLine(" from : " + file);
                dts.AddRange(ExcelByBase.Data.ImportToListTable(file));
            });

            if (dts.Count == 0)
                return;

            string root = InputPath.Replace(".xlsx", "");

            foreach (ListTable table in dts)
            {
                var cache = table.List
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
                        var lt = tableModel.loadAction(fullpath);
                        var columns = lt.Key;

                        if (lt.IsArray)
                        {
                            foreach (List<object> objects in pair.Value)
                            {
                                object id = objects[1];
                                string key = objects[2].ToString();
                                object value = objects[3];
                                string value_zh_cn = objects[4].ToString();

                                foreach (List<object> dtr in lt.List)
                                {
                                    if (columns.Contains(key))
                                    {
                                        var idTemp = dtr[0].ToString();
                                        var keyTemp = dtr[lt.Key.IndexOf(key)];
                                        if (idTemp.Equals(id) /*&& keyTemp.Equals(value)*/)
                                        {
                                            dtr[lt.Key.IndexOf(key)] = value_zh_cn.Replace("＠", "@");
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
                            if (isBak)
                                File.Copy(fullpath, fullpath + ".bak", true);
                            tableModel.saveAction.Invoke(lt, fullpath);
                        }
                        else
                        {
                            if (tableModel.isCustomAction == null) continue;
                            if (isBak)
                                File.Copy(fullpath, fullpath + ".bak", true);
                            File.WriteAllText(fullpath, tableModel.isCustomAction.Invoke(fullpath, pair.Value));
                        }
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