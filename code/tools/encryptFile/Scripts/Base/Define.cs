using System;
using System.IO;
using Library;

namespace Encrypt
{
    internal class Define
    {
        public static string DefineKey
        {
            get { return Config.IniReadValue("Config", "md5key").Trim(); }
            set { Config.IniWriteValue("Config", "md5key", value.Trim()); }
        }

        public static string[] DefineExclude
        {
            get { return Config.IniReadValue("Path", "exclude").Split(','); }
            set { Config.IniWriteValue("Path", "exclude", string.Join(",", value)); }
        }

        public static string DefineRoot
        {
            get
            {
                var path = Config.IniReadValue("Path", "defineRoot");
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                    DefineRoot = path = Environment.CurrentDirectory + "\\Root";
                return path;
            }
            set { Config.IniWriteValue("Path", "defineRoot", value); }
        }

        public static string DefineSave
        {
            get
            {
                var path = Config.IniReadValue("Path", "defineSave");
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                    DefineSave = path = Environment.CurrentDirectory + "\\Save";
                return path;
            }
            set { Config.IniWriteValue("Path", "defineSave", value); }
        }
    }
}
