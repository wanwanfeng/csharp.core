using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Library.Helper
{
    public class FileHelper
    {
        /// <summary>
        /// 创建父级目录
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectory(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
    }
}
