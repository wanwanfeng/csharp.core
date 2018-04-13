using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Helper;

namespace Script
{
    public class BaseClass
    {
        private string _root;

        public string root
        {
            get
            {
                if (!string.IsNullOrEmpty(_root)) return _root;
                Console.Write("请拖入选定文件夹:");
                _root = Console.ReadLine();
                Console.WriteLine();
                Console.WriteLine("您选择的文件夹：" + _root);
                if (!string.IsNullOrEmpty(_root)) _root = _root.Replace("\\", "/");
                return _root;
            }
            set { _root = value; }
        }

        protected string Folder
        {
            get { return GetType().Name + "/"; }
            set { }
        }

        public string[] ReadAllLines(string name)
        {
            var path = Folder + name + ".txt";
            if (!File.Exists(path))
            {
                Console.WriteLine(path + " is not exist !");
                Console.ReadKey();
                return new string[0];
            }
            return File.ReadAllLines(path);
        }

        public void WriteAllLines(string name, string[] content)
        {
            var path = Folder + name + ".txt";
            FileHelper.CreateDirectory(path);
            File.WriteAllLines(path, content);
        }


        public void WriteAllLines(Dictionary<string,string> dic)
        {
            WriteAllLines("key", dic.Keys.ToArray());
            WriteAllLines("value", dic.Values.ToArray());
            //WriteAllLines("kv", dic.Select(p => p.Value + "\r\n" + p.Key).ToArray());
        }

        public void CreateDirectory(string name)
        {
            var path = GetType().Name + "/" + name;
            FileHelper.CreateDirectory(path);
        }


        public string GetExcelCell(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                int width = image.Width;
                int height = image.Height;


                int max = Math.Max(width, height);
                if (max == width)
                {
                    if (max > 350)
                    {
                        height = 350 * height / width;
                        width = 350;
                    }
                }
                else
                {
                    if (max > 350)
                    {
                        width = 350 * width / height;
                        height = 350;
                    }
                }
                return string.Format("<table><img src='{0}' width='{1}' height='{2}'>", path, width, height);
            }
        }

        protected void RunList(List<string> res)
        {
            var index = 0;
            foreach (string re in res)
            {
                Console.WriteLine((((float)index++) / res.Count).ToString("p") + "\t" + re);
                if (!File.Exists(re)) continue;

                RunListOne(re);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="re">全路径包含root</param>
        public virtual void RunListOne(string re)
        {
            
        }
    }
}