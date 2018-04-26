using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private bool guiyi = true;

        public CopyToOneFolder()
        {
            List<string> res = new List<string>();

            if (guiyi)
            {
                root = "D:/Work/mfxy/ron_mfsn2/";
                //root = "D:/Work/yuege/res/app/assets/app/daifanyi/image_new/";
                res = Directory.GetFiles(root + "cocostudio/演出/", "*", SearchOption.AllDirectories)
                    .Where(p => p.Contains("cocos_Data"))
                    .Where(p => p.Contains("Resources"))
                    .Where(p => p.EndsWith("png") || p.EndsWith("jpg"))
                    .Select(p => p.Replace("\\", "/"))
                    .ToList();


                //res = res.Take(200).ToList();
                // res = new[] {"D:/Work/mfxy/ron_mfsn2/cocostudio/演出/ADV/cocos_Data/3001_fog/Resources/adv_フェリシア.png"};

                root = @"D:\婚纱照\结果 - 副本\".Replace("\\", "/");
                res =
                    Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                        .Select(p => p.Replace("\\", "/"))
                        .ToList();
            }
            else
            {
                root = Folder;
                res =
                    Directory.GetFiles(root, "*", SearchOption.TopDirectoryOnly)
                        .Select(p => p.Replace("\\", "/"))
                        .ToList();
            }
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
            string newPath = Folder + haha.Replace("/", "..");
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