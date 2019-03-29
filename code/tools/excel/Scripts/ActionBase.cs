using System;
using System.Collections.Generic;
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
    public class ActionBase : BaseSystemExcel
    {
        /// <summary>
        /// Json是否缩进
        /// </summary>
        protected bool isIndent { get; set; }

        /// <summary>
        /// excel导入时首行是否是Key
        /// </summary>
        protected bool firstIsKey { get; set; }

        public virtual Func<string, IEnumerable<DataTable>> import { get; set; }
        public virtual Action<DataTable, string> export { get; set; }
        public virtual string selectExtension { get; set; }

        protected void ToCommon(Action<DataTable, string> expAction, Func<DataTable, DataTable> runAction = null)
        {
            Action<string> action = file =>
            {
                Console.WriteLine(" is now : " + file);
                var dts =
                    import.Invoke(file)
                        .Where(p => p != null)
                        .Select(p => runAction == null ? p : runAction.Invoke(p))
                        .ToList();
                if (dts.Count == 1)
                {
                    expAction.Invoke(dts.First(), file);
                    return;
                }
                dts.ForEach(dt =>
                {
                    string newPath = file.Replace(Path.GetExtension(file), "/" + dt.TableName.Trim('$'));
                    expAction.Invoke(dt, newPath);
                });
            };

            //Parallel.ForEach(CheckPath(selectExtension), action);//并行操作
            CheckPath(selectExtension).AsParallel().ForAll(action); //并行操作
            //CheckPath(selectExtension).ForEach(action); //并行操作
        }

        protected void ToCsv()
        {
            ToCommon(ExcelByBase.Data.ExportToCsv);
        }

        protected void ToJson()
        {
            ToCommon((table, s) => { ExcelByBase.Data.ExportToJson(table, s, isIndent); });
        }

        protected void ToXml()
        {
            ToCommon(ExcelByBase.Data.ExportToXml);
        }

        protected void ToExcel()
        {
            ToCommon(ExcelByBase.Data.ExportToExcel);
        }

        /// <summary>
        /// 多个DataTable保存在同一文件
        /// </summary>
        public void ToOneExcel()
        {
            var dts = CheckPath(selectExtension, SelectType.Folder).AsParallel().SelectMany(file =>
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

        public void ToKvExcelAll()
        {
            var cache = AttributeHelper.GetCacheStringValue<RegexLanguaheEnum>();
            var str = string.Join("|", cache.Values);
            {
                SystemConsole.Run(config: new Dictionary<string, Action>()
                {
                    {
                        "不筛选", () =>
                        {
                            ToKvExcel();
                        }
                    },
                    {
                        "筛选声音路径", () =>
                        {
                            ToKvExcel(
                                p =>
                                    p.Contains("bgm/") || p.Contains("fullvoice/") || p.Contains("jingle/") ||
                                    p.Contains("se/") || p.Contains("surround/") || p.Contains("voice/") ||
                                    p.Contains("vo_kyube_") || p.Contains("vo_char_") || p.Contains("vo_game_"));
                        }
                    },
                    {
                        "匹配中文与日文",
                        () =>
                        {
                            ToKvExcel(p => Regex.IsMatch(p, str));
                        }
                    },
                    {
                        "只匹配中文", () =>
                        {
                            ToKvExcel(
                                p => Regex.IsMatch(p, str)
                                , s => Regex.IsMatch(s, cache[RegexLanguaheEnum.中文])
                                //,s => !Regex.IsMatch(s, string.Join("|", cache.Where(p => p.Key < MyEnum.中文).Select(p => p.Value)))
                                );
                        }
                    },
                    {
                        "只匹配日文", () =>
                        {
                            ToKvExcel(
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
        private void ToKvExcel(params Func<string, bool>[] predicate)
        {
            var dtArray = new List<System.Data.DataTable>();
            var dtObject = new List<DataTable>();

            CheckPath(selectExtension).ForEach((file, index) =>
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
                            .Select(p => string.Join("|", p.Select(q => q.ToString()).ToArray()))
                            .Select(p => predicate.Length == 0 || predicate.First()(p))
                            .ToList();
                        //返回符合正则表达式的列
                        var header = dt.GetHeaderList().Where((p, i) => (i == 0 || list[i])).ToList();

                        var resdt = dt.DefaultView.ToTable(false, header.ToArray());

                        //foreach (object o in header.Skip(1))
                        //    resdt.Columns.Add(o + "_zh_ch", typeof(string));

                        if (header.Count > 1)
                            dtArray.Add(resdt);
                    }
                    else
                    {
                        var lt = (ListTable) dt;
                        lt.Rows = lt.Rows
                            .ToDictionary(p => p, p => string.Join("", p.Select(q => q.ToString()).ToArray()))
                            .Where(p => predicate.Length == 0 || predicate.First()(p.Value))
                            .ToList()
                            .Where(p => predicate.Length == 0 || predicate.Last()(p.Value))
                            .Select(p => p.Key)
                            .ToList();
                        //返回符合正则表达式的行
                        dtObject.Add(lt);
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
                    var header = dataTable.GetHeaderList();
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
        public void KvExcelTo(Func<string, List<List<object>>, string> isCustomAction = null)
        {
            var caches = GetFileCaches();
            if (caches.Count == 0) return;

            string root = InputPath.Replace(".xlsx", "");
            var isBak = SystemConsole.GetInputStr("是否每一个备份文件？(true:false)", def: "false").AsBool(false);
            List<List<string>> error = new List<List<string>>();

            foreach (var table in caches)
            {
                foreach (KeyValuePair<string, List<List<object>>> pair in table)
                {
                    string fullpath = root + pair.Key;

                    try
                    {
                        bool isSave = false;
                        Console.WriteLine(" is now : " + fullpath);
                        var data = import(fullpath).FirstOrDefault();
                        var columns = data.Columns;

                        if (data.IsArray)
                        {
                            foreach (List<object> objects in pair.Value)
                            {
                                object id = objects[1];
                                string key = objects[2].ToString();
                                object value = objects[3];
                                object value_zh_cn = objects[4];

                                foreach (DataRow dtr in data.Rows)
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
                            export.Invoke(data, fullpath);
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