using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace Encrypt
{
    static class Program
    {
		/// <summary>
		/// https://blog.csdn.net/Q672405097/article/details/86639488
		/// </summary>
		private static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

		public static void SetEnvironmentVariable(string key, string value)
		{
			Environment.SetEnvironmentVariable(key, value);
			config.AppSettings.Settings[key].Value = value;
			config.Save(ConfigurationSaveMode.Modified);
		}

		public static string GetEnvironmentVariable(string key)
		{
			return Environment.GetEnvironmentVariable(key);
		}

		static Program()
		{
			Environment.SetEnvironmentVariable("defineRoot", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Root"));
			Environment.SetEnvironmentVariable("defineSave", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Save"));
			Environment.SetEnvironmentVariable("md5key", "");
			Environment.SetEnvironmentVariable("exclude", "");

			foreach (var item in ConfigurationManager.AppSettings.AllKeys)
			{
				Environment.SetEnvironmentVariable(item, config.AppSettings.Settings[item].Value);
			}
		}

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EncryptFile());
        }
    }

	internal class Define
	{
		private static void SetEnvironmentVariable(string key, string value) => Program.SetEnvironmentVariable(key, value);
		private static string GetEnvironmentVariable(string key) => Program.GetEnvironmentVariable(key);

		public static string DefineKey
		{
			get => GetEnvironmentVariable("md5key");
			set => SetEnvironmentVariable("md5key", value);
		}

		public static string DefineExclude
		{
			get => GetEnvironmentVariable("exclude");
			set => SetEnvironmentVariable("exclude", value);
		}

		public static string DefineRoot
		{
			get => GetEnvironmentVariable("defineRoot");
			set => SetEnvironmentVariable("defineRoot", value);
		}

		public static string DefineSave
		{
			get => GetEnvironmentVariable("defineSave");
			set => SetEnvironmentVariable("defineSave", value);
		}
	}
}

