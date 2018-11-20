using System;
using System.Collections.Generic;
using System.Linq;
using Library.Extensions;

namespace Library.Excel
{
    public class BaseSystemExcel : BaseSystemConsole
    {
        /// <summary>
        /// 已首列分组形成字典
        /// </summary>
        /// <returns></returns>
        protected static List<Dictionary<string, List<List<object>>>> GetFileCaches()
        {
            return CheckPath(".xlsx", SelectType.File).SelectMany(file =>
            {
                Console.WriteLine(" from : " + file);
                return ExcelByBase.Data.ImportToListTable(file);
            }).Select(table =>
            {
                var cache = table.List
                    .GroupBy(p => p.First())
                    .ToDictionary(p => p.Key.ToString(), q => q.ToList());
                cache.Remove(table.Key.First());
                return cache;
            }).ToList();
        }
    }
}