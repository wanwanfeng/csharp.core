using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Library.Extensions;

namespace Script
{
    internal class GetLineCount
    {
        public GetLineCount()
        {
            do
            {
                var folder = SystemConsole.GetInputStr("请拖入文件夹：");
                if (!Directory.Exists(folder))
                    continue;
                var ex = SystemConsole.GetInputStr("请输入文件后缀(如\"cs,cpp\"：");
                if (string.IsNullOrEmpty(folder))
                    continue;

                var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories).ToList();
                var res = new List<string>();
                ex.Split(',').ToList().ForEach(s => res.AddRange(files.Where(p => Path.GetExtension(p) == s)));
                File.WriteAllLines(ex + ".txt", res.ToArray());
                Console.WriteLine("总行数：" + res.Sum(p => File.Exists(p) ? File.ReadAllLines(p).Length : 0));
            } while (SystemConsole.ContinueY());
        }
    }
}
