using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Library.Helper;

namespace Library.Excel
{
    /// <summary>
    /// DataTable与CSV
    /// </summary>
    public abstract partial class ExcelByBase
    {
        #region  Convert Csv and DataTable

        public static DataTable ImportCsvToDataTable(string path)
        {
            path = Path.ChangeExtension(path, ".csv");
            if (!File.Exists(path))
                Ldebug.Log("文件不存在!");
            if (path == null) return null;

            //string[] content = File.ReadAllLines(path);
            //var list =
            //    content.Select(
            //        q =>
            //        {
            //            CsvHelper.CsvFileReader

            //            return q.Split(',')
            //                .Select(p => p.StartsWith("\"") ? p.Substring(1, p.Length - 2) : p)
            //                .Cast<object>()
            //                .ToList();
            //        }).ToList();

            //var countList = list.Select(p => p.Count()).ToList();
            //if (countList.Min() != countList.Max())
            //{
            //    Ldebug.Log("\n--------------------------------");
            //    Ldebug.Log("文件格式有问题!");
            //    Ldebug.Log("--------------------------------\n");
            //    SystemExtensions.QuitReadKey();
            //}

            var list = CsvHelper.ReadCSV(path);
            return ConvertListToDataTable(new ListTable()
            {
                TableName = Path.GetFileNameWithoutExtension(path),
                FullName = path,
                IsArray = true,
                Key = list.First(),
                List = list.Skip(1).ToList()
            });
        }

        public static void ExportDataTableToCsv(DataTable dt, string file)
        {
            file = string.IsNullOrEmpty(file) ? dt.FullName : file;

            var list = ConvertDataTableToList(dt);
            var contents = list.List.Select(p => string.Join(",", p.Select(q =>
            {
                //var str = q.ToString().Replace("\"", "\"\""); //替换英文冒号 英文冒号需要换成两个冒号
                //if (str.Contains(',') || str.Contains('\"') || str.Contains('\r') || str.Contains('\n'))
                //{
                //    //含逗号 冒号 换行符的需要放到引号中
                //    str = string.Format("\"{0}\"", str);
                //}
                //return str;
                return string.Format("\"{0}\"", q); ;
            }).ToArray())).ToArray();
            string newPath = Path.ChangeExtension(file, ".csv");
            FileHelper.CreateDirectory(newPath);
            File.WriteAllLines(newPath, contents, new UTF8Encoding(false));
            //CsvHelper.SaveCSV(list, newPath);
        }

        #endregion
    }
}