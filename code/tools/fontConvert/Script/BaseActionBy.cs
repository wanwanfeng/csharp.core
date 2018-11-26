using System;
using System.IO;
using Library.Extensions;

namespace fontConvert
{
    public abstract class BaseActionBy : BaseSystemConsole
    {
        public void Open()
        {
            CheckPath("*.*", SelectType.All).ForEach(OpenRun, "替换中...请稍后");
        }

        protected virtual void OpenRun(string path)
        {
            string[] input = File.ReadAllLines(path);

            bool isSave = false;
            for (int i = 0; i < input.Length; i++)
            {
                string value = input[i];
                input[i] = OpenRunLine(value);
                isSave = value == input[i];
            }
            if (isSave)
                File.WriteAllLines(path, input);
        }

        protected virtual string OpenRunLine(string value)
        {
            return value;
        }
    }
}
