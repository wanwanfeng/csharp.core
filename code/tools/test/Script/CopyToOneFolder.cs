﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace Script
{
    /// <summary>
    /// 把图片搜集到一个文件夹方便剔除
    /// </summary>
    public class CopyToOneFolder : BaseClass
    {
        /// <summary>
        /// true 归一到一个文件夹下
        /// false 还原
        /// </summary>
        private bool guiyi = false;

        public CopyToOneFolder()
        {
            List<string> res = new List<string>();

            Console.WriteLine("----------------------");
            Console.WriteLine("1:归一到一个文件夹下");
            Console.WriteLine("2:还原");
            Console.WriteLine("----------------------");
            var y = Console.ReadLine();

            switch (y)
            {
                case "1":
                {
                    guiyi = true;
                    var eList = ".png|.jpg|.bmp|.psd|.tga|.tif|.dds".AsStringArray('|').ToList();
                    res = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                        .Where(p => eList.Contains(Path.GetExtension(p)))
                        .Select(p => p.Replace("\\", "/"))
                        .ToList();
                    break;
                }
                case "2":
                {
                    guiyi = false;
                    res =
                        Directory.GetFiles(root, "*", SearchOption.TopDirectoryOnly)
                            .Select(p => p.Replace("\\", "/"))
                            .ToList();
                    break;
                }
            }
            if (res.Count == 0) return;
            res.Sort();
            RunList(res);
        }

        public override void RunListOne(string re)
        {
            if (guiyi)
            {
                CopyToOne(re);
            }
            else
            {
                RevertCopyToOne(re);
            }
        }

        private void CopyToOne(string re)
        {
            string haha = re;
            if (haha.StartsWith(root))
                haha = haha.Replace(root, "");

            //唯一文件夹路径
            string newPath = root + "_merge/" + haha.Replace("/", "..");
            FileHelper.CreateDirectory(newPath);
            File.Copy(root + haha, newPath, true);
        }

        /// <summary>
        /// 全路径
        /// </summary>
        /// <param name="re"></param>
        private void RevertCopyToOne(string re)
        {
            string haha = re;
            if (haha.StartsWith(root))
                haha = haha.Replace(root, "");

            string newPath = Folder + haha.Replace("..", "/");
            FileHelper.CreateDirectory(newPath);
            File.Copy(re, newPath, true);
        }
    }
}