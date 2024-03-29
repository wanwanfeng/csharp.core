﻿using System;
using System.IO;
using System.Text.RegularExpressions;

namespace findText.Script
{
    public class ActionForCpp : BaseActionFor
    {
        protected override string textName
        {
            get { return "Find_Cpp_Text"; }
        }

        protected override string exName
        {
            get { return "*.cpp|*.h"; }
        }

        protected override void OpenRun(string file)
        {
            string[] input = File.ReadAllLines(file);

            bool isDouble = false;

            for (int k = 0; k < input.Length; k++)
            {
                if (isDouble) continue;
                var val = input[k];
                //if (val.TrimStart().StartsWith("#") && !val.TrimStart().StartsWith("#define")) continue;
                //if (val.TrimStart().StartsWith("@brief ")) continue;
                //if (val.TrimStart().StartsWith("///")) continue;
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

				if (CheckMatches(val)) continue;

				MatchCollection mc = null;

				//去除中间有//
				var index = val.IndexOf("//", StringComparison.Ordinal);
                if (index >= 0)
                {
                    val = val.Substring(0, index);
                    mc = regex.Matches(val);
                    if (mc.Count == 0) continue;
                }
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