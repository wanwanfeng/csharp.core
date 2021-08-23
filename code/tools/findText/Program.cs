using findText.Script;
using Library;
using Library.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;

namespace findText
{
    internal static class Program
    {
		/// <summary>
		/// https://blog.csdn.net/Q672405097/article/details/86639488
		/// </summary>
		private static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

		static Program()
		{
			Environment.SetEnvironmentVariable("SkipSheet", SystemConsole.GetommandLineArgs("skipSheet"));
			Environment.SetEnvironmentVariable("TakeSheet", SystemConsole.GetommandLineArgs("takeSheet"));

			Environment.SetEnvironmentVariable("JsonIsIndent", 1.ToString());
			Environment.SetEnvironmentVariable("KeyLine", 1.ToString());
			Environment.SetEnvironmentVariable("SkipRows", 0.ToString());
			Environment.SetEnvironmentVariable("SkipColumns", 0.ToString());
			Environment.SetEnvironmentVariable("TakeColumns", int.MaxValue.ToString());

			foreach (var item in ConfigurationManager.AppSettings.AllKeys)
			{
				Environment.SetEnvironmentVariable(item, config.AppSettings.Settings[item].Value);
			}
		}

		public enum ConvertType
        {
            [TypeValue(typeof(ActionForUnity2))] unity,
            [TypeValue(typeof(ActionForCss))] css,
            [TypeValue(typeof(ActionForCpp))] cpp,
            [TypeValue(typeof(ActionForCSharp))] csharp,
            [TypeValue(typeof(ActionForPhp))] php,
            [TypeValue(typeof(ActionForJava))] java,

            [TypeValue(typeof(ActionForJavaScript))]
            javascript,
            [TypeValue(typeof(ActionForHtml))] html,

            [TypeValue(typeof(ActionForHtml2))]
            [Description("html 插件")]
            html2
        }

        private static void Main(string[] args)
        {
            Action<object> callFunc = obj =>
            {
                BaseActionFor baseActionFor = (BaseActionFor)obj;
                SystemConsole.Run(config: new Dictionary<string, Action>()
                {
                    {"搜索", () => { baseActionFor.Open(); }},
                    {"还原", () => { baseActionFor.Revert(); }},
                });
            };
            SystemConsole.Run<ConvertType>(callFunc);
        }
    }
}
