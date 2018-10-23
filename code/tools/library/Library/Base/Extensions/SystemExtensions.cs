using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Helper;

namespace Library.Extensions
{
    public abstract class BaseSystemConsole
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
            string path = SystemConsole.GetInputStr(new SystemConsole.Model
            {
                beforeTip = CacheSelect[selectType],
                afterTip = "您选择的文件夹或文件："
            });
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
                        files = DirectoryHelper.GetFiles(path, exce, SearchOption.AllDirectories);
                    }
                    break;
                case SelectType.All:
                    if (Directory.Exists(path))
                    {
                        files = DirectoryHelper.GetFiles(path, exce, SearchOption.AllDirectories);
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
                    resetInput:
                    var index = GetInputStr(new Model {beforeTip = "请选择，然后回车：", def = "e", regex = "^[0-9]*$"}).AsInt();
                    if (cacheType.ContainsKey(index))
                    {
                        Console.WriteLine("当前的选择：" + index);
                        do
                        {
                            var obj = Activator.CreateInstance(cacheType[index].Value.value);
                            if (callAction != null)
                                callAction.Invoke(obj);
                        } while (ContinueY());
                    }
                    goto resetInput;

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

        public class Model
        {
            public string beforeTip = "请输入:";
            public string afterTip = "";
            public string def = "e";
            public string regex = null;
            public Dictionary<string, Action> config = new Dictionary<string, Action>();
        }

        /// <summary>
        /// 用于控制台程序
        /// </summary>
        /// <returns></returns>
        public static string GetInputStr(Model model)
        {
            Console.Write(model.beforeTip);
            var cmd = Console.ReadLine();
            Input = cmd;
            var str = string.IsNullOrEmpty(cmd) ? model.def : cmd;
            str = str.Contains(" ") ? str.Substring(1, str.Length - 2) : str;

            model.config = new Dictionary<string, Action>
            {
                {"e", () => { QuitReadKey(); }}
            }.Merge(model.config);

            Console.WriteLine(model.afterTip + str);

            if (model.regex == null || Regex.IsMatch(str, "e|" + model.regex))
            {
                if (model.config.ContainsKey(str))
                    model.config[str].Invoke();
                return str;

            }
            return GetInputStr(model);
        }

        /// <summary>
        /// 用于控制台程序
        /// </summary>
        /// <returns></returns>
        public static string GetInputStr(string beforeTip = "请输入:", string afterTip = "", string def = "e",
            string regex = null)
        {
            return GetInputStr(new Model()
            {
                beforeTip = beforeTip,
                afterTip = afterTip,
                def = def,
                regex = regex
            });
        }

        /// <summary>
        /// 用于控制台是否继续，"按'y'键继续，按其余键退出， 
        /// </summary>
        /// <param name="beforeTip"> "press 'y' to continue : "; //"按'y'键继续！"</param>
        /// <returns></returns>
        public static bool ContinueY(string beforeTip = "按'y'键继续（默认‘y’）： ")
        {
            Console.Write(Environment.NewLine);
            var x = GetInputStr(new Model()
            {
                beforeTip = beforeTip,
                def = "y",
            }) == "y";
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

        public static void SetProgress(int left, int top, float progress, string info = "")
        {
            Console.SetCursorPosition(left, top);
            Console.WriteLine("[{0}] {1}", progress.ToString("p0"), info);
        }

        public static void SetProgress(int top, float progress, string info = "")
        {
            SetProgress(0, top, progress, info);
        }
    }
}