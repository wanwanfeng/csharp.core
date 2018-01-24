using UnityEngine;

namespace UnityEditor
{
    public abstract class BaseDoubleWindow<T> : BaseEditorWindow where T : BaseEditorWindow
    {
        private int selectId = 0;
        private T mainInstance;
        private T[] array;

        public string[] titleStr;
        public int mainWidth = 0;
        public int toolBarWidth = 0;
        public int toolBarHeight = 20;

        public virtual void Awake()
        {
            array = new T[titleStr.Length];
            MainInstance = SetMainInstance();
            CurInstance = SetInstance(0);
        }

        public override void OnDestroy()
        {
            titleStr = null;
            array = null;
        }

        public override void DrawGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(mainWidth));
            if (MainInstance)
            {
                MainInstance.DrawGUI();
            }
            GUILayout.EndVertical();

            this.SplitVertical(3);

            GUILayout.BeginVertical();
            GUILayout.Space(10);
            selectId = this.SetToolbar(titleStr, selectId, index =>
            {
                for (var i = 0; i < array.Length; i++)
                {
                    array[i] = i == index ? SetInstance(i) : null;
                }
            }, toolBarWidth == 0 ? Screen.width - mainWidth - 6 : toolBarWidth, toolBarHeight);
            this.SplitHorizontal(3);
            if (CurInstance)
            {
                CurInstance.DrawGUI();
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        public T MainInstance
        {
            get
            {
                mainWidth = (int) Mathf.Max(mainInstance.minSize.x, mainInstance.maxSize.x);
                return mainInstance;
            }
            set { mainInstance = value; }
        }

        public virtual T SetMainInstance()
        {
            return null;
        }

        public T CurInstance
        {
            get
            {
                return array[selectId];
            }
            set { array[selectId] = value; }
        }

        public virtual T SetInstance(int i)
        {
            return null;
        }
    }
}