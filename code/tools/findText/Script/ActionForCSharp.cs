using System;
using System.Text.RegularExpressions;

namespace findText.Script
{
    public class ActionForCSharp : BaseActionFor
    {
        protected override string textName
        {
            get { return "Find_CSharp_Text"; }
        }

        protected override string exName
        {
            get { return "*.cs";}
        }

        protected override void OpenRun(string[] input)
        {
            bool isTrue = false;

            for (int k = 0; k < input.Length; k++)
            {
                if (isTrue) continue;
                var val = input[k];
                if (val.TrimStart().StartsWith("#")) continue;
                if (val.TrimStart().StartsWith("@brief ")) continue;
                if (val.TrimStart().StartsWith("///")) continue;
                if (val.TrimStart().StartsWith("//")) continue;
                //跨行注释
                if (val.TrimStart().StartsWith("/*"))
                {
                    if (!val.Contains("*/"))
                        isTrue = true;
                    continue;
                }
                if (val.TrimStart().EndsWith("*/"))
                {
                    if (!val.Contains("/*"))
                        isTrue = false;
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
                //去除中间有/**
                index = val.IndexOf("/**", StringComparison.Ordinal);
                if (index >= 0)
                {
                    val = val.Substring(0, index);
                    mc = regex.Matches(val);
                    if (mc.Count == 0) continue;
                }
                //去除最后一个双引号后的
                index = val.LastIndexOf("\"", StringComparison.Ordinal);
                if (index >= 0)
                {
                    val = val.Substring(0, index);
                }
                //去除第一个双引号前的
                index = val.IndexOf("\"", StringComparison.Ordinal);
                if (index >= 0)
                {
                    val = val.Substring(index + 1);
                }
                GetJsonValue(val, k, input);
            }
        }
    }
}