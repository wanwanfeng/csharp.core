using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor
{
    public abstract class BaseBarWindow<T> : BaseEditorWindow where T : BaseEditorWindow
    {
        protected int selectId = 0;
        protected int toolBarWidth = 0;

        protected Dictionary<string, Func<T>> dic;
        protected T[] array;

        public virtual void Awake()
        {
            array = new T[dic.Count];
            selectId = Mathf.Min(EditorPrefs.GetInt(GetType().Name, 0), array.Length - 1);
            CurInstance = SetInstance(selectId);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            dic = null;
            array = null;
        }

        public T CurInstance
        {
            get { return array[selectId]; }
            set
            {
                array[selectId] = value;
                value.FocusAction = Focus;
            }
        }

        public T SetInstance(int i)
        {
            if (i > dic.Count)
            {
                Debug.LogError("数组越界！");
                return null;
            }
            var func = dic.Values.ElementAtOrDefault(i);
            return func == null ? null : func.Invoke();
        }

        public override void SaveAssets()
        {

        }

        public override void DrawGUI()
        {
            if (dic == null)
                Awake();
        }
    }

    public abstract class BaseToolBarWindow<T> : BaseBarWindow<T> where T : BaseEditorWindow
    {
        public override void DrawGUI()
        {
            base.DrawGUI();
            this.Horizontal(() =>
            {
                selectId = this.SetToolbar(dic.Keys.ToArray(), selectId, (index) =>
                {
                    EditorPrefs.SetInt(this.GetType().Name, index);
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = (i == index ? SetInstance(i) : null);
                    }
                }, toolBarWidth == 0 ? Screen.width - 12 : toolBarWidth);
            }, 6, 10);

            this.SplitHorizontal(3);
            GUILayout.Space(2);
            GUILayout.BeginVertical();
            if (CurInstance)
            {
                CurInstance.DrawGUI();
            }
            EditorGUILayout.EndVertical();
        }
    }

    public abstract class BaseGridBarWindow<T> : BaseBarWindow<T> where T : BaseEditorWindow
    {
        public override void DrawGUI()
        {
            this.Horizontal(() =>
            {
                GUILayout.BeginVertical();
                selectId = this.SetSelectionGrid(dic.Keys.ToArray(), selectId, (index) =>
                {
                    EditorPrefs.SetInt(this.GetType().Name, index);
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = (i == index ? SetInstance(i) : null);
                    }
                }, toolBarWidth);
                EditorGUILayout.EndVertical();

                this.SplitVertical(3);
                GUILayout.Space(2);

                GUILayout.BeginVertical();
                if (CurInstance)
                {
                    CurInstance.DrawGUI();
                }
                EditorGUILayout.EndVertical();
            }, 5, 0);
        }
    }
}