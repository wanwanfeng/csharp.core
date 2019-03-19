using System.IO;
using System.Text.RegularExpressions;

namespace findText.Script
{
    public class ActionForHtml : BaseActionFor
    {
        protected override string textName
        {
            get { return "Find_Html_Text"; }
        }

        protected override string exName
        {
            get { return "*.html|*.htm|*.tpl|*.plist"; }
        }

        protected override void OpenRun(string file)
        {
            string[] input = File.ReadAllLines(file);

            bool isTrue = false;

            for (int k = 0; k < input.Length; k++)
            {
                if (isTrue) continue;
                var val = input[k];
                //if (val.TrimStart().StartsWith("//")) continue;
                ////跨行注释
                //if (val.TrimStart().Contains("<!--"))
                //{
                //    if (!val.Contains("-->"))
                //        isTrue = true;
                //    continue;
                //}
                //if (val.TrimStart().Contains("-->"))
                //{
                //    if (!val.Contains("<!--"))
                //        isTrue = false;
                //    continue;
                //}

                MatchCollection mc = regex.Matches(val);
                if (mc.Count == 0) continue;
                ////去除中间有//
                //var index = val.IndexOf("//", StringComparison.Ordinal);
                //if (index >= 0)
                //{
                //    val = val.Substring(0, index);
                //    mc = regex.Matches(val);
                //    if (mc.Count == 0) continue;
                //}
                ////去除中间有/**
                //index = val.IndexOf("/**", StringComparison.Ordinal);
                //if (index >= 0)
                //{
                //    val = val.Substring(0, index);
                //    mc = regex.Matches(val);
                //    if (mc.Count == 0) continue;
                //}
                ////去除最后一个双引号后的
                //index = val.LastIndexOf("\"", StringComparison.Ordinal);
                //if (index >= 0)
                //{
                //    val = val.Substring(0, index);
                //}
                ////去除第一个双引号前的
                //index = val.IndexOf("\"", StringComparison.Ordinal);
                //if (index >= 0)
                //{
                //    val = val.Substring(index + 1);
                //}
                GetJsonValue(val, file, k, input);
            }
        }
    }
}