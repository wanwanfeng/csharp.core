using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;

namespace test
{
    internal class GetLineCount
    {
        public GetLineCount()
        {
            do
            {
                var folder = SystemExtensions.GetInputStr("请拖入文件夹：");
                if (!Directory.Exists(folder))
                    continue;
                var ex = SystemExtensions.GetInputStr("请输入文件后缀(如\"cs,cpp\"：");
                if (string.IsNullOrEmpty(folder))
                    continue;

                var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories).ToList();
                var res = new List<string>();
                foreach (var s in ex.Split(',').ToList())
                {
                    res.AddRange(files.Where(p => Path.GetExtension(p) == s));
                }
                File.WriteAllLines(ex + ".txt", res.ToArray());
                var sum = 0;
                res.ForEach(p =>
                {
                    if (File.Exists(p))
                        sum += File.ReadAllLines(p).Length;
                });
                Console.WriteLine("总行数：" + sum);
            } while (SystemExtensions.ContinueY());
        }
    }
}
