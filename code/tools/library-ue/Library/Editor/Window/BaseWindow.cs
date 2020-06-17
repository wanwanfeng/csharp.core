using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor.Library;

namespace UnityEditor
{
    public class BaseWindow : EditorWindow
    {
        private void GetInstance(object value)
        {
            FieldInfo fieldInfo = this.GetType().GetField("Instance", BindingFlags.Static | BindingFlags.Public);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(this, value);
            }
        }

        #region 焦点控制

        public Action FocusAction { get; set; }

        /// <summary>
        /// 获得焦点时
        /// </summary>
        public virtual void OnFocus()
        {
            //Debug.Log("获得焦点");
            GetInstance(this);
            if (FocusAction != null)
            {
                FocusAction.Invoke();
            }
        }

        public virtual void OnLostFocus()
        {
            //Debug.Log("失去焦点");
        }

        public virtual void OnDestroy()
        {
            //Debug.Log("销毁");
            GetInstance(null);
        }

        #endregion

        #region 基本控件封装

        public string focused;

        public virtual void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion

        #region File

        private static string GetFileName(string fileName)
        {
            var traceName = Path.GetFileNameWithoutExtension(new StackTrace(true).GetFrame(2).GetFileName());
            return Path.Combine("log", Path.Combine(traceName, fileName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<string> FileRead(string fileName)
        {
            return EditorUtils.FileRead(GetFileName(fileName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">包括文件后缀</param>
        /// <param name="res"></param>
        /// <param name="isPopup"></param>
        public static void FileWrite(string fileName, string[] res, bool isPopup = true)
        {
            EditorUtils.FileWrite(GetFileName(fileName), res, isPopup);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">包括文件后缀</param>
        /// <param name="res"></param>
        /// <param name="isPopup"></param>
        public static void FileWrite(string fileName, string res, bool isPopup = true)
        {
            EditorUtils.FileWrite(GetFileName(fileName), res, isPopup);
        }

        #endregion
    }
}