using System;
using Library.Extensions;
using UnityEngine;

namespace Library
{
    /// <summary>
    /// Unity扩展
    /// </summary>
    public static class UnityExtensions
    {
        /// <summary>
        /// 转为Vector3型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static Vector3 AsVector3(this string value, char separator = ',')
        {
            if (string.IsNullOrEmpty(value)) return Vector3.zero;
            string[] result = value.Split(separator);
            var temp = Array.ConvertAll(result, p => p.AsFloat());
            switch (temp.Length)
            {
                default:
                    return Vector3.zero;
                case 1:
                    return new Vector3(temp[0], 0, 0);
                case 2:
                    return new Vector3(temp[0], temp[1], 0);
                case 3:
                    return new Vector3(temp[0], temp[1], temp[2]);
            }
        }

        /// <summary>
        /// 转为Vector2型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static Vector2 AsVector2(this string value, char separator = ',')
        {
            if (string.IsNullOrEmpty(value)) return Vector2.zero;
            var result = value.Split(separator);
            var temp = Array.ConvertAll(result, p => p.AsFloat());
            switch (temp.Length)
            {
                default:
                    return Vector2.zero;
                case 1:
                    return new Vector2(temp[0], 0);
                case 2:
                    return new Vector2(temp[0], temp[1]);
            }
        }

        /// <summary>
        /// 转为string型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string AsString(this Vector3 value, char separator = ',')
        {
            return string.Format("{1}{0}{2}{0}{3}", separator, value.x, value.y, value.z);
        }

        /// <summary>
        /// 转为string型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string AsString(this Vector2 value, char separator = ',')
        {
            return string.Format("{1}{0}{2}", separator, value.x, value.y);
        }

        /// <summary>
        ///随机位置
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static Vector3 RangeVector3(this Vector3 from, Vector3 to)
        {
            return new Vector3(UnityEngine.Random.Range(from.x, to.x), UnityEngine.Random.Range(from.y, to.y),
                UnityEngine.Random.Range(from.z, to.z));
        }

        /// <summary>
        /// 随机位置
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static Vector2 RangeVector2(this Vector2 from, Vector2 to)
        {
            return new Vector3(UnityEngine.Random.Range(from.x, to.x), UnityEngine.Random.Range(from.y, to.y));
        }
    }
}