using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;
using UnityEngine;

namespace Library.Helper
{
    public partial class PathHelper : DirectoryHelper
    {
#if UNITY
        public static string GetTempFileName()
        {
            return Application.persistentDataPath + Path.GetRandomFileName();
        }
#endif
    }
}
