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
            try
            {
                GetValue();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                //exception.StackTrace.Split('\n').ToList().ForEach(Console.WriteLine);
            }
            Console.ReadKey();
        }

        private static void GetValue()
        {
            var fileName = Path.GetFullPath("MessageHandler.txt");
            var content = File.ReadAllLines(fileName).ToList();
            if (!File.Exists(fileName))
                throw new Exception(string.Format("[{0}] is not exist!!!", fileName));

            var dirName = Path.GetFullPath("../meta/msg/");
            if (!Directory.Exists(dirName))
                throw new Exception(string.Format("[{0}] is not exist!!!", dirName));

            var xx =
                Directory.GetFiles(dirName, "*.proto")
                    .Select(p => Path.GetFileNameWithoutExtension(p).Split('_'))
                    .Where(p => p.Length > 1)
                    .Select(p => p[1]).ToList();
            var list =
                xx.Select(
                    p => p[0].ToString().ToUpper() + string.Join("", p.Skip(1).Select(q => q.ToString()).ToArray()) + "Mod")
                    .ToList();

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

            var newName = Path.ChangeExtension(fileName, ".cs");
            File.WriteAllLines(newName, content.ToArray());
            Console.WriteLine("create [" + newName + "] success !!!!!");
        }
    }
}
