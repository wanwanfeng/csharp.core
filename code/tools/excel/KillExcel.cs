using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace excel
{
    class KillExcel
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);


        /// <summary>
        /// 强制关闭当前Excel进程
        /// </summary>
        public static void Kill(IntPtr intPtr)
        {
            try
            {
                Process[] ps = Process.GetProcesses();
                int ExcelID = 0;
                GetWindowThreadProcessId(intPtr, out ExcelID); //得到本进程唯一标志k   
                foreach (Process p in ps)
                {
                    if (p.ProcessName.ToLower().Equals("excel"))
                    {
                        if (p.Id == ExcelID)
                        {
                            p.Kill();
                        }
                    }
                }
            }
            catch
            {
                //不做任何处理   
            }
        }
    }
}
