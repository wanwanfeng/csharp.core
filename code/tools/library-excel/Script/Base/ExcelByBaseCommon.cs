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
            string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var array = str.ToCharArray().Select(p => p.ToString()).ToArray();
            List<string> list = new List<string>(array);
            foreach (char c in str)
                list.AddRange(str.ToCharArray().Select(p => c + p.ToString()).ToArray());
            list.TrimExcess();
            return list;
        }
    }
}