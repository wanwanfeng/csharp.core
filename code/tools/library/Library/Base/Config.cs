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

        private static string Inipath { get; set; }

        static Config()
        {
            Init(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
        }

        /// <summary>
        /// 初始化文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defaultName"></param>
        public static void Init(string path, string defaultName = "config.ini")
        {
            if (Path.HasExtension(path))
            {
                Inipath = path;
            }
            else
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                Inipath = Path.Combine(path, defaultName);
            }
            if (!File.Exists(Inipath))
                File.WriteAllText(Inipath, "");
        }

        /// <summary>
        /// 读取配置ini文件
        /// </summary>
        /// <param name="section">配置段</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">值</param>
        /// <param name="buffSize">缓冲区大小</param>
        /// <returns></returns>
        public static string IniReadValue(string section, string key, string defaultValue = "",int buffSize = 500)
        {
            StringBuilder temp = new StringBuilder(buffSize);
            var value = GetPrivateProfileString(section, key, "", temp, buffSize, Inipath);
            if (value != 0) return temp.ToString();
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
            WritePrivateProfileString(section, key, value, Inipath);
        }
    }
}
