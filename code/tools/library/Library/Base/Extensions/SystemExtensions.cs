using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Helper;

namespace Library.Extensions
{
    public abstract class BaseSystemConsole : CmdHelper
    {
        public enum SelectType
        {
            [Description("请拖入文件({0})：")] File,
            [Description("请拖入文件夹({0})：")] Folder,
            [Description("请拖入文件夹或文件({0})：")] All
        }

        public static IDictionary<SelectType, string> CacheSelect;

        static BaseSystemConsole()
        {
            CacheSelect = AttributeHelper.GetCacheDescription<SelectType>();
        }

        public static string InputPath { get; set; }

        public static string CheckExtension()
        {
            var selectExtension = SystemConsole.GetInputStr("请输入文件后缀[如\"cs,cpp\"]:");
            return (string.IsNullOrEmpty(selectExtension) || selectExtension == "*.*")
                ? "*.*"
                : string.Join("|",
                    selectExtension.Split(',', '|').Select(p => "." + p.TrimStart('*').TrimStart('.')).ToArray());
        }

        public static List<string> CheckPath(string selectExtension = "*.*", SelectType selectType = SelectType.All,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (string.IsNullOrEmpty(selectExtension))
                selectExtension = SystemConsole.GetInputStr("请输入文件后缀 ('.cs'):");

            var selectExtensions = (string.IsNullOrEmpty(selectExtension) || selectExtension == "*.*")
                ? new string[0]
                : selectExtension.Split(',', '|').Select(p => "." + p.TrimStart('*').TrimStart('.')).ToArray();

            List<string> files = new List<string>();
            string path = SystemConsole.GetInputStr(
                beforeTip: string.Format(CacheSelect[selectType], selectExtension),
                afterTip: "您选择的文件夹或文件：");
            if (string.IsNullOrEmpty(path))
                return files;

            InputPath = path.Replace("\\", "/").TrimEnd('/');

            switch (selectType)
            {
                case SelectType.File:
                    if (File.Exists(path))
                    {
                        if (selectExtensions.Contains(Path.GetExtension(path)))
                            files.Add(path);
                        if (selectExtensions.Length == 0)
                            files.Add(path);
                    }
                    break;
                case SelectType.Folder:
                    if (Directory.Exists(path))
                        files.AddRange(DirectoryHelper.GetFiles(path, selectExtensions, searchOption));
                    break;
                case SelectType.All:
                    if (Directory.Exists(path))
                        files.AddRange(DirectoryHelper.GetFiles(path, selectExtensions, searchOption));
                    else if (File.Exists(path))
                    {
                        if (selectExtensions.Contains(Path.GetExtension(path)))
                            files.Add(path);
                        if (selectExtensions.Length == 0)
                            files.Add(path);
                    }
                    break;
                default:
                    Console.WriteLine("path is not valid!");
                    return files;
            }

            files.Sort();
            return files;
        }

        protected static void WriteError(string name, IEnumerable<string> resList)
        {
            // ReSharper disable once PossibleNullReferenceException
            var newPath = Path.ChangeExtension(InputPath, "").Trim('.') + "[" + name + "].txt";
            var enumerable = resList as string[] ?? resList.ToArray();
            if (enumerable.Length == 0)
            {
                File.Delete(newPath);
                return;
            }
            File.WriteAllLines(newPath, enumerable.ToArray());
        }

        protected void WriteError(IEnumerable<string> resList)
        {
            WriteError(GetType().Name, resList);
        }


        public virtual string WriteAllLines(string path, string[] contents)
        {
            path = GetType() + "/" + path;
            DirectoryHelper.CreateDirectory(path);
            File.WriteAllLines(path, contents);
            return path;
        }

        public virtual string WriteAllText(string path, string content)
        {
            path = GetType() + "/" + path;
            DirectoryHelper.CreateDirectory(path);
            File.WriteAllText(path, content);
            return path;
        }
    }

    public static class BaseSystemConsoleExtensions
    {
        public static void ForEachPaths(this IEnumerable<string> paths, Action<string> callAction)
        {
            paths.Select(p => p.Replace("\\", "/").TrimStart('/')).ToList().ForEach((p, i, target) =>
            {
                SystemConsole.SetProgress(string.Format("is now : {0} {1}", (((float) i)/target.Count).ToString("p"), p));
                //Console.WriteLine("is now : " + (((float) i)/target.Count).ToString("p") + "\t" + p);
                if (File.Exists(p)) callAction(p);
            });
        }
    }

