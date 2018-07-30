using System.Data;
using System.Text.RegularExpressions;

namespace Library.Excel
{
    public abstract partial class ExcelByBase
    {
        public void HaHa(DataTable dt)
        {
            var headers = Data.GetHeaderList(dt);

            //DataTable dataTable = (DataTable) dt.Clone();
            //dataTable.DefaultView.ToTable()
            string regex = "";

            foreach (string header in headers)
            {
                //List<object> vals = new List<object>();
                //foreach (DataRow dr in dt.Rows)
                //{
                //    vals.Add(dr[header]);
                //}
                //var content = string.Join(",", vals.Select(p => p.ToString()).ToArray());
                //if (Regex.IsMatch(content, regex))
                //{

                //}

                foreach (DataRow dr in dt.Rows)
                {
                    var content = dr[header].ToString();
                    if (Regex.IsMatch(content, regex))
                    {
                        dr.Delete();
                        break;
                    }
                }
            }
            //dataTable.AcceptChanges();
        }
    }
}