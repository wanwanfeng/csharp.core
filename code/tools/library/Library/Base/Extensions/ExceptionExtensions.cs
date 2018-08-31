using System.Collections.Generic;

namespace Library.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 查找最终异常
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static List<System.Exception> FinalException(this System.Exception exception)
        {
            var list = new List<System.Exception>() { exception };
            do
            {
                exception = exception.InnerException;
                if (exception != null)
                    list.Add(exception);
            } while (exception != null && exception.InnerException != null);
            return list;
        }
    }
}