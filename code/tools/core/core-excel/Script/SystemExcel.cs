using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Extensions;

namespace Library.Excel
{
    public class BaseSystemExcel : BaseSystemConsole
    {
		[Description("https://www.cnblogs.com/csguo/p/7401874.html")]
		public enum RegexLanguaheEnum
		{
			[Description("日文"), StringValue(@"([\u0800-\u4E00])")]
			日文 = 1,
			[Description("日文平假名"), StringValue(@"([\u3040-\u309F])")]
			日文平假名,
			[Description("日文片假名"), StringValue(@"([\u30A0-\u30FF])")]
			日文片假名,
			[Description("日文片假名语音扩展"), StringValue(@"([\u31F0-\u31FF])")]
			日文片假名语音扩展,
			[Description("中文"), StringValue(@"([\u4E00-\u9FA5])")]
			中文 = 10,
			[Description("韩文"), StringValue(@"([\xAC00-\xD7A3])|([\x3130-\x318F])")]
			韩文 = 20,
		}

		/// <summary>
		/// https://www.cnblogs.com/icejd/archive/2010/12/22/1913508.html
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static bool CheckMatches(Regex regex, string val)
		{
			var temp = Regex.Replace(val, "[\\s\\p{P}\n\r=<>$>+￥^]", "");
			MatchCollection mc = regex.Matches(temp);
			return mc.Count == 0;
		}

		/// <summary>
		/// 已首列分组形成字典
		/// </summary>
		/// <returns></returns>
		protected static List<Dictionary<string, List<List<object>>>> GetFileCaches()
        {
            return GetListTables().Select(table =>
            {
                var cache = table.Rows
                    .GroupBy(p => p.First())
                    .ToDictionary(p => p.Key.ToString(), q => q.ToList());
                cache.Remove(table.Columns.First());
                cache.Remove("");
                return cache;
            }).ToList();
        }

        /// <summary>
        /// 已首列分组形成字典
        /// </summary>
        /// <returns></returns>
        protected static IEnumerable<ListTable> GetListTables()
        {
            return CheckPath(".xlsx|.xls", SelectType.File).SelectMany(file =>
            {
                Console.WriteLine(" from : " + file);
                return ExcelUtils.ImportFromExcel(file, false).Select(p => p.ToListTable());
            });
        }
    }
}