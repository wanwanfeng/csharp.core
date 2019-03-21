
using System.ComponentModel;
using Library;
using Library.Extensions;
using search.Script;

namespace search
{
    internal class Program
    {
        public enum ConvertType
        {
            [TypeValue(typeof (SearchIP))] [Description("获取代理IP")] SearchIP,
            [TypeValue(typeof (SearchMovie))] [Description("获取代理IP")] SearchMovie,
        }

        private static void Main(string[] args)
        {
            SystemConsole.Run<ConvertType>();
        }
    }
}
