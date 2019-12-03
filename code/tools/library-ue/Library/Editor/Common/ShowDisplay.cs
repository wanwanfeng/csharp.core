using System;
using System.Linq;

namespace UnityEditor
{
    public class ShowDisplay : Exception
    {
        private static bool Enable = true;
        static ShowDisplay()
        {
            if (Environment.GetCommandLineArgs().Select(p => p.Trim()).Contains("-batchmode"))
                Enable = false;
            else
                Enable = true;
            EditorUtility.ClearProgressBar();
        }
        public ShowDisplay(string message = "") : base(message)
        {
            EditorUtility.ClearProgressBar();
        }
        public static void CancelableProgressBar(string title, string info, float progress)
        {
            if (!Enable) return;
            bool cancel = EditorUtility.DisplayCancelableProgressBar(title, info, progress);
            if (cancel) throw new ShowDisplay("DisplayCancelableProgressBar is Cancel !!!!!");
        }
        public static void CancelableProgressBar(string title, string info, float index, int count)
        {
            if (!Enable) return;
            bool cancel = EditorUtility.DisplayCancelableProgressBar(title, string.Format("【{0}/{1}】{2}", index + 1, count, info), (index + 1) / count);
            if (cancel) throw new ShowDisplay("DisplayCancelableProgressBar is Cancel !!!!!");
        }
        public static void Dialog(string message, string title = "tips")
        {
            if (!Enable) return;
            bool ok = EditorUtility.DisplayDialog(title, message, "Ok", "Cancel");
            if (!ok) throw new ShowDisplay("DisplayDialog is Cancel !!!!!");
        }
        public static void DialogOK(string message, string title = "tips")
        {
            if (!Enable) return;
            EditorUtility.DisplayDialog(title, message, "Ok");
        }
        public static void DisplayDialog(string output, string message)
        {
            if (!Enable) return;
            if (EditorUtility.DisplayDialog(output, message, "Ok", "Cancel"))
                EditorUtility.RevealInFinder(output);
        }
    }
}