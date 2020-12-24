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
		/// http://www.unicode.org/Public/UNIDATA/UnicodeData.txt
		/// https://www.cnblogs.com/wq920/p/4268400.html
		///
		/// \pP 其中的小写 p 是 property 的意思，表示 Unicode 属性，用于 Unicode 正表达式的前缀。 
		/// 大写 P 表示 Unicode 字符集七个字符属性之一：标点字符。
		/// 其他六个是
		/// L：字母；
		/// M：标记符号（一般不会单独出现）；
		/// Z：分隔符（比如空格、换行等）；
		/// S：符号（比如数学符号、货币符号等）；
		/// N：数字（比如阿拉伯数字、罗马数字等）；
		/// 
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static bool CheckMatches(Regex regex, string val)
		{
			//val = Regex.Replace(val, "[\\p{P}]", "");
			val = Regex.Replace(val, "[\\s\\p{P}\n\r=<>$>+￥^]", "");
			//val = Regex.Replace(val, "[\\s\\p{N}\n\r=<>$>+￥^]", "");
			//val = Regex.Replace(val, "[\\s\\p{S}\n\r=<>$>+￥^]", "");
			//val = Regex.Replace(val, "[\\s\\p{M}\n\r=<>$>+￥^]", "");
			//val = Regex.Replace(val, "[\\s\\p{Z}\n\r=<>$>+￥^]", "");
			//val = Regex.Replace(val, "[\\s\\p{L}\n\r=<>$>+￥^]", "");
			MatchCollection mc = regex.Matches(val);
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