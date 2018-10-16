using System;
using System.Collections.Generic;

namespace Library
{
    public static class Utils
    {
        private static Dictionary<string, string> _typeCache = new Dictionary<string, string>()
        {
            {"bool", "System.Boolean"},
            {"byte", "System.Byte"},
            {"sbyte", "System.SByte"},
            {"char", "System.Char"},
            {"decimal", "System.Decimal"},
            {"double", "System.Double"},
            {"float", "System.Single"},
            {"int", "System.Int32"},
            {"uint", "System.UInt32"},
            {"long", "System.Int64"},
            {"ulong", "System.UInt64"},
            {"object", "System.Object"},
            {"short", "System.Int16"},
            {"ushort", "System.UInt16"},
            {"string", "System.String"},
        };


        static Utils()
        {
        }
    }
}