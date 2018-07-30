using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Helper;

namespace Library.Extensions
{
    public class SystemConsole
    {
        public static void Run<T>(Action<object> callAction = null, int columnsCount = 1) where T : struct
        {
            Console.Title = typeof (T).Namespace ?? Console.Title;
            //Console.OutputEncoding = Console.InputEncoding = System.Text.Encoding.UTF8;

            var cacheCategory = AttributeHelper.GetCache<T, CategoryAttribute>()
                .ToLookup(p => p.Value == null ? "" : p.Value.Category,
                    q => new KeyValuePair<int, T>((int) q.Key, (T) q.Key))
                .ToDictionary(p => p.Key, q => q.ToList().ToDictionary(p => p.Key, o => o.Value));

            var cacheType = AttributeHelper.GetCache<T, TypeValueAttribute>().ToDictionary(p => (int)p.Key);
            var cacheDesc = AttributeHelper.GetCacheDescription<T>()
                .ToDictionary(p => p.Key, q => string.IsNullOrEmpty(q.Value) ? q.Key.ToString() : q.Value);

            int maxLength = cacheDesc.Values.Max(p => p.Length);

            do
            {
                Console.Clear();

                List<string> showList = new List<string>();

                foreach (KeyValuePair<string, Dictionary<int, T>> pair in cacheCategory)
                {
                    showList.Add(pair.Key);

                    var lineNum = (pair.Value.Count / columnsCount + 1);
                    var cache = pair.Value
                        .ToLookup(p => p.Key % lineNum, q => q)
                        .ToDictionary(p => p.Key, q => q.ToList());

                    foreach (var value in cache.Values)
                    {
                        var res = value.Select(p => string.Format("{0:d2}：{1}", p.Key, cacheDesc[p.Value].PadRight(maxLength, '.')));
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
                    int caoType = SystemConsole.GetInputStr("请选择，然后回车：", "", "e", "^[0-9]*$").AsInt();

                    if (cacheType.ContainsKey(caoType))
                    {
                        Console.WriteLine("当前的选择：" + caoType);
                        var obj = Activator.CreateInstance(cacheType[caoType].Value.value);
                        if (callAction != null)
                        {
                            callAction.Invoke(obj);
                        }
                    }
                    else
                    {
                        Console.WriteLine("不存在的命令编号！");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
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
        public static bool ContinueY(string beforeTip = "按'y'键继续： ")
        {
            Console.Write(Environment.NewLine);
            var x = GetInputStr(beforeTip, "", "y") == "y";
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