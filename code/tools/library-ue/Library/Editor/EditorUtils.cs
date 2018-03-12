﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Helper;
using UnityEngine;

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
            TextEditor te = new TextEditor();
#if UNITY_5_3||UNITY_5_4||UNITY_5_5
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

        #region File

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<string> FileRead(string fileName)
        {
            string outpath = Application.dataPath + fileName;
            if (!Path.HasExtension(fileName))
                outpath += ".txt";
            outpath = outpath.Replace("\\", "/");
            List<string> res = new List<string>();
            if (File.Exists(outpath))
                res = new List<string>(File.ReadAllLines(outpath));
            else
                Debug.LogError("文件不存在！" + outpath);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">包括文件后缀</param>
        /// <param name="res"></param>
        /// <param name="isPopup"></param>
        public static void FileWrite(string fileName, string[] res, bool isPopup = true)
        {
            string outpath = Application.dataPath + fileName;
            if (!Path.HasExtension(fileName))
                outpath += ".txt";
            outpath = outpath.Replace("\\", "/");
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
            string outpath = Application.dataPath + fileName;
            if (!Path.HasExtension(fileName))
                outpath += ".txt";
            outpath = outpath.Replace("\\", "/");
            FileHelper.CreateDirectory(outpath);
            File.WriteAllText(outpath, res);
            if (isPopup && EditorUtility.DisplayDialog(outpath, "是否打开文件查看信息？", "确定", "取消"))
                EditorUtility.OpenWithDefaultApp(outpath);
        }

        public static string[] GetFiles(out string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            path = EditorUtility.OpenFolderPanel("", "", "");
            if (string.IsNullOrEmpty(path))
                return new string[0];
            return Directory.GetFiles(path, "*", searchOption).Select(p => p.Replace("\\", "/")).ToArray();
        }

        #endregion

        public static string DataPath
        {
            get { return Application.dataPath.Replace("Assets", ""); }
        }

        public static string ProjectSettingsPath
        {
            get { return GetProjectPath("ProjectSettings"); }
        }

        public static string GetProjectPath(string path = null)
        {
            string savePath = "./";
            if (!string.IsNullOrEmpty(path))
                savePath = savePath + path + "/";
            FileHelper.CreateDirectory(savePath);
            return savePath;
        }
    }
}