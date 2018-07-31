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
using DataTable = Library.Excel.DataTable;

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


        public static string regex =
            "([\u4E00-\u9FA5]+)|([\u4E00-\u9FA5])|([\u30A0-\u30FF])|([\u30A0-\u30FF])";
        //"[\\x{3041}-\\x{3096}\\x{30A0}-\\x{30FF}\\x{3400}-\\x{4DB5}\\x{4E00}-\\x{9FCB}\\x{F900}-\\x{FA6A}\\x{2E80}-\\x{2FD5}\\x{FF5F}-\\x{FF9F}\\x{3000}-\\x{303F}\\x{31F0}-\\x{31FF}\\x{3220}-\\x{3243}\\x{3280}-\\x{337F}\\x{FF01}-\\x{FF5E}].+";


        static ActionBase()
        {
            CacheSelect = AttributeHelper.GetCacheDescription<SelectType>();
        }

        public static string InputPath { get; set; }

        public static List<string> CheckPath(string exce, SelectType selectType = SelectType.All)
        {
            List<string> files = new List<string>();

            string path = SystemConsole.GetInputStr(CacheSelect[selectType], "您选择的文件夹或文件：");
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

        private static void ToCommon(string exs, Func<string, List<DataTable>> import, Action<DataTable, string> export)
        {
            ExcelByNpoi.OnSheetAction = null;
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
            ExcelByNpoi.OnSheetAction = null;
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
            ExcelByNpoi.ExportDataTableToExcel(Path.ChangeExtension(InputPath, ".xlsx"), dts.ToArray());
        }

        #region 键值对

        /// <summary>
        /// 导出为键值对
        /// </summary>
        public static void ToKvExcel(string exs, Func<string, DataTable> import)
        {
            ExcelByNpoi.OnSheetAction = null;
            List<string> files = CheckPath(exs);
            if (files.Count == 0) return;

            var dtArray = new List<System.Data.DataTable>();
            var dtObject = new List<DataTable>();

            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                var dt = import.Invoke(file);
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

            ExcelByNpoi.ExportDataTableToExcel(Path.ChangeExtension(InputPath, ".xlsx"), dd);
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public static void KvExcelTo(Func<string, DataTable> loadAction, Action<DataTable, string> saveAction,
            Func<string, List<List<object>>, string> isNotArrayAction = null)
        {
            ExcelByNpoi.OnSheetAction = null;
            List<string> files = CheckPath(".xlsx", SelectType.File);
            if (files.Count == 0) return;

            var isBak = SystemConsole.GetInputStr("是否每一个备份文件？(true:false)").AsBool();

            var dts = new List<DataTable>();
            files.ForEach(file =>
            {
                Console.WriteLine(" is now : " + file);
                dts.AddRange(ExcelByNpoi.ImportExcelToDataTable(file));
            });

            if (dts.Count == 0)
                return;

            string root = InputPath.Replace(".xlsx", "");

            foreach (DataTable dataTable in dts)
            {
                var cache =
                    ExcelByBase.Data.ConvertToListTable(dataTable)
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
                            if (isBak)
                                File.Copy(fullpath, fullpath + ".bak", true);
                            saveAction.Invoke(dt, fullpath);
                        }
                        else
                        {
                            if (isNotArrayAction == null) continue;

                            if (isBak)
                                File.Copy(fullpath, fullpath + ".bak", true);
                            File.WriteAllText(fullpath, isNotArrayAction.Invoke(fullpath, pair.Value));
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