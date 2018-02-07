using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Library
{
    public partial class Config
    {     
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);        
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        private static string inipath = Environment.CurrentDirectory + "\\config.ini";

        static Config()
        {
            if (File.Exists(inipath))
                return;
            File.WriteAllText(inipath, "");
        }

        /// <summary>
        /// 读取配置ini文件
        /// </summary>
        /// <param name="section">配置段</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">值</param>
        /// <returns></returns>
        public static string IniReadValue(string section, string key, string defaultValue)
        {
            StringBuilder temp = new StringBuilder(500);
            var value = GetPrivateProfileString(section, key, "", temp, 500, inipath);
            if (temp.Length != 0) return temp.ToString();
            IniWriteValue(section, key, defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// 写入ini文件的操作
        /// </summary>
        /// <param name="section">配置段</param>
        /// <param name="key">键</param>
        /// <param name="value">键值</param>
        public static void IniWriteValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, inipath);
        }
    }
}
