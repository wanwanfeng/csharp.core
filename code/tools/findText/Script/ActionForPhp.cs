using System;
using System.IO;
using System.Text.RegularExpressions;
using LitJson;

namespace findText.Script
{
    public class ActionForPhp : BaseActionFor
    {
        public override string regexStr
        {
            get
            {
                return
                    //"\"([\u4E00-\u9FA5]+)|([\u4E00-\u9FA5]+.*\")|(\".*[\u30A0-\u30FF]+)|([\u30A0-\u30FF])\""
                    //"([\u4E00-\u9FA5]+)|([\u4E00-\u9FA5]')|([\u30A0-\u30FF])|([\u30A0-\u30FF])"
                    //"/u0800-/u4e00"
                    //"[\u{2E80}-\u{9FFF}]+/u"
                    "[\u2E80-\u9FFF]+"
                    ;
            }
        }


        protected override string textName
        {
            get { return "Find_Php_Text"; }
        }

        protected override string[] exName
        {
            get { return new[] {".php"}; }
        }

        protected override void OpenRun()
        {
            for (int i = 0; i < all.Count; i++)
            {
                string[] input = GetShowInfo(i);

                bool isNewLine = false;

                for (int k = 0; k < input.Length; k++)
                {
                    var val = input[k];
                    if (val.TrimStart().EndsWith("*/"))
                    {
                        if (val.Contains("/*") == false)
                            isNewLine = false;
                        continue;
                    }
                    //跨行注释
                    if (val.TrimStart().StartsWith("/*"))
                    {
                        if (val.Contains("*/") == false)
                            isNewLine = true;
                        continue;
                    }
                    if (isNewLine) continue;

                    if (val.TrimStart().StartsWith("#")) continue;
                    if (val.TrimStart().StartsWith("@brief ")) continue;
                    if (val.TrimStart().StartsWith("///")) continue;
                    if (val.TrimStart().StartsWith("//")) continue;
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
                    //去除最后一个单引号后的
                    index = val.LastIndexOf("'", StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        val = val.Substring(0, index);
                    }
                    //去除第一个单引号前的
                    index = val.IndexOf("'", StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        val = val.Substring(index + 1);
                    }
                    GetJsonValue(val, i, k, input);
                }
            }
        }
    }
}