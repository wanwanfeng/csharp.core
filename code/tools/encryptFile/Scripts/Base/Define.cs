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
            get { return Config.IniReadValue("Path", "defineRoot", Environment.CurrentDirectory + "\\Root", 1000); }
            set { Config.IniWriteValue("Path", "defineRoot", value); }
        }

        public static string DefineSave
        {
            get { return Config.IniReadValue("Path", "defineSave", Environment.CurrentDirectory + "\\Save", 1000); }
            set { Config.IniWriteValue("Path", "defineSave", value); }
        }
    }
}
