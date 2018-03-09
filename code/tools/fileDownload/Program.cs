using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using excel;
using Library.Helper;
using LitJson;

namespace fileDownload
{
    internal class Program
    {
        //public static string root = Environment.CurrentDirectory + "/www/assets/";
        public static string root = "D:/Work/yuege/www/assets/";

        /// <summary>
        ///  file_name	hash_name	dl_point	file_type	bind_priority	hash_value	encrypted_hash_value	revision	is_deleted	created_at	updated_at
        /// </summary>
        public static string url
        {
            get { return "https://tkpres.global.ssl.fastly.net/assets/app/"; }
        }

        public enum  SelectId
        {
            Down, Move
        }

        public static SelectId selectId = SelectId.Down;
        public static bool isFromExcel = true;
        public static int index = 0;
        public static int count = 0;

        private static void Main(string[] args)
        {
            Console.WriteLine("1,来自excel [输入1]\n2,来自json[输入2]");
            var haha = Console.ReadLine();
            isFromExcel = haha == "1";
            Console.WriteLine("1,down [输入1]\n2,md5还原与解密 [输入2]");
            haha = Console.ReadLine();
            selectId = haha == "1" ? SelectId.Down : SelectId.Move;

            Dictionary<string, JsonData> cacheMaster = CacheJson("c_master").ToDictionary(p => "master/" + p.Key, q => q.Value);

            Dictionary<string, JsonData> cacheRresource = CacheJson("c_resource");//.Where(p => p.Key.Contains("unity")).ToDictionary(p => p.Key, q => q.Value);

            Dictionary<string, JsonData> cache = new Dictionary<string, JsonData>();

            foreach (KeyValuePair<string, JsonData> pair in cacheMaster)
                cache[pair.Key] = pair.Value;
            foreach (KeyValuePair<string, JsonData> pair in cacheRresource)
                cache[pair.Key] = pair.Value;

            FileHelper.CreateDirectory("log/");
            if (File.Exists("log/overList.txt"))
                File.Delete("log/overList.txt");
            if (File.Exists("log/errList.txt"))
                File.Delete("log/errList.txt");
            return;
            count = cache.Count;
            int max = 25;
            ThreadPool.SetMaxThreads(max, max);
            foreach (KeyValuePair<string, JsonData> pair in cache)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(RunSingle), new object[] {pair});
            }

