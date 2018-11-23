using System;
using System.IO;
using System.Text.RegularExpressions;

namespace findText.Script
{
    public class ActionForCss : BaseActionFor
    {
        protected override string textName
        {
            get { return "Find_Css_Text"; }
        }

        protected override string exName
        {
            get { return "*.css|*.sass|*.scss"; }
        }

        protected override void OpenRun(string file)
        {
            string[] input = File.ReadAllLines(file);

            bool isDouble = false;

            for (int k = 0; k < input.Length; k++)
            {
                if (isDouble) continue;
                var val = input[k];
                if (val.TrimStart().StartsWith("//")) continue;
                //跨行注释
                if (val.TrimStart().StartsWith("/*"))
                {
                    if (!val.Contains("*/"))
                        isDouble = true;
                    continue;
                }
                if (val.TrimEnd().EndsWith("*/"))
                {
                    if (!val.Contains("/*"))
                        isDouble = false;
                    continue;
                }
                if (val.TrimStart().StartsWith("*")) continue;
                MatchCollection mc = regex.Matches(val);
                if (mc.Count == 0) continue;
                //去除中间有//
                var index = val.IndexOf("//", StringComparison.Ordinal);
                if (index >= 0)
                {
                    val = val.Substring(0, index);
                    mc = regex.Matches(val);
                    if (mc.Count == 0) continue;
                }
                GetJsonValue(val, file, k, input);
            }
        }
    }
}