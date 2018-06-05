using System;
using System.ComponentModel;
using fontConvert.Script;
using Library.Extensions;
using Library.Helper;

namespace fontConvert
{
    public enum ConvertType
    {
        [Description("简体转换为繁体(Dll)")][TypeValue(typeof (ActionByDLL.ToTraditional))] ToTraditional_Dll = 1,
        [Description("繁体转换为简体(Dll)")][TypeValue(typeof (ActionByDLL.ToSimplified))] ToSimplified_Dll,
        [Description("简体转换为繁体(Array)")][TypeValue(typeof (ActionByArray.ToTraditional))] ToTraditional_Array,
        [Description("繁体转换为简体(Array)")][TypeValue(typeof (ActionByArray.ToSimplified))] ToSimplified_Array,
        //[TypeValue(typeof (ActionByVB.ToTraditional))] ToTraditional_VB,
        //[TypeValue(typeof (ActionByVB.ToSimplified))] ToSimplified_VB,
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Action<object> callAction = delegate(object o)
            {
                BaseActionBy baseActionFor = (BaseActionBy) o;
                var path = SystemExtensions.GetInputStr("input dir path:");
                var ex = SystemExtensions.GetInputStr("input file extension ('.cs'):");
                baseActionFor.Open(path, ex);
            };
            SystemConsole.Run<ConvertType>(callAction);
        }
    }
}
