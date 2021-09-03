using Library.Excel;
using Library.Extensions;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Script
{
    public class ActionBase : BaseSystemExcel
    {
        public virtual Func<string, IEnumerable<DataTable>> import { get; set; }
        public virtual Action<DataTable, string> export { get; set; }
        public virtual string selectExtension { get; set; }
		public string getSelectExtension => Environment.GetEnvironmentVariable(GetType().Namespace + ".SelectExtension") ?? selectExtension;
        protected void ToCommon(Action<DataTable, string> expAction)
        {
            Action<string> action = file =>
            {
                Console.WriteLine(" is now : " + file);
                var dts = import.Invoke(file).Where(p => p != null).ToList();

                foreach (var dt in dts)
                {
                    //例:删除第一列 
                    //dt.Columns.RemoveAt(0);

                    //例:根据过滤条件删除行
                    //DataRow[] select = dt.Select("sex = '男' and age >= 18");
                    //dt.Rows.Clear();
                    //select.ForEach(dt.Rows.Add);
                }

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

            //Parallel.ForEach(CheckPath(getSelectExtension), action);//并行操作
            CheckPath(getSelectExtension).AsParallel().ForAll(action); //并行操作
            //CheckPath(getSelectExtension).ForEach(action); //并行操作
        }

        protected void ToCsv() => ToCommon(ExcelUtils.ExportToCsv);

        protected void ToJson() => ToCommon((dt, file) => { ExcelUtils.ExportToJson(dt, file); });

        protected void ToXml() => ToCommon(ExcelUtils.ExportToXml);

        protected void ToExcel() => ToCommon((dt, file) => { ExcelUtils.ExportToExcel(dt, file); });

        /// <summary>
        /// 多个DataTable保存在同一文件
        /// </summary>
        public void ToOneExcel()
        {
            var dts = CheckPath(getSelectExtension, SelectType.Folder).AsParallel().SelectMany(file =>
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
            ExcelUtils.ExportToExcel(dts, InputPath);
        }

		#region 键值对

		public void ToKvExcelAll(bool isArray = true)
		{
			var cache = AttributeHelper.GetCacheStringValue<RegexLanguaheEnum>();
			var regexStr = string.Empty;
			SystemConsole.Run(config: new Dictionary<string, Action>()
			{
				["匹配中文与日文"] = () =>
				{
					regexStr = string.Join("|", cache.Where(p => p.Key >= RegexLanguaheEnum.日文 && p.Key <= RegexLanguaheEnum.中文).Select(p => p.Value));
					ToKvExcel(isArray, regexStr);
				},
				["只匹配中文"] = () =>
				{
					 regexStr = cache[RegexLanguaheEnum.中文];
					ToKvExcel(isArray, regexStr);
				},
				["只匹配日文"] = () =>
				{
					regexStr = string.Join("|", cache.Where(p => p.Key < RegexLanguaheEnum.中文).Select(p => p.Value));
					ToKvExcel(isArray, regexStr);
				},
				["只匹配韩文"] = () =>
				{
					regexStr = cache[RegexLanguaheEnum.韩文];
					ToKvExcel(isArray, regexStr);
				}
			});
		}

        /// <summary>
        /// 导出为键值对
        /// </summary>
        private void ToKvExcel(bool isArray, string regexStr, params Func<string, bool>[] predicate)
        {
            var dtArray = new List<System.Data.DataTable>();
            var dtObject = new List<DataTable>();

			Regex regex = new Regex(regexStr);

			CheckPath(getSelectExtension).ForEach((file, index) =>
            {
                Console.WriteLine(" is now : " + file);
                var dts = import(file).Where(p => p != null).ToList();
                if (dts.Count <= 0) return;
                dts.ForEach(dt =>
                {
                    dt.TableName = file.Replace(InputPath, "");
                    //if (dt.IsArray)
                    if (isArray)
                    {
						var list = ExcelUtils.ConvertToRowsList(dt)
							.Select( p => {
								bool result = false;
								foreach (var item in p)
								{
									if (CheckMatches(regex, item.ToString())) continue;
									result = true;
									break;
								}
								return new { p, result };
							})
							.Select(p => p.result)
							.ToList();

                        //返回符合正则表达式的列
                        var header = dt.GetHeaderList().Where((p, i) => (i == 0 || list[i])).ToList();

                        var resdt = dt.DefaultView.ToTable(false, header.ToArray());

                        if (header.Count > 1)
                            dtArray.Add(resdt);
                    }
                    else
                    {
                        var lt = dt.ToListTable();
                        lt.Rows = lt.Rows
                            .ToDictionary(p => p, p => string.Join("", p.Select(q => q.ToString()).ToArray()))
                            .Where(p => predicate.Length == 0 || predicate.First()(p.Value))
                            .ToList()
                            .Where(p => predicate.Length == 0 || predicate.Last()(p.Value))
                            .Select(p => p.Key)
                            .ToList();
                        //返回符合正则表达式的行
                        dtObject.Add(lt.ToDataTable());
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
							if (CheckMatches(regex, dr[s].ToString())) continue;
                            dd.Rows.Add(dataTable.TableName, dr[0], s, dr[s], "");
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
						if (CheckMatches(regex, dr[1].ToString())) continue;
						dd.Rows.Add(dataTable.TableName, dr[0], Path.GetFileName(dr[0].ToString()), dr[1], "");
					}
				}
			}

            ExcelUtils.ExportToExcel(dd, InputPath);
        }

        /// <summary>
        /// 还原键值对
        /// </summary>
        public void KvExcelTo(bool isArray = true, Func<string, List<List<object>>, string> isCustomAction = null)
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

                        //if (data.IsArray)
                        if (isArray)
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