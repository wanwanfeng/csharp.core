using System;
using System.Reflection;

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
    }
}