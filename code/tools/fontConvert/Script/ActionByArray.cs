using System;
using System.IO;
using System.Text;

namespace fontConvert.Script
{
    public class ActionByArray : BaseActionBy
    {
        protected class Base
        {
            private static string _sourceArray;
            private static string _targetArray;

            static Base()
            {
                _sourceArray = File.ReadAllText("Template/CharJianTi.txt");
                _targetArray = File.ReadAllText("Template/CharFanTi.txt");
            }

            public static string ToSimplified(string source, ref bool isSave)
            {
                return ConvertTo(source, _targetArray, _sourceArray, ref isSave);
            }

            public static string ToTraditional(string source, ref bool isSave)
            {
                return ConvertTo(source, _sourceArray, _targetArray, ref isSave);
            }

            private static string ConvertTo(string text, string source, string target, ref bool isSave)
            {
                char[] content = text.ToCharArray();
                StringBuilder str = new StringBuilder(content.Length);
                for (var i = 0; i < content.Length; i++)
                {
                    char value = content[i];
                    var index = source.IndexOf(value);
                    if (index >= 0)
                    {
                        if (value == target[index])
                        {
                            str.Append(value);
                        }
                        else
                        {
                            str.Append(target[index]);
                            isSave = true;
                        }
                    }
                    else
                    {
                        str.Append(value);
                    }
                }
                return str.ToString();
            }
        }

        public class ToTraditional : ActionByArray
        {
            protected override void OpenRun()
            {
                for (int i = 0; i < all.Count; i++)
                {
                    Console.WriteLine("替换中...请稍后" + ((float)i / all.Count).ToString("f") + "\t" + all[i]);
                    string content = File.ReadAllText(all[i]);

                    bool isSave = false;

                    string result = Base.ToTraditional(content, ref isSave);

                    if (isSave)
                    {
                        File.WriteAllText(all[i], result, Encoding.UTF8);
                    }
                }
            }
        }

        public class ToSimplified : ActionByArray
        {
            protected override void OpenRun()
            {
                for (int i = 0; i < all.Count; i++)
                {
                    Console.WriteLine("替换中...请稍后" + ((float)i / all.Count).ToString("f") + "\t" + all[i]);
                    string content = File.ReadAllText(all[i]);

                    bool isSave = false;

                    string result = Base.ToSimplified(content, ref isSave);

                    if (isSave)
                    {
                        File.WriteAllText(all[i], result, Encoding.UTF8);
                    }
                }
            }
        }

    }
}