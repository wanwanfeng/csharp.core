using System;
using UnityEngine;

namespace UnityEditor
{
    public static class ExtensionsForUnityEditor
    {
        #region 基本控件封装

        public static void SplitSeparator(this ScriptableObject target, int count = 1)
        {
            if (count <= 0) return;
            for (int i = 0; i < count; i++)
            {
                EditorGUILayout.Separator();
            }
        }

        public static void SplitHorizontal(this ScriptableObject target, int height)
        {
            GUILayout.BeginHorizontal("AS TextArea", GUILayout.Height(height));
            GUILayout.EndHorizontal();
        }

        public static void SplitVertical(this ScriptableObject target, int width)
        {
            GUILayout.BeginVertical("AS TextArea", GUILayout.Width(width));
            GUILayout.EndVertical();
        }

        public static void Horizontal(this ScriptableObject target, Action callAction, int spaceH = 10, int spaceV = 0)
        {
            GUILayout.Space(spaceV);
            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(20));
            {
                GUILayout.Space(spaceH);
                callAction();
                GUILayout.Space(spaceH);
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void HorizontalAsTextArea(this ScriptableObject target, Action callAction, int spaceH = 10,
            int spaceV = 0)
        {
            GUILayout.Space(spaceV);
            EditorGUILayout.BeginHorizontal("As TextArea", GUILayout.MaxHeight(10));
            {
                GUILayout.Space(spaceH);
                callAction();
                GUILayout.Space(spaceH);
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void HorizontalAsToolBar(this ScriptableObject target, Action callAction, int spaceH = 10, int spaceV = 0)
        {
            GUILayout.Space(spaceV);
            EditorGUILayout.BeginHorizontal("Toolbar", GUILayout.MinHeight(10));
            {
                GUILayout.Space(spaceH);
                callAction();
                GUILayout.Space(spaceH);
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void Vertical(this ScriptableObject target, Action callAction, int spaceH = 10)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(spaceH);
                EditorGUILayout.BeginVertical();
                {
                    callAction();
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(spaceH);
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void SetButton(this ScriptableObject target, string msg, Action action, int width, GUIStyle style = null)
        {
            SetButton(target, msg, action, style, GUILayout.MaxWidth(width));
        }

        public static void SetButton(this ScriptableObject target, string msg, Action action, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(msg, options))
                SetButtonAction(target, action);
        }

        public static void SetButton(this ScriptableObject target, string msg, Action action, GUIStyle style,
            params GUILayoutOption[] options)
        {
            if (style == null)
            {
                if (GUILayout.Button(msg, "button", options))
                    SetButtonAction(target, action);
            }
            else
            {
                if (GUILayout.Button(msg, style, options))
                    SetButtonAction(target, action);
            }
        }

        private static void SetButtonAction(this ScriptableObject target, Action action)
        {
            if (action != null)
                action();
            SaveAssets(target);
            SetFocus(target);
        }

        public static Enum SetEnumPopup(this ScriptableObject target, Enum t, Action<Enum> action, int width = 0,
            GUIStyle style = null)
        {
            Enum selected = t;
            if (style == null)
            {
                selected = width == 0
                    ? EditorGUILayout.EnumPopup(selected)
                    : EditorGUILayout.EnumPopup(selected, GUILayout.MaxWidth(width));
            }
            else
            {
                selected = width == 0
                    ? EditorGUILayout.EnumPopup(selected, style)
                    : EditorGUILayout.EnumPopup(selected, style, GUILayout.MaxWidth(width));
            }

            if (selected.Equals(t)) return t;
            t = selected;
            if (action != null)
                action(t);
            SaveAssets(target);
            SetFocus(target);
            return t;
        }

        public static int SetPopup(this ScriptableObject target, string[] list, int t, Action<int> action, int width = 0, GUIStyle style = null)
        {
            int selected = t;
            if (style == null)
            {
                selected = width == 0
                    ? EditorGUILayout.Popup(selected, list)
                    : EditorGUILayout.Popup(selected, list, GUILayout.MaxWidth(width));
            }
            else
            {
                selected = width == 0
                    ? EditorGUILayout.Popup(selected, list, style)
                    : EditorGUILayout.Popup(selected, list, style, GUILayout.MaxWidth(width));
            }

            if (selected.Equals(t)) return t;
            t = selected;
            if (action != null)
                action(t);
            SaveAssets(target);
            SetFocus(target);
            return t;
        }

        public static bool SetToggle(this ScriptableObject target, string msg, bool t, Action<bool> action, int width = 0)
        {
            bool toggle = width <= 0 ? GUILayout.Toggle(t, msg) : GUILayout.Toggle(t, msg, GUILayout.MaxWidth(width));
            if (toggle == t) return t;
            t = toggle;
            if (action != null)
                action(t);
            SaveAssets(target);
            SetFocus(target);
            return t;
        }

        public static int SetToolbar(this ScriptableObject target, string[] msg, int t, Action<int> action, int width = 0, int height = 20)
        {
            int selectId = t;
            if (width == 0)
            {
                selectId = GUILayout.Toolbar(selectId, msg, GUILayout.Height(height));
            }
            else
            {
                selectId = GUILayout.Toolbar(selectId, msg, GUILayout.Width(width), GUILayout.Height(height));
            }
            if (selectId.Equals(t)) return t;
            t = selectId;
            if (action != null)
                action(t);
            SaveAssets(target);
            SetFocus(target);
            return t;
        }

        public static int SetSelectionGrid(this ScriptableObject target, string[] msg, int t, Action<int> action,
            int width = 100, int height = 20)
        {
            int selectId = t;
            if (width == 0)
            {
                selectId = GUILayout.SelectionGrid(selectId, msg, 1, GUILayout.Height(height*msg.Length));
            }
            else
            {
                selectId = GUILayout.SelectionGrid(selectId, msg, 1, GUILayout.Width(width),
                    GUILayout.Height(height*msg.Length));
            }
            if (selectId.Equals(t)) return t;
            t = selectId;
            if (action != null)
                action(t);
            SaveAssets(target);
            SetFocus(target);
            return t;
        }

        private static void SaveAssets(ScriptableObject target)
        {
            var window = target as BaseWindow;
            if (window != null)
            {
                window.SaveAssets();
            }
            else
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private static void SetFocus(ScriptableObject target)
        {
            var window = target as EditorWindow;
            if (window != null)
            {
                window.Focus();
            }
        }

        public static void SetSelectableLabel(this BaseWindow target, string controlName, string msg, string path, params GUILayoutOption[] option)
        {
            GUI.SetNextControlName(controlName);
            EditorGUILayout.SelectableLabel(msg, option);
            target.focused = GUI.GetNameOfFocusedControl();
            if (string.IsNullOrEmpty(target.focused))
            {
                return;
            }
            if (!target.focused.StartsWith(controlName)) return;
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof (UnityEngine.Object));
            //if (Selection.activeObject == null)
            //{
            //    EditorUtility.DisplayDialog("错误", "路径资源不存在！", "确定");
            //}
        }

        public static void SetSelectableLabel2(this BaseWindow target, string controlName, string msg, string path,
            params GUILayoutOption[] option)
        {
            GUI.SetNextControlName(controlName);
            EditorGUILayout.SelectableLabel(msg, option);
            target.focused = GUI.GetNameOfFocusedControl();
            if (string.IsNullOrEmpty(target.focused))
            {
                return;
            }
            if (!target.focused.StartsWith(controlName)) return;
            Selection.activeObject = AssetDatabase.LoadAllAssetsAtPath(path)[0];
            //if (Selection.activeObject == null)
            //{
            //    EditorUtility.DisplayDialog("错误", "路径资源不存在！", "确定");
            //}
        }

        #endregion
    }
}
