using System.ComponentModel;
using checkcard.Scripts;
using Library;
using Library.Extensions;

namespace checkcard
{
    internal class Program
    {
        private enum MyEnum
        {
            [Description("卡片检测"), TypeValue(typeof (CheckCard))] CheckCard,
            [Description("剧情符号检测"), TypeValue(typeof (CheckScenario))] CheckScenario,
            [Description("剧情导出"), TypeValue(typeof (ExportScenario))] ExportScenario,
        }

        private static void Main(string[] args)
        {
            SystemConsole.Run<MyEnum>();
        }
    }
}
