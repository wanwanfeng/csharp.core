using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace checkcard.Scripts
{
    public class CheckJson : BaseSystemConsole
    {
        public CheckJson()
        {
            var res = new List<string>();
            Action<string> action = file =>
            {
                Console.WriteLine(" is now : " + file);
                try
                {
                    JsonData json = JsonHelper.ToObject(File.ReadAllText(file).Trim('\0').Trim());
                    JsonHelper.ToJson(json, false);
                }
                catch (Exception)
                {
                    res.Add(file);
                }
            };
            //Parallel.ForEach(CheckPath(".json", SelectType.Folder), action);//并行操作
            CheckPath(".json", SelectType.Folder).AsParallel().ForAll(action); //并行操作
            //CheckPath(".json", SelectType.Folder).ForEach(action);//线性操作

            WriteError(res.Select(p => p.Replace("/", "\\")));
        }
    }

    public class IndentJson : BaseSystemConsole
    {
        public IndentJson()
        {
            var res = new List<string>();
            Action<string> action = file =>
            {
                Console.WriteLine(" is now : " + file);
                try
                {
                    JsonData json = JsonHelper.ToObject(File.ReadAllText(file).Trim('\0').Trim());
                    string jsonStr = JsonHelper.ToJson(json, indentLevel: 2);
                    File.WriteAllText(file, jsonStr, new UTF8Encoding(false));
                }
                catch (Exception)
                {
                    res.Add(file);
                }
            };
            //Parallel.ForEach(CheckPath(".json", SelectType.Folder), action);//并行操作
            CheckPath(".json", SelectType.Folder).AsParallel().ForAll(action); //并行操作
            //CheckPath(".json", SelectType.Folder).ForEach(action);//线性操作

            WriteError(res.Select(p => p.Replace("/", "\\")));
        }
    }

    public class CancelIndentJson : BaseSystemConsole
    {
        public CancelIndentJson()
        {
            var res = new List<string>();
            Action<string> action = file =>
            {
                Console.WriteLine(" is now : " + file);
                try
                {
                    JsonData json = JsonHelper.ToObject(File.ReadAllText(file).Trim('\0').Trim());
                    string jsonStr = JsonHelper.ToJson(json, isUnicode: false);
                    File.WriteAllText(file, jsonStr, new UTF8Encoding(false));
                }
                catch (Exception)
                {
                    res.Add(file);
                }
            };
            //Parallel.ForEach(CheckPath(".json", SelectType.Folder), action);//并行操作
            CheckPath(".json", SelectType.Folder).AsParallel().ForAll(action); //并行操作
            //CheckPath(".json", SelectType.Folder).ForEach(action);//线性操作

            WriteError(res.Select(p => p.Replace("/", "\\")));
        }
    }

    public class CopyFile : BaseSystemConsole
    {
        public CopyFile()
        {
            var res = new List<string>();
            Action<string> action = file =>
            {
                Console.WriteLine(" is now : " + file);
                try
                {
                    string oldFolder = (Path.GetDirectoryName(InputPath) + "/").Replace("\\", "/"); ;
                    string newFolder = (Path.GetDirectoryName(InputPath).TrimEnd('\\') + "_new/").Replace("\\", "/");
                    File.ReadAllLines(file).ForEach(p =>
                    {
                        var oldpath = oldFolder + p.TrimStart('/');
                        var newpath = newFolder + p.TrimStart('/');
                        DirectoryHelper.CreateDirectory(newpath);
                        if (File.Exists(oldpath))
                            File.Copy(oldpath, newpath, true);
                    });
                }
                catch (Exception)
                {
                    res.Add(file);
                }
            };
            //Parallel.ForEach(CheckPath(".json", SelectType.Folder), action);//并行操作
            CheckPath(".txt", SelectType.File).AsParallel().ForAll(action); //并行操作
            //CheckPath(".json", SelectType.Folder).ForEach(action);//线性操作

            WriteError(res.Select(p => p.Replace("/", "\\")));
        }
    }
}