            GC.Collect();
            Console.ReadKey();
        }

        private static void RunSingle(object obj)
        {
            object[] objs = (object[])obj;
            KeyValuePair<string, JsonData> pair = (KeyValuePair<string, JsonData>)objs[0];
            var filename = pair.Key;
            var hashname = pair.Value["hash_name"].ToString();
            var hashvalue = pair.Value["hash_value"].ToString();
            var encryptedhashvalue = pair.Value["encrypted_hash_value"].ToString();
            var revision = pair.Value["revision"].ToString();

            Console.WriteLine("is Now :{2} \n{0}/{1}", 0, count, pair.Key);
            //Console.WriteLine("is Now :{2} \n{0}/{1}", (++index), count, pair.Key);

            if (selectId == SelectId.Down)
            {
                if (HttpDownload(revision, hashname.Trim(), filename, hashvalue, encryptedhashvalue))
                {
                    WriteLog("log/overList", filename);
                }
                else
                {
                    WriteLog("log/errList", filename);
                }
                Console.WriteLine("下载完毕");
            }
            else if (selectId == SelectId.Move)
            {
                if (Move(revision, hashname.Trim(), filename, hashvalue, encryptedhashvalue))
                {
                    WriteLog("log/overList", filename);
                }
                else
                {
                    WriteLog("log/errList", filename);
                }
                Console.WriteLine("还原完毕");
            }
        }

        private static void WriteLog(string name, string content)
        {
            return;
            using (StreamWriter streamWriter = File.AppendText(name + ".txt"))
            {
                streamWriter.WriteLine(content);
                streamWriter.Close();
            }
        }

        private static Dictionary<string, JsonData> CacheJson(string jsonName)
        {
            Dictionary<string, JsonData> dic = new Dictionary<string, JsonData>();
            if (isFromExcel)
            {
                dic = GetCacheValueFromExcel(jsonName);
                JsonData jsonData = new JsonData();
                foreach (KeyValuePair<string, JsonData> keyValuePair in dic)
                    jsonData[keyValuePair.Key] = keyValuePair.Value;
                var path = root + "res/master/" + jsonName + ".json";
                File.WriteAllText(path, JsonMapper.ToJson(jsonData));
            }
            else
            {
                var path = Environment.CurrentDirectory + "/json/" + jsonName + ".json";
                var json = JsonMapper.ToObject(File.ReadAllText(path).Trim().Trim('\0'));
                dic = json.Inst_Object.ToDictionary(p => p.Key, q => q.Value);
            }
            return dic;
        }

        private static Dictionary<string, JsonData> GetCacheValueFromExcel(string path)
        {
            Dictionary<string, JsonData> cache = new Dictionary<string, JsonData>();
            Dictionary<string, List<List<object>>> dic = OfficeWorkbooks.ReadFromExcel(Environment.CurrentDirectory + "/excel/" + path + ".xlsx");
            foreach (KeyValuePair<string, List<List<object>>> pair in dic)
            {
                JsonData jsonData = SetJsonDataArray(pair.Value);

                foreach (JsonData data in jsonData)
                {
                    if (data["file_name"] != null)
                        cache[data["file_name"].ToString()] = data;
                }
            }
            return cache;
        }

        private static JsonData SetJsonDataArray(List<List<object>> content)
        {
            JsonData jsonDatas = new JsonData();
            jsonDatas.SetJsonType(JsonType.Array);
            List<object> first = content.First();
            content.Remove(first);
            foreach (List<object> objects in content)
            {
                JsonData jsonData = new JsonData();
                for (int j = 0; j < first.Count; j++)
                {
                    string val = objects[j].ToString();
                    jsonData[first[j].ToString()] = val;
                }
                jsonDatas.Add(jsonData);
            }
            return jsonDatas;
        }

        public static bool HttpDownload(string revision, string hashname, string filename, string hashvalue, string encryptedhashvalue)
        {
            var houzhui = revision + "/" + hashname;
            string newname = root + "app2/" + houzhui;
            FileHelper.CreateDirectory(newname);

            string tempFile = newname + ".temp"; //临时文件
            if (File.Exists(tempFile))
                File.Delete(tempFile); //存在则删除

            if (File.Exists(newname))
            {
                //var hash = UtilSecurity.GetMD5Value(File.ReadAllText(newname));
                //if (hashvalue == hash)
                return true;
            }


            //return false;

            FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            try
            {
                // 设置参数

                HttpWebRequest request = WebRequest.Create(url + houzhui) as HttpWebRequest;

                //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)";
                //request.Method = "GET";

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                //Stream stream = new FileStream(tempFile, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int) bArr.Length);
                while (size > 0)
                {
                    //stream.Write(bArr, 0, size);
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int) bArr.Length);
                }
                //stream.Close();
                fs.Close();
                responseStream.Close();

                File.Move(tempFile, newname);
                return true;
            }
            catch (Exception ex)
            {
                fs.Close();
                if (File.Exists(tempFile))
                    File.Delete(tempFile); //存在则删除
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool Move(string revision, string hashname, string filename, string hashvalue,
            string encryptedhashvalue)
        {
            var houzhui = revision + "/" + hashname;
            var source = (root + "app/" + houzhui).Replace("/", "\\");
            string newname = (root + "res/" + filename).Replace("/", "\\");
            FileHelper.CreateDirectory(newname);
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
                        if (!newname.Contains("cri/"))
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
                Util.MakeDirectory(path);
                File.WriteAllBytes(path, bytes);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}