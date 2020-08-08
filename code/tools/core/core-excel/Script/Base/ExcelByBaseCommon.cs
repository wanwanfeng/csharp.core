using System.Collections.Generic;
using System.Linq;

namespace Library.Excel
{
    public abstract partial class ExcelByBase
    {
        public static readonly List<string> zimu = GetList();

        public static List<string> GetList()
        {
            char[] charArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var list = new List<string>(charArray.Select(p => p.ToString()));
            foreach (var c in charArray)
                list.AddRange(charArray.Select(p => c + p.ToString()));
            list.TrimExcess();
            return list;
        }
    }
}