using System;
using fontConvert.Script;
using Library.Extensions;
using Library.Helper;

namespace fontConvert
{
    public enum ConvertType
    {
        [TypeValue(typeof (ActionByDLL.ToTraditional))] ToTraditional_Dll = 1,
        [TypeValue(typeof (ActionByDLL.ToSimplified))] ToSimplified_Dll,
        [TypeValue(typeof (ActionByArray.ToTraditional))] ToTraditional_Array,
        [TypeValue(typeof (ActionByArray.ToSimplified))] ToSimplified_Array,
        [TypeValue(typeof (ActionByVB.ToTraditional))] ToTraditional_VB,
        [TypeValue(typeof (ActionByVB.ToSimplified))] ToSimplified_VB,
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            do
            {
                Console.Clear();
                Console.WriteLine("-------简繁体转换命令列表-------\n");
                foreach (var value in Enum.GetValues(typeof (ConvertType)))
                {
                    Console.WriteLine("\t" + (int) value + ":" + value);
                }
                Console.WriteLine("\n--------------------------------");

                var cache = AttributeHelper.GetCacheTypeValue<ConvertType>();
                ConvertType convertType = (ConvertType) SystemExtensions.GetInputStr().AsInt();

                BaseActionBy baseActionFor = (BaseActionBy) Activator.CreateInstance(cache[convertType]);
                baseActionFor.Open(SystemExtensions.GetInputStr("input dir path:"),
                    SystemExtensions.GetInputStr("input file extension ('.cs'):"));
            } while (SystemExtensions.ContinueY());
        }
    }
}
