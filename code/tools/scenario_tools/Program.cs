using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Excel;
using Library.Extensions;
using LitJson;

namespace scenario_tools
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            switch (SystemExtensions.GetInputStr("1:读取csv写入excel\n2:读取excel写入json\ninput:"))
            {
                case "1":
                    new ReadCsv();
                    break;
                case "2":
                    new ReadExcel();
                    break;
            }
        }
    }
}