    public static class SystemConsole
    {
        public static void Run<T>(Action<object> callAction = null, int columnsCount = 1) where T : struct
        {
            Console.Title = typeof (T).Namespace ?? Console.Title;
            //Console.OutputEncoding = Console.InputEncoding = System.Text.Encoding.UTF8;

            var cacheCategory = AttributeHelper.GetCache<T, CategoryAttribute>()
                .GroupBy(p => p.Value == null ? "" : p.Value.Category)
                .ToDictionary(p => p.Key, p => p.ToDictionary(q => (int) q.Key, q => (T) q.Key));
            var cacheType = AttributeHelper.GetCache<T, TypeValueAttribute>()
                .ToDictionary(p => (int) p.Key);
            var cacheDesc = AttributeHelper.GetCacheDescription<T>()
                .ToDictionary(p => p.Key, q => string.IsNullOrEmpty(q.Value) ? q.Key.ToString() : q.Value);

            int maxLength = cacheDesc.Values.Max(p => p.Length);

            bool y = false;
            do
            {
                Console.Clear();

                List<string> showList = new List<string>();

                foreach (KeyValuePair<string, Dictionary<int, T>> pair in cacheCategory)
                {
                    showList.Add(pair.Key);

                    var lineNum = (int) Math.Ceiling((float) pair.Value.Count/columnsCount);
                    var cache = pair.Value
                        .GroupBy(p => p.Key%lineNum, q => q)
                        .ToDictionary(p => p.Key, q => q.ToList());

                    foreach (var value in cache.Values)
                    {
                        var res =
                            value.Select(
                                p => string.Format("{0:d2}：{1}", p.Key, cacheDesc[p.Value].PadRight(maxLength, '.')));
                        showList.Add(string.Format("\t{0}\t", string.Join("\t", res.ToArray())));
                    }
                }

                int maxLine = showList.Max(p => p.Length + columnsCount*4 + 8);
                maxLine += (maxLine%2 == 0 ? 0 : 1);
                Console.WindowWidth = maxLine < Console.WindowWidth ? Console.WindowWidth : maxLine;
                showList.Add("\n\t" + "e：exit\n");
                showList.Add("-".PadRight(maxLine, '-'));
                showList.Insert(0, "命令索引".Pad(maxLine - 4, '-') + "\n");
                showList.ForEach(Console.WriteLine);
                Console.WindowHeight = showList.Count < 25 ? 25 : showList.Count + 5;

                try
                {
                    //resetInput:
                    var index = GetInputStr("请选择，然后回车：", def: "e", regex: "^[0-9]*$").AsInt();
                    if (cacheType.ContainsKey(index))
                    {
                        Console.WriteLine("当前的选择：" + index);
                        //do
                        //{
                        var obj = Activator.CreateInstance(cacheType[index].Value.value);
                        if (callAction != null)
                            callAction.Invoke(obj);
                        //} while (ContinueY());
                    }
                    //goto resetInput;
                }
                catch (Exception e)
                {
                    e.FinalException().ForEach(p =>
                    {
                        Console.WriteLine(p.Message);
                        Console.WriteLine(p.StackTrace);
                    });
                }

                GC.Collect();
            } while (ContinueY());
        }

        public static void Run(Dictionary<string, Action> config)
        {
            Console.WriteLine("-------操作列表-------");
            config.Keys.ToList().ForEach((p, i) => Console.WriteLine((i + 1) + "：" + p));
            Console.WriteLine("----------------------");
            List<Action> result = config.Values.ToList();
            config = result.ToDictionary(p => (result.IndexOf(p) + 1).ToString());
            config = new Dictionary<string, Action>
            {
                {"e", () => { QuitReadKey(); }}
            }.Merge(config);

            string cmd = "e";
            do
            {
                cmd = GetInputStr("输入指令：");
                if (config.ContainsKey(cmd))
                    config[cmd].Invoke();
            } while (!config.ContainsKey(cmd));
        }

        /// <summary>
        /// 用于控制台程序
        /// </summary>
        /// <returns></returns>
        public static string GetInputStr(
            string beforeTip = "请输入:",
            string afterTip = "",
            string def = "e",
            string regex = null)
        {
            Console.Write(beforeTip);
            var cmd = Console.ReadLine();
            var str = string.IsNullOrEmpty(cmd) ? def : cmd;
            str = str.Contains(" ") ? str.Substring(1, str.Length - 2) : str;

            Console.WriteLine(afterTip + str);

            if (regex == null || Regex.IsMatch(str, "e|" + regex))
            {
                var config = new Dictionary<string, Action>
                {
                    {"e", () => { QuitReadKey(); }}
                }.Merge(null);
                if (config.ContainsKey(str))
                    config[str].Invoke();
                return str;
            }
            return GetInputStr(beforeTip, afterTip, def, regex);
        }

        /// <summary>
        /// 用于控制台是否继续，"按'y'键继续，按其余键退出， 
        /// </summary>
        /// <param name="beforeTip"> "press 'y' to continue : "; //"按'y'键继续！"</param>
        /// <returns></returns>
        public static bool ContinueY(string beforeTip = "按'y'键继续（默认‘y’）： ")
        {
            try
            {
                Console.Write(Environment.NewLine);
                return GetInputStr(beforeTip, def: "y") == "y";
            }
            finally
            {
                Console.Write(Environment.NewLine);
            }
        }

        /// <summary>
        /// 用于控制台是否继续
        /// </summary>
        /// <param name="beforeTip"> </param>
        /// <returns></returns>
        public static void ContinueReadKey(string beforeTip = "按任意键继续！")
        {
            Console.WriteLine(beforeTip);
            Console.ReadKey();
        }


        /// <summary>
        /// 用于控制台是否退出
        /// </summary>
        /// <param name="beforeTip">"press any key to exit;"; //"按任意键退出！"</param>
        public static void QuitReadKey(string beforeTip = "按任意键退出！")
        {
            Console.WriteLine(beforeTip);
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static int _lastOffset = 0;
        public static void SetProgress(string info = "", float progress = 0.0f)
        {
            var msg = string.Format("[{0}] {1} {2}", "*".PadRight((int) Math.Floor(15*progress), '*').PadRight(15, '-'), progress.ToString("p2"), info);
            Console.WriteLine(msg);
            _lastOffset = msg.Length / Console.WindowWidth + 1;
            Console.SetCursorPosition(0, Math.Max(0, Console.CursorTop - _lastOffset));
        }

        public static void ClearProgress()
        {
            Console.SetCursorPosition(0, Math.Max(0, Console.CursorTop + _lastOffset));
        }
    }
}