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

            for (int k = 0; k < input.Length; k++)
            {
                var val = input[k];
                MatchCollection mc = regex.Matches(val);
                if (mc.Count == 0) continue;
                GetJsonValue(val, file, k, input);
            }
        }
    }
}