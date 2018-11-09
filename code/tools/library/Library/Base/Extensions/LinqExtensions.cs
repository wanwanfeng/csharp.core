using System;
using System.Collections.Generic;

namespace Library.Extensions
{
    /// <summary>
    /// Linq扩展
    /// </summary>
    public static class LinqExtensions
    {
        public static void Parallel<T>(this List<T> value, Action<T> callAction)
        {
           // Parallel.ForEach(CheckPath(exs), p => { });
        }
    }
}