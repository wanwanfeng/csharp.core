using System.ComponentModel;
using Library.Extensions;
using Script;

namespace Library.Excel
{
    public class Json2Excel
    {
        public enum CaoType
        {
            [Category("文件转换")] [Description("Excel->ToJson")] [TypeValue(typeof (ActionExcel.ToJson))] ExcelToJson,
            [Category("文件转换")] [Description("Json->ToExcel")] [TypeValue(typeof (ActionJson.ToExcel))] JsonToExcel
        }

        private static void Main(string[] args)
        {
            SystemConsole.Run<CaoType>();
        } 
    }
}