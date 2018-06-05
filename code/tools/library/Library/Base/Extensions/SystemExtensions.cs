using System;
using System.Collections.Generic;
using System.Linq;
using Library.Helper;

namespace Library.Extensions
{
    public class SystemConsole
    {
        public static void Run<T>(Action<object> callAction = null) where T : struct
        {
            var cacheType = AttributeHelper.GetCache<T, TypeValueAttribute>().ToDictionary(p => (int) p.Key);
            var cacheDesc = AttributeHelper.GetCacheDescription<T>();
            do
            {
                Console.Clear();

                List<string> showList = new List<string>();
                foreach (var value in Enum.GetValues(typeof (T)))
                {
                    var tips = cacheDesc[(T) value];
                    tips = string.IsNullOrEmpty(tips) ? value.ToString() : tips;
                    showList.Add(string.Format("\t{0}\t", (int) value + "：" + tips));
                }
                int maxLine = showList.Max(p => p.Length) + 20;
                showList.Add(' '.CopyChar(maxLine));
                showList.Add('-'.CopyChar(maxLine));
                showList.Insert(0, ' '.CopyChar(maxLine));
                showList.Insert(0, '-'.CopyChar(maxLine/2 - 2) + "命令索引" + '-'.CopyChar(maxLine/2 - 2));
                showList.ForEach(Console.WriteLine);

                try
                {
                    int caoType = SystemExtensions.GetInputStr("请选择，然后回车：").AsInt();

                    if (cacheType.ContainsKey(caoType))
                    {
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
            } while (SystemExtensions.ContinueY());
        }
    }

    public class SystemExtensions
    {
        /// <summary>
        /// 用于控制台程序
        /// </summary>
        /// <returns></returns>
        public static string GetInputStr(string beforeTip = "请输入:", string afterTip = "", string def = "e")
        {
            Console.Write(beforeTip);
            var cmd = Console.ReadLine();
            var str = string.IsNullOrEmpty(cmd) ? def : cmd;
            if (str == "e")
                QuitReadKey();
            str = str.Contains(" ") ? str.Substring(1, str.Length - 2) : str;
            Console.WriteLine(afterTip + str);
            return str;
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