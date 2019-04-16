using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace protobuf
{
    class Program
    {
        /// <summary>
        /// protobuf 模板类生成
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var content = File.ReadAllLines("MessageHandler.txt").ToList();
            var xx = Directory.GetFiles("meta/msg/", "*.proto").Select(p => Path.GetFileNameWithoutExtension(p).Split('_')).Where(p => p.Length > 1)
                .Select(p => p[1]).ToList();
            var list = xx.Select(p => p[0].ToString().ToUpper() + string.Join("", p.Skip(1).Select(q => q.ToString()).ToArray()) + "Mod").ToList();

            for (int i = content.Count - 1; i >= 0; i--)
            {
                var temp = content[i];
                if (temp.Contains("{0}"))
                {
                    content.RemoveAt(i);
                    for (int j = 0; j < list.Count; j++)
                    {
                        content.Insert(i, temp.Replace("{0}", list[j]));
                        i++;
                    }

                    i -= list.Count;
                }
            }

            File.WriteAllLines("MessageHandler.cs", content.ToArray());
        }
    }
}
