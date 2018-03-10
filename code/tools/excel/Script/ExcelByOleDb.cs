using System.Collections.Generic;

namespace excel.Script
{
    public class ExcelByOleDb : ExcelByBase
    {
        public override Dictionary<string, List<List<object>>> ReadFromExcel(string filename)
        {
            //var dt = ExcelToTable(filename);
            return new Dictionary<string, List<List<object>>>()
            {
                //{filename, ConvertToList(dt)}
            };
        }

        public override void WriteToExcel(string filename, List<List<object>> vals)
        {
            var dt = ConvertToDataTable(vals);
            //TableToExcel(dt, filename);
        }
    }
}