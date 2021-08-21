using Library.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
                SystemConsole.SetProgress(string.Format("is now : {0} {1}", (((float)i) / target.Count).ToString("p"), p));
                //Console.WriteLine("is now : " + (((float) i)/target.Count).ToString("p") + "\t" + p);
                if (File.Exists(p)) callAction(p);
            });
        }

        /// <summary>
        /// 默认1000毫秒循环一次
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="callAction"></param>
        /// <param name="millisecondsTimeout"></param>
        public static void ForEachPathsAndSleep(this IEnumerable<string> paths, Action<string> callAction, int millisecondsTimeout = 1000)
        {
            paths.Select(p => p.Replace("\\", "/")).ToList().ForEach((p, i, target) =>
            {
                System.Threading.Thread.Sleep(millisecondsTimeout);
                SystemConsole.SetProgress(p, ((float)i) / target.Count);
                callAction(p);
            });
            SystemConsole.ClearProgress();
        }
    }

    public static partial class SystemConsole
    {
        class data
        {
            public string group = "";
            public string description = "";
            public object arges = null;
			public Type type = null;
            public Action action = null;

            public int Length()
            {
                return System.Text.Encoding.Default.GetBytes(description).Length;
            }

            public int ZhChLength()
            {
                var xx = description.ToCharArray().Where(p => 2 == System.Text.Encoding.Default.GetByteCount(p.ToString())).Count();
                return xx;
            }
        }

        class datastr
        {
            public string content = "";
            public ConsoleColor color = ConsoleColor.White;

            public int Length()
            {
                return System.Text.Encoding.Default.GetBytes(content).Length;
            }

            public void WriteLine()
            {
                Console.ForegroundColor = color;
                Console.WriteLine(content);
                Console.ResetColor();
            }
        }
		class dataParams
		{
			public List<data> datas;
			public int columnsCount;
		}

		static Stack<dataParams> stack = new Stack<dataParams>();

        private static void ShowCmd(List<data> datas, int columnsCount, Action<data> action)
        {
			columnsCount = stack.Count == 0 ? columnsCount : Math.Max(columnsCount, stack.Pop().columnsCount);

			stack.Push(new dataParams() { datas = datas, columnsCount = columnsCount });

            Console.Clear();

            int maxLength = datas.Max(p => p.Length() + 2);

            List<datastr> showList = new List<datastr>();

            foreach (var pair in datas.GroupBy(p => p.group).ToDictionary(p => p.Key))
            {
                showList.Add(new datastr()
                {
                    content = pair.Key,
                    color = ConsoleColor.Red,
                });

                for (int i = 0, max = pair.Value.Count(); i < max; i += columnsCount)
                {
                    var strs = pair.Value.Skip(i).Take(columnsCount).Select(p => p.description.PadRight(maxLength - p.ZhChLength(), '.')).ToArray();
                    showList.Add(new datastr()
                    {
                        content = string.Format("\t{0}", string.Join("\t", strs))
                    });
                }

                showList.Add(new datastr()
                {
                    content = "",
                });
            }

            int maxLine = showList.Max(p => p.Length() + columnsCount * 4 + 8);
            maxLine += (maxLine % 2 == 0 ? 0 : 1);

			Console.WindowWidth = maxLine = Math.Max(maxLine, Console.WindowWidth);

			showList.Add(new datastr()
            {
                content = "\n\t" + "e：exit\n"
            });
            showList.Add(new datastr()
            {
                content = "-".PadRight(maxLine, '-')
            });
            showList.Insert(0, new datastr()
            {
                content = "命令索引".Pad(maxLine - 4, '-') + "\n"
            });

			Console.WindowHeight = Math.Max(Console.WindowHeight, Math.Max(showList.Count, 30) + 10);

            showList.ForEach(p => p.WriteLine());

            do
            {
                try
                {
                    var index = GetInputStr("请选择，然后回车：", def: "e", regex: @"^[0-9]*$").AsInt();
                    Console.WriteLine("当前的选择：" + index);
                    action.Invoke(datas[index]);
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

        public static void Run(Action<object> callAction = null, int columnsCount = 4, string group = "", params Type[] types)
        {
            //Console.OutputEncoding = Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.ResetColor();

            var datas = new List<data>();

            foreach (var type in types)
            {
                if (type.IsEnum)
                {
                    foreach (var it in Enum.GetValues(type))
                    {
                        var field = type.GetField(it.ToString());
                        if (field == null) continue;
                        var typeValueAttribute = field.GetCustomAttributes(false).OfType<TypeValueAttribute>().FirstOrDefault();
                        if (typeValueAttribute == null) continue;

                        var categoryAttribute = field.GetCustomAttributes(false).OfType<CategoryAttribute>().FirstOrDefault();

                        var data = new data();

                        data.group = categoryAttribute == null ? "" : categoryAttribute.Category;
                        if (!string.IsNullOrEmpty(group) && group != data.group)
                            continue;

                        data.group = type.FullName + "/" + data.group;

                        var descriptionAttribute = field.GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault();
                        data.description = descriptionAttribute == null ? field.Name : descriptionAttribute.Description;
                        data.description = string.Format("{0:d2}：{1}", datas.Count, data.description);

						data.type = typeValueAttribute.value;
                        data.arges = typeValueAttribute.args;

                        datas.Add(data);
                    }
                }
            }

			ShowCmd(datas, columnsCount, data =>
			{
				var obj = data.arges == null ? Activator.CreateInstance(data.type) : Activator.CreateInstance(data.type, new object[] { data.arges });
				callAction?.Invoke(obj);
			});
		}

        public static void Run<T, T1, T2, T3, T4, T5, T6>(Action<object> callAction = null, int columnsCount = 4, string group = "")
            where T : struct
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
            where T6 : struct
        {
            Run(callAction, columnsCount, group, typeof(T), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        }
        public static void Run<T, T1, T2, T3, T4, T5>(Action<object> callAction = null, int columnsCount = 4, string group = "")
           where T : struct
           where T1 : struct
           where T2 : struct
           where T3 : struct
           where T4 : struct
           where T5 : struct
        {
            Run(callAction, columnsCount, group, typeof(T), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        }
        public static void Run<T, T1, T2, T3, T4>(Action<object> callAction = null, int columnsCount = 4, string group = "")
          where T : struct
          where T1 : struct
          where T2 : struct
          where T3 : struct
          where T4 : struct
        {
            Run(callAction, columnsCount, group, typeof(T), typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }
        public static void Run<T, T1, T2, T3>(Action<object> callAction = null, int columnsCount = 4, string group = "")
         where T : struct
         where T1 : struct
         where T2 : struct
         where T3 : struct
        {
            Run(callAction, columnsCount, group, typeof(T), typeof(T1), typeof(T2), typeof(T3));
        }
        public static void Run<T, T1, T2>(Action<object> callAction = null, int columnsCount = 4, string group = "")
         where T : struct
         where T1 : struct
         where T2 : struct
        {
            Run(callAction, columnsCount, group, typeof(T), typeof(T1), typeof(T2));
        }
        public static void Run<T, T1>(Action<object> callAction = null, int columnsCount = 4, string group = "")
         where T : struct
         where T1 : struct
        {
            Run(callAction, columnsCount, group, typeof(T), typeof(T1));
        }
        public static void Run<T>(Action<object> callAction = null, int columnsCount = 4, string group = "") where T : struct
        {
            Run(callAction, columnsCount, group, typeof(T));
        }

        public static void Run(Dictionary<string, Action> config, int columnsCount = 1, string group = "")
        {
            var datas = config.Select((p, i) =>
           {
               return new data()
               {
                   action = p.Value,
                   description = string.Format("{0:d2}：{1}", i, p.Key),
                   group = group
               };
           }).ToList();

            ShowCmd(datas, columnsCount, data =>
            {
                if (data.action != null)
                    data.action.Invoke();
            });
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
                };
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
            var msg = string.Format("[{0}] {1} {2}", "*".PadRight((int)Math.Floor(15 * progress), '*').PadRight(15, '-'), progress.ToString("p2"), info);
            Console.WriteLine(msg);
            _lastOffset = msg.Length / Console.WindowWidth + 1;
            Console.SetCursorPosition(0, Math.Max(0, Console.CursorTop - _lastOffset));
        }

        public static void ClearProgress()
        {
            Console.SetCursorPosition(0, Math.Max(0, Console.CursorTop + _lastOffset));
        }
    }

    public static partial class SystemConsole
    {
        public static string GetommandLineArgs(string key)
        {
            var args = Environment.GetCommandLineArgs().ToList();
            var index = args.IndexOf(key);
            return index == -1 ? null : args.Skip(index).Take(2).LastOrDefault();
        }
    }
}