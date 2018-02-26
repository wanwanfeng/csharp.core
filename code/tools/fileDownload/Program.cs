using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using excel;
using LitJson;

namespace fileDownload
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string url = "https://tkpres.global.ssl.fastly.net/assets/app/";
            // file_name	hash_name	dl_point	file_type	bind_priority	hash_value	encrypted_hash_value	revision	is_deleted	created_at	updated_at

            Dictionary<string, JsonData> cacheMaster = CacheJson("c_master").ToDictionary(p => "master/" + p.Key, q => q.Value);

            Dictionary<string, JsonData> cacheRresource = CacheJson("c_resource");//.Where(p => p.Key.Contains("unity")).ToDictionary(p => p.Key, q => q.Value);

            Dictionary<string, JsonData> cache = new Dictionary<string, JsonData>();

            foreach (KeyValuePair<string, JsonData> pair in cacheMaster)
                cache[pair.Key] = pair.Value;
            foreach (KeyValuePair<string, JsonData> pair in cacheRresource)
                cache[pair.Key] = pair.Value;


            return;

            if (File.Exists("overList.txt"))
                File.Delete("overList.txt");
            if (File.Exists("errList.txt"))
                File.Delete("errList.txt");

            int index = 0;
            foreach (KeyValuePair<string, JsonData> pair in cache)
            {
                var filename = pair.Key;
                var hashname = pair.Value["hash_name"].ToString();
                var hashvalue = pair.Value["hash_value"].ToString();
                var encryptedhashvalue = pair.Value["encrypted_hash_value"].ToString();
                var revision = pair.Value["revision"].ToString();
                Console.WriteLine("is Now : {0}/{1}     {2}", (++index), cache.Count, pair.Key);

                if (HttpDownload(url, revision, hashname.Trim(), filename, hashvalue, encryptedhashvalue))
                {
                    WriteLog("overList", filename);
                }
                else
                {
                    WriteLog("errList", filename);
                }
            }
            Console.WriteLine("下载完毕");
            Console.ReadKey();
        }

        private static void WriteLog(string name, string content)
        {
            using (StreamWriter streamWriter = File.AppendText(name + ".txt"))
            {
                streamWriter.WriteLine(content);
                streamWriter.Close();
            }
        }

        private static Dictionary<string, JsonData> CacheJson(string jsonName)
        {
            Dictionary<string, JsonData> dic = new Dictionary<string, JsonData>();

            var path = Environment.CurrentDirectory + "\\" + jsonName + ".json";
            if (File.Exists(path))
            {
                var json = LitJson.JsonMapper.ToObject(File.ReadAllText(path).Trim().Trim('\0'));
                //Console.WriteLine(json);
                dic = json.Inst_Object.ToDictionary(p => p.Key, q => q.Value);
            }
            else
            {
                dic = GetCacheValueFromExcel(jsonName);

                JsonData jsonData = new JsonData();
                jsonData.SetJsonType(JsonType.Array);

                foreach (KeyValuePair<string, JsonData> keyValuePair in dic)
                {
                    JsonData data = new JsonData();
                    data[keyValuePair.Key] = keyValuePair.Value;
                    jsonData.Add(data);
                }

                File.WriteAllText(path, LitJson.JsonMapper.ToJson(jsonData));
            }
            return dic;
        }

        private static Dictionary<string, JsonData> GetCacheValueFromExcel(string path)
        {
            Dictionary<string, JsonData> cache = new Dictionary<string, JsonData>();
            Dictionary<string, List<List<object>>> dic = OfficeWorkbooks.ReadFromExcel(Environment.CurrentDirectory + "/" + path + ".xlsx");
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

        public static bool HttpDownload(string url, string revision, string hashname, string filename,string  hashvalue, string encryptedhashvalue)
        {
            string tempPath = Environment.CurrentDirectory + @"\temp\" + Path.GetDirectoryName(filename);
            Directory.CreateDirectory(tempPath); //创建临时文件目录
            var newname = tempPath + @"\" + Path.GetFileName(filename);

            string tempFile = newname + ".temp"; //临时文件
            if (File.Exists(tempFile))
                File.Delete(tempFile); //存在则删除

            if (File.Exists(newname))
            {
                //var hash = UtilSecurity.GetMD5Value(File.ReadAllText(newname));
                //if (hashvalue == hash)
                return true;
            }

            FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            try
            {
                // 设置参数

                url = url + revision + "/" + hashname;// + "?t=" + DateTime.Now.Ticks;
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                Console.WriteLine(url);

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

                UnMakeDecryptor(newname);

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