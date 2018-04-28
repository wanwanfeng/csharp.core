using System;

namespace Library.Extensions
{
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
        /// 用于控制台是否继续
        /// </summary>
        /// <param name="beforeTip"> "press 'y' to continue : "; //"按'y'键继续！"</param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static bool Continue(string beforeTip = "按'y'键继续： ", string def = "y")
        {
            Console.Write(Environment.NewLine);
            var x = GetInputStr(beforeTip, "", def) == "y";
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