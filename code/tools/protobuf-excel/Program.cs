using System.ComponentModel;
using Library;
using Library.Extensions;

namespace protobuf_excel
{
    class Program
    {
        private enum MyEnum
        {
            [Description("CreateProto2"), TypeValue(typeof(CreateProto20))] CreateProto2,
            [Description("WriteProto2"), TypeValue(typeof(WriteProto2))] WriteProto2,
            [Description("CreateProto3"), TypeValue(typeof(CreateProto3))] CreateProto3,
            [Description("WriteProto3"), TypeValue(typeof(WriteProto3))] WriteProto3,
        }

        static void Main(string[] args)
        {
            SystemConsole.Run<MyEnum>();
        }
    }
}
