using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Library.Excel;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace fileDownload
{
    internal class Program
    {
        //public static string root = Environment.CurrentDirectory + "/www/assets/";
        public static string root = "D:/Work/yuege/www/assets/";

        /// <summary>
        /// file_name	
        /// hash_name	
        /// dl_point	
        /// file_type	
        /// bind_priority	
        /// hash_value	
        /// encrypted_hash_value	
        /// revision	
        /// is_deleted	
        /// created_at	
        /// updated_at
        /// </summary>
        public static string url
        {
            get
            {
                return "http://10.23.25.8:9999/assets/app-bs/";
                return "https://tkpres.global.ssl.fastly.net/assets/app/";
            }
        }

        public static string Cmd1 = "e", FromExcel = "e";
        public static int index = 0, count = 0;

        private static void Main(string[] args)
        {
            Cmd1 = SystemConsole.GetInputStr("1:down\n2:md5还原与解密\nInput:");
            FromExcel = SystemConsole.GetInputStr("1:来自excel\n2:来自json\nInput:");

            root = SystemConsole.GetInputStr("输入文件目录:");
            Dictionary<string, JsonData> cacheMaster = CacheJson("c_master").ToDictionary(p => "master/" + p.Key, q => q.Value);
            Dictionary<string, JsonData> cacheRresource = CacheJson("c_resource");//.Where(p => p.Key.Contains("unity")).ToDictionary(p => p.Key, q => q.Value);

            FileHelper.CreateDirectory("log/");
            if (File.Exists("log/overList.txt"))
                File.Delete("log/overList.txt");
            if (File.Exists("log/errList.txt"))
                File.Delete("log/errList.txt");

            cacheRresource.Concat(cacheMaster)
                .ToDictionary(p => p.Key, q => q.Value)
                .AsParallel()
                .WithDegreeOfParallelism(4)
                .ForAll(RunSingle);

            GC.Collect();
            Console.ReadKey();
        }

        private static void RunSingle(KeyValuePair<string, JsonData> pair)
        {
            var filename = pair.Key;
            var hashname = pair.Value["hash_name"].ToString();
            var hashvalue = pair.Value["hash_value"].ToString();
            var encryptedhashvalue = pair.Value["encrypted_hash_value"].ToString();
            var revision = pair.Value["revision"].ToString();

            Console.WriteLine("is Now :{2} \n{0}/{1}", (++index), count, pair.Key);

            switch (Cmd1)
            {
                case "1":
                {
                    WriteLog(
                        HttpDownload(revision, hashname.Trim(), filename, hashvalue, encryptedhashvalue)
                            ? "log/overList"
                            : "log/errList", filename);
                    Console.WriteLine("下载完毕");
                    break;
                }
                case "2":
                {
                    WriteLog(
                        Move(revision, hashname.Trim(), filename, hashvalue, encryptedhashvalue)
                            ? "log/overList"
                            : "log/errList", filename);
                    Console.WriteLine("还原完毕");
                    break;
                }
            }
        }

        private static void WriteLog(string name, string content)
        {
            return;
            //lock (thisLock)
            //{
            //    using (StreamWriter streamWriter = File.AppendText(name + ".txt"))
            //    {
            //        streamWriter.WriteLine(content);
            //        streamWriter.Close();
            //    }
            //}
        }

        private static Dictionary<string, JsonData> CacheJson(string jsonName)
        {
            Dictionary<string, JsonData> dic = new Dictionary<string, JsonData>();
            switch (FromExcel)
            {
                case "1":
                {
                    var path = string.Format("{0}/{1}.xlsx", root, jsonName);
                    var tables = ExcelUtils.ImportFromPath(path, false).Select(p => (JsonData) p).ToList();
                    dic = tables.First().Cast<JsonData>().ToDictionary(p => p["file_name"].ToString(), q => q);
                    var newPath = root + "res/master/" + jsonName + ".json";
                    File.WriteAllText(newPath, JsonHelper.ToJson(dic));
                }
                    break;
                case "2":
                {
                    var path = string.Format("{0}/{1}.json", root, jsonName);
                    var json = JsonMapper.ToObject(File.ReadAllText(path).Trim().Trim('\0'));
                    //dic = json.Inst_Object.ToDictionary(p => p.Key, q => q.Value);
                    foreach (var key in json.Keys)
                        dic[key] = json[key];
                }
                    break;
            }
            return dic;
        }

        public static bool HttpDownload(string revision, string hashname, string filename, string hashvalue, string encryptedhashvalue)
        {
            var houzhui = revision + "/" + hashname;
            string newname = root + "/app/" + houzhui;

            if (File.Exists(newname))
            {
                //var hash = UtilSecurity.GetMD5Value(File.ReadAllText(newname));
                //if (hashvalue == hash)
                    return true;
            }

            //return false;

            try
            {
                Console.WriteLine(url + houzhui);
                HttpWebRequest request = WebRequest.Create(url + houzhui) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                string tempFile = Path.GetTempFileName();
                using (var fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    //直到request.GetResponse()程序才开始向目标网页发送Post请求
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null) responseStream.CopyTo(fs);
                    }
                }
                File.Move(tempFile, newname);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static List<string> extensionList = new List<string>()
        {
            ".acb"
        };

        public static bool Move(string revision, string hashname, string filename, string hashvalue,
            string encryptedhashvalue)
        {
            var houzhui = revision + "/" + hashname;
            var source = (root + "app/" + houzhui).Replace("/", "\\");
            string newname = (root + "res/" + filename).Replace("/", "\\");
            DirectoryHelper.CreateDirectory(newname);
            try
            {
                if (File.Exists(source))
                {
                    if (File.Exists(newname))
                    {
                        //var hash = UtilSecurity.GetMD5Value(File.ReadAllText(newname));
                        //if (hashvalue == hash)
                        return true;
                    }
                    else
                    {
                        //return false;
                        File.Copy(source, newname);
                        if (!extensionList.Contains(Path.GetExtension(newname)))
                            UnMakeDecryptor(newname);
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("未下载");
                    return false;                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static void UnMakeDecryptor(string path)
        {
            var bytes = File.ReadAllBytes(path);
            try
            {
                bytes = UtilSecurity.DecryptionBytes(bytes, UtilSecurity.MakeDecryptorTransform(Path.GetFileName(path)));
                FileHelper.CreateDirectory(path);
                File.WriteAllBytes(path, bytes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void MakeDecryptor(string path)
        {
            var bytes = File.ReadAllBytes(path);
            try
            {
                bytes = UtilSecurity.EncryptionBytes(bytes, UtilSecurity.MakeEncryptorTransform(Path.GetFileName(path)));
                FileHelper.CreateDirectory(path);
                File.WriteAllBytes(path, bytes);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}