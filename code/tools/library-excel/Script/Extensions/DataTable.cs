namespace Library.Excel
{
    public class DataTable : System.Data.DataTable
    {
        public bool IsArray = true;
        public string FullName = "";

        public DataTable(string table)
            : base(table)
        {
        }

        public DataTable()
        {

        }
    }
}