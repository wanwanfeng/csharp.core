using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library;
using Library.Excel;
using Library.Extensions;
using Library.Helper;
using DataTable = Library.Excel.DataTable;

namespace Script
{
    public class ActionBase : BaseSystemConsole
    {

        [Description("https://www.cnblogs.com/csguo/p/7401874.html")]
        public enum MyEnum
        {
            [Description("日文"), StringValue("([\u0800-\u4E00])")] 日文 = 1,
            [Description("日文平假名"), StringValue("([\u3040-\u309F])")] 日文平假名,
            [Description("日文片假名"), StringValue("([\u30A0-\u30FF])")] 日文片假名,
            [Description("日文片假名语音扩展"), StringValue("([\u31F0-\u31FF])")] 日文片假名语音扩展,
            [Description("中文"), StringValue("([\u4E00-\u9FA5])")] 中文 = 10
        }

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

        private static void ToCommon(string exs, Func<string, IEnumerable<DataTable>> import,
            Action<DataTable, string> export)
        {
            Action<string> action = file =>
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
                    string newPath = file.Replace(Path.GetExtension(file), "/" + dt.TableName.Trim('$'));
                    export.Invoke(dt, newPath);
                });
            };

            //Parallel.ForEach(CheckPath(exs), action);//并行操作
            CheckPath(exs).AsParallel().ForAll(action); //并行操作
            //CheckPath(exs).ForEach(action);//线性操作
        }

        public static void ToXml(string exs, Func<string, IEnumerable<DataTable>> import)
        {
            ToCommon(exs, import, ExcelByBase.Data.ExportToXml);
        }

        public static void ToCsv(string exs, Func<string, IEnumerable<DataTable>> import)
        {
            ToCommon(exs, import, ExcelByBase.Data.ExportToCsv);
        }

        public static void ToJson(string exs, Func<string, IEnumerable<DataTable>> import)
        {
            var isIndent = SystemConsole.GetInputStr("json文件是否进行格式化？(true:false)").AsBool();
            ToCommon(exs, import, (table, s) => { ExcelByBase.Data.ExportToJson(table, s, isIndent); });
        }

        public static void ToExcel(string exs, Func<string, IEnumerable<DataTable>> import)
        {
            ToCommon(exs, import, ExcelByBase.Data.ExportToExcel);
        }

        /// <summary>
        /// 多个DataTable保存在同一文件
        /// </summary>
        /// <param name="exs"></param>
        /// <param name="import"></param>
        public static void ToOneExcel(string exs, Func<string, IEnumerable<DataTable>> import)
        {
            var dts = CheckPath(exs, SelectType.Folder).AsParallel().SelectMany(file =>
            {
                Console.WriteLine(" is now : " + file);
                return import(file).Where(p => p != null).Select(p =>
                {
                    p.TableName = Path.GetFileNameWithoutExtension(file);
                    return p;
                });
            }).ToList();

            if (dts.Count == 0)
                return;
            ExcelByBase.Data.ExportToOneExcel(dts, InputPath);
        }

        #region 键值对

        public static void ToKvExcelAll(string exs, Func<string, IEnumerable<DataTable>> import)
        {
            var cache = AttributeHelper.GetCacheStringValue<MyEnum>();
            var str = string.Join("|", cache.Values);
            {
                SystemConsole.Run(config: new Dictionary<string, Action>()
                {
                    {
                        "不筛选", () =>
                        {
                            ToKvExcel(exs, import);
                        }
                    },
                     {
                        "筛选声音路径", () =>
                        {
                            ToKvExcel(exs, import,
                                p =>
                                    p.Contains("bgm/") || p.Contains("fullvoice/") || p.Contains("jingle/") ||
                                    p.Contains("se/") || p.Contains("surround/") || p.Contains("voice/"));
                        }
                    },
                    {
                        "匹配中文与日文",
                        () =>
                        {
                            ToKvExcel(exs, import, p => Regex.IsMatch(p, str));
                        }
                    },
                    {
                        "只匹配中文", () =>
                        {
                            ToKvExcel(exs, import,
                                p => Regex.IsMatch(p, str)
                                , s => Regex.IsMatch(s, cache[MyEnum.中文])
                                //,s => !Regex.IsMatch(s, string.Join("|", cache.Where(p => p.Key < MyEnum.中文).Select(p => p.Value)))
                                );
                        }
                    },
                    {
                        "只匹配日文", () =>
                        {
                            ToKvExcel(exs, import,
                                p => Regex.IsMatch(p, str),
                                p =>
                                {
                                    var val = Regex.Replace(p, "[a-z]", "", RegexOptions.IgnoreCase);
                                    val = Regex.Replace(val, "[0-9]", "", RegexOptions.IgnoreCase);
                                    val = Regex.Replace(val,
                                        "[ \\[ \\] \\^ \\-_*×――(^)（^）$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]", "");
                                    return Regex.IsMatch(val, str);
                                }
                                //,s => Regex.IsMatch(s,string.Join("|", cache.Where(p => p.Key < MyEnum.中文).Select(p => p.Value)))
                                //, s => !Regex.IsMatch(s, cache[MyEnum.中文])
                                );
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 导出为键值对
        /// </summary>
        private static void ToKvExcel(string exs, Func<string, IEnumerable<DataTable>> import, params Func<string, bool>[] predicate)
        {
            var dtArray = new List<System.Data.DataTable>();
            var dtObject = new List<DataTable>();

            CheckPath(exs).ForEach((file, index, count) =>
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
                            .Select(p => predicate.Length==0  || predicate.First()(p))
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
                            .ToDictionary(p => p, p => string.Join("", p.Cast<string>().ToArray()))
                            .Where(p => predicate.Length == 0 || predicate.First()(p.Value))
                            .ToList()
                            .Where(p => predicate.Length == 0 || predicate.Last()(p.Value))
                            .Select(p => p.Key)
                            .ToList();
                        //返回符合正则表达式的行
                        dtObject.Add(ExcelByBase.List.ConvertToDataTable(lt));
                    }
                });
            });

            if (dtArray.Count == 0 && dtObject.Count == 0) return;

            DataTable dd = new DataTable();
            dd.Columns.Add("path", typeof (string));
            dd.Columns.Add("id", typeof (string));
            dd.Columns.Add("key", typeof (string));
            dd.Columns.Add("value", typeof (string));
            dd.Columns.Add("value_zh_cn", typeof (string));

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
                        dd.Rows.Add(dataTable.TableName, dr[0], Path.GetFileName(dr[0].ToString()), dr[1], dr[1]);
                    }
                }
            }

            ExcelByBase.Data.ExportToExcel(dd, InputPath);
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public static void KvExcelToFromDataTable(
            Func<string, DataTable> loadAction,
            Action<DataTable, string> saveAction,
            Func<string, List<List<object>>, string> isCustomAction = null
            )
        {
            var caches = CheckPath(".xlsx", SelectType.File).AsParallel().SelectMany(file =>
            {
                Console.WriteLine(" from : " + file);
                return ExcelByBase.Data.ImportToDataTable(file);
            }).Select(table =>
            {
                var cache =
                    ExcelByBase.Data.ConvertToListTable(table)
                        .List
                        .ToLookup(p => p.First())
                        .ToDictionary(p => p.Key.ToString(), q => q.ToList());
                cache.Remove("path");
                return new {dic = cache, file = table.FullName};
            }).ToList();

            if (caches.Count == 0)
                return;

            string root = InputPath.Replace(".xlsx", "");
            var isBak = SystemConsole.GetInputStr("是否每一个备份文件？(true:false)").AsBool();
            List<List<string>> error = new List<List<string>>();

            foreach (var table in caches)
            {
                foreach (KeyValuePair<string, List<List<object>>> pair in table.dic)
                {
                    string fullpath = root + pair.Key;
                    try
                    {
                        bool isSave = false;
                        Console.WriteLine(" is now : " + fullpath);
                        var dt = loadAction(fullpath);
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
                            saveAction.Invoke(dt, fullpath);
                        }
                        else
                        {
                            if (isCustomAction == null) continue;
                            if (isBak)
                                File.Copy(fullpath, fullpath + ".bak", true);
                            File.WriteAllText(fullpath, isCustomAction.Invoke(fullpath, pair.Value));
                        }
                    }
                    catch (Exception e)
                    {
                        error.Add(new List<string>()
                        {
                            error.Count + Enumerable.Repeat("-", 100).Aggregate("", (a, b) => a + b),
                            fullpath,
                            e.Message,
                            e.StackTrace,
                            ""
                        });
                        Console.WriteLine(fullpath + "\t" + e.Message);
                    }
                }
            }
            WriteError("export-file", error.Select(p => p.Skip(1).First()));
            WriteError("export-message", error.SelectMany(p => p));
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public static
            void KvExcelToFromListTable(
            Func<string, ListTable> loadAction,
            Action<ListTable, string> saveAction,
            Func<string, List<List<object>>, string> isCustomAction = null
            )
        {
            var caches = CheckPath(".xlsx", SelectType.File).AsParallel().SelectMany(file =>
            {
                Console.WriteLine(" from : " + file);
                return ExcelByBase.Data.ImportToListTable(file);
            }).Select(table =>
            {
                var cache = table.List
                    .ToLookup(p => p.First())
                    .ToDictionary(p => p.Key.ToString(), q => q.ToList());
                cache.Remove("path");
                return new {dic = cache, file = table.FullName};
            }).ToList();

            if (caches.Count == 0)
                return;

            string root = InputPath.Replace(".xlsx", "");
            var isBak = SystemConsole.GetInputStr("是否每一个备份文件？(true:false)").AsBool();
            List<List<string>> error = new List<List<string>>();

            foreach (var table in caches)
            {
                foreach (KeyValuePair<string, List<List<object>>> pair in table.dic)
                {
                    string fullpath = root + pair.Key;
                    try
                    {
                        bool isSave = false;
                        Console.WriteLine(" is now : " + fullpath);
                        var lt = loadAction(fullpath);
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
                            saveAction.Invoke(lt, fullpath);
                        }
                        else
                        {
                            if (isCustomAction == null) continue;
                            if (isBak)
                                File.Copy(fullpath, fullpath + ".bak", true);
                            File.WriteAllText(fullpath, isCustomAction.Invoke(fullpath, pair.Value));
                        }
                    }
                    catch (Exception e)
                    {
                        error.Add(new List<string>()
                        {
                            error.Count + Enumerable.Repeat("-", 100).Aggregate("", (a, b) => a + b),
                            fullpath,
                            e.Message,
                            e.StackTrace,
                            ""
                        });
                        Console.WriteLine(fullpath + "\t" + e.Message);
                    }
                }
            }
            WriteError("export-file", error.Select(p => p.Skip(1).First()));
            WriteError("export-message", error.SelectMany(p => p));
        }

        #endregion
    }
}