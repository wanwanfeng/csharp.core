using UnityEngine;

namespace UnityEditor
{
    public class BaseEditorWindow : BaseWindow
    {
        #region 焦点控制

        /// <summary>
        /// 获得焦点时
        /// </summary>
        public override void OnFocus()
        {
            if (singleWindow)
            {
                base.OnFocus();
            }
        }

        public override void OnLostFocus()
        {
            if (singleWindow)
            {
                base.OnLostFocus();
            }
        }

        #endregion

        public bool singleWindow;
        public int tempHeight = 100;
        public void OnGUI()
        {
            if (singleWindow)
            {
                DrawGUI();
            }
        }

        public virtual void DrawGUI()
        {
            tempHeight = singleWindow ? 0 : 40;
        }

        public static T 新Window<T>(bool utility, string title, bool singleWindow) where T : BaseEditorWindow
        {
            var instance = singleWindow ? GetWindow<T>(utility, title) : CreateInstance<T>();
            instance.singleWindow = singleWindow;
            //if (singleWindow)
            {
                instance.minSize = instance.maxSize = new Vector2(300, 450);
            }
            return instance;
        }

        public static T 新Window<T>(bool singleWindow) where T : BaseEditorWindow
        {
            var instance = singleWindow ? GetWindow<T>(typeof(T).Name) : CreateInstance<T>();
            instance.singleWindow = singleWindow;
            return instance;
        }
    }
}
