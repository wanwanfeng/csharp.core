using System;
using System.Collections.Generic;
using Library.Extensions;

namespace scenario_tools
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SystemConsole.Run(config: new Dictionary<string, Action>()
            {
                {"读取csv写入excel", () => { new ReadCsv(); }},
                {"读取excel写入json", () => { new ReadExcel(); }}
            });
        }
    }
}