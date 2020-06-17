using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Helper;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UnityEditor.Library
{
    public partial class EditorUtils
    {
        /// <summary>
        /// 内容复制到剪切板
        /// </summary>
        /// <param name="content"></param>
        public static void CopyToClipboard(string content)
        {
            Debug.Log(content);
            try
            {
                TextEditor te = new TextEditor();
#if UNITY_5_3_OR_NEWER
            te.text = content;
#else
                te.content = new GUIContent()
                {
                    text = content
                };
#endif
                te.SelectAll();
                te.Copy();
            }
            catch
            {
                // ignored
            }
        }

        #region File

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<string> FileRead(string fileName)
        {
            string outpath = new Uri(Path.ChangeExtension(fileName, ".txt")).LocalPath;
            if (File.Exists(outpath)) return File.ReadAllLines(outpath).ToList();
            throw new Exception("文件不存在！" + outpath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">包括文件后缀</param>
        /// <param name="res"></param>
        /// <param name="isPopup"></param>
        public static void FileWrite(string fileName, string[] res, bool isPopup = true)
        {
            string outpath = new Uri(Path.ChangeExtension(fileName, ".txt")).LocalPath;
            FileHelper.CreateDirectory(outpath);
            File.WriteAllLines(outpath, res);
            if (isPopup && EditorUtility.DisplayDialog(outpath, "是否打开文件查看信息？", "确定", "取消"))
                EditorUtility.OpenWithDefaultApp(outpath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">包括文件后缀</param>
        /// <param name="res"></param>
        /// <param name="isPopup"></param>
        public static void FileWrite(string fileName, string res, bool isPopup = true)
        {
            string outpath = new Uri(Path.ChangeExtension(fileName, ".txt")).LocalPath;
            FileHelper.CreateDirectory(outpath);
            File.WriteAllText(outpath, res);
            if (isPopup && EditorUtility.DisplayDialog(outpath, "是否打开文件查看信息？", "确定", "取消"))
                EditorUtility.OpenWithDefaultApp(outpath);
        }

        public static string[] OpenFolderGetFiles(out string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            path = EditorUtility.OpenFolderPanel("", "", "");
            return GetFiles(path, searchOption);
        }

        public static string[] GetFiles(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (string.IsNullOrEmpty(path))
                return new string[0];
            return Directory.GetFiles(path, "*", searchOption).Select(p => p.Replace("\\", "/")).ToArray();
        }

        #endregion
    }
}
