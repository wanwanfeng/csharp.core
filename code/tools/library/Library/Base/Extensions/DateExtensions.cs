using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Extensions
{
    /// <summary>
    /// Date扩展
    /// </summary>
    public static class DateExtensions
    {
        public static string Format(this DateTime date, string format = "YYYY/MM/DD hh:mm:ss SSS")
        {
            new Dictionary<string, string>()
            {
                {"YYYY", date.Year.ToString("0000")},
                {"YY", date.Year.ToString("0000").Substring(2)},
                {"yyyy", date.Year.ToString("0000")},
                {"yy", date.Year.ToString("0000").Substring(2)},

                {"MM", date.Month.ToString("00")},
                {"M", date.Month.ToString()},

                {"DD", date.Day.ToString("00")},
                {"D", date.Day.ToString()},
                {"dd", date.Day.ToString("00")},
                {"d", date.Day.ToString()},

                {"hh", date.Hour.ToString("00")},
                {"h", date.Hour.ToString()},

                {"mm", date.Minute.ToString("00")},
                {"m", date.Minute.ToString()},

                {"ss", date.Second.ToString("00")},
                {"s", date.Second.ToString()},

                {"SSS", date.Millisecond.ToString("000")},
                {"S", date.Millisecond.ToString()},
            }.ForEach(p =>
            {
                format = format.Replace(p.Key, p.Value);
            });

            return format;
        }
    }
}