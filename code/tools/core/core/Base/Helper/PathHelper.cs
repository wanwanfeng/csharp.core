using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;

namespace Library.Helper
{
    public partial class PathHelper : DirectoryHelper
    {
#if !UNITY
        public static string GetTempFileName()
        {
            return Path.GetTempFileName();
    }
#endif
    }
}
