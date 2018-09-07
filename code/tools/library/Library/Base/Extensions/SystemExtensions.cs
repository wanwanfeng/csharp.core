using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Helper;

namespace Library.Extensions
{
    public class BaseSystemConsole
    {
        public enum SelectType
        {
            [Description("请拖入文件：")] File,
            [Description("请拖入文件夹：")] Folder,
            [Description("请拖入文件夹或文件：")] All
        }

        public static IDictionary<SelectType, string> CacheSelect;

        static BaseSystemConsole()
        {
            CacheSelect = AttributeHelper.GetCacheDescription<SelectType>();
        }

        public static string InputPath { get; set; }

        public static List<string> CheckPath(string exce, SelectType selectType = SelectType.All)
        {
            List<string> files = new List<string>();

            string path = SystemConsole.GetInputStr(CacheSelect[selectType], "您选择的文件夹或文件：");
            if (string.IsNullOrEmpty(path))
                return files;

            switch (selectType)
            {
                case SelectType.File:
                    if (File.Exists(path))
                    {
                        files.Add(path);
                    }
                    break;
                case SelectType.Folder:
                    if (Directory.Exists(path))
                    {
                        files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();
                    }
                    break;
                case SelectType.All:
                    if (Directory.Exists(path))
                    {
                        files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();
                    }
                    else if (File.Exists(path))
                    {
                        files.Add(path);
                    }
                    break;
                default:
                    Console.WriteLine("path is not valid!");
                    return files;
            }

            InputPath = path;
            var exs = exce.AsStringArray(',', '|').Select(p => p.StartsWith(".") ? p : "." + p).ToList();
            files = files.Where(p => exs.Contains(Path.GetExtension(p))).ToList();
            files.Sort();
            return files;
        }
    }

    public class SystemConsole
    {

        private static string Input { get; set; }

        public static void Run<T>(Action<object> callAction = null, int columnsCount = 1) where T : struct
        {
            Console.Title = typeof (T).Namespace ?? Console.Title;
            //Console.OutputEncoding = Console.InputEncoding = System.Text.Encoding.UTF8;

            var cacheCategory = AttributeHelper.GetCache<T, CategoryAttribute>()
                .ToLookup(p => p.Value == null ? "" : p.Value.Category,
                    q => new KeyValuePair<int, T>((int) q.Key, (T) q.Key))
                .ToDictionary(p => p.Key, q => q.ToList().ToDictionary(p => p.Key, o => o.Value));

            var cacheType = AttributeHelper.GetCache<T, TypeValueAttribute>().ToDictionary(p => (int) p.Key);
            var cacheDesc = AttributeHelper.GetCacheDescription<T>()
                .ToDictionary(p => p.Key, q => string.IsNullOrEmpty(q.Value) ? q.Key.ToString() : q.Value);

            int maxLength = cacheDesc.Values.Max(p => p.Length);

            reset:
            bool y = false;
            do
            {
                Console.Clear();

                List<string> showList = new List<string>();

                foreach (KeyValuePair<string, Dictionary<int, T>> pair in cacheCategory)
                {
                    showList.Add(pair.Key);

                    var lineNum = (pair.Value.Count/columnsCount + 1);
                    var cache = pair.Value
                        .ToLookup(p => p.Key%lineNum, q => q)
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
                    int caoType = GetInputStr("请选择，然后回车：", "", "e", "^[0-9]*$").AsInt();

                    if (cacheType.ContainsKey(caoType))
                    {
                        Console.WriteLine("当前的选择：" + caoType);
                        do
                        {
                            var obj = Activator.CreateInstance(cacheType[caoType].Value.value);
                            if (callAction != null)
                                callAction.Invoke(obj);
                            y = ContinueY();
                            if (Input == "../")
                                goto reset;
                        } while (y);
                    }
                    else
                    {
                        Console.WriteLine("不存在的命令编号！");
                    }

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

        /// <summary>
        /// 用于控制台程序
        /// </summary>
        /// <returns></returns>
        public static string GetInputStr(string beforeTip = "请输入:", string afterTip = "", string def = "e",
            string regex = null)
        {
            Console.Write(beforeTip);
            var cmd = Console.ReadLine();
            Input = cmd;
            var str = string.IsNullOrEmpty(cmd) ? def : cmd;

            if (regex == null || Regex.IsMatch(str, "e") || Regex.IsMatch(str, regex))
            {
                if (str == "e")
                    QuitReadKey();
                str = str.Contains(" ") ? str.Substring(1, str.Length - 2) : str;
                Console.WriteLine(afterTip + str);
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
            return ContinueYStr(beforeTip) == "y";
        }

        /// <summary>
        /// 用于控制台是否继续，"按'y'键继续，按其余键退出， 
        /// </summary>
        /// <param name="beforeTip"> "press 'y' to continue : "; //"按'y'键继续！"</param>
        /// <returns></returns>
        public static string ContinueYStr(string beforeTip = "按'y'键继续（默认‘y’）： ")
        {
            Console.Write(Environment.NewLine);
            var x = GetInputStr(beforeTip, "", "y");
            Console.Write(Environment.NewLine);
            return x;
        }

       
        /// <summary>
        /// 用于控制台是否继续
        /// </summary>
        /// <param name="beforeTip"> </param>
        /// <returns></returns>
        public static void ContinueReadKey(string beforeTip = "按任意键继续！")
        {
            Console.WriteLine("按任意键继续！");
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
    }
}