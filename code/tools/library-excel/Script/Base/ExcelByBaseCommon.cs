using System.Collections.Generic;
using System.Linq;

namespace Library.Excel
{
    public abstract partial class ExcelByBase
    {
        public enum LineType
        {
            FieldName,
            FieldType,
            FieldValue,
        }

        #region virtual


        public virtual List<ListTable> ImportExcelToListTable(string filename)
        {
            return null;
        }

        public virtual void ExportToExcel(string filename, ListTable list)
        {

        }

        public virtual void ExportToOneExcel(string fileName, List<ListTable> list)
        {

        }

        #endregion

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