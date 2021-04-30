﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SteamTool.Core.Common
{
    public static class Common
    {
        /// <summary>
        /// 时间戳转DateTime（系统时区）
        /// </summary>
        /// <returns></returns>
        public static DateTime ToDateTime(this long timestamp)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp), TimeZoneInfo.Local);
        }

        /// <summary>
        /// 返回当前时间戳
        /// </summary>
        /// <returns></returns>
        public static long CurrentMillis(this DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        /// <summary>
        /// 返回当前最后相对URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetCurrentPath(this string url)
        {
            int index = url.LastIndexOf("/");

            if (index != -1)
            {
                return url.Substring(0, index) + "/";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 移除字符串内所有\r \n \t
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemovePattern(this string inputString)
        {
            return inputString.Replace("\t", "").Replace("\r", "").Replace("\n", "");
        }

        /// <summary>
        /// 获取两个字符串中间的字符串
        /// </summary>
        /// <param name="str">要处理的字符串,例ABCD</param>
        /// <param name="str1">第1个字符串,例AB</param>
        /// <param name="str2">第2个字符串,例D</param>
        /// <param name="isContains">是否包含标志字符串</param>
        /// <returns>例返回C</returns>
        public static string Substring(this string str, string str1, string str2, bool isContains = false)
        {
            int i1 = str.IndexOf(str1);
            if (i1 < 0) //找不到返回空
            {
                return "";
            }

            int i2 = str.IndexOf(str2, i1 + str1.Length); //从找到的第1个字符串后再去找
            if (i2 < 0) //找不到返回空
            {
                return "";
            }
            if (isContains)
                return str.Substring(i1, i2 - i1 + str1.Length);
            else
                return str.Substring(i1 + str1.Length, i2 - i1 - str1.Length);
        }


        public static string GetValue(this Match match, Func<Match, bool> action)
        {
            if (action.Invoke(match))
                return match.Value.Trim();
            else
                return "";
        }

        public static IEnumerable<string> GetValues(this MatchCollection match, Func<Match, bool> action)
        {
            foreach (Match item in match)
            {
                if (action.Invoke(item))
                    yield return item.Value.Trim();
            }
        }

        /// <summary>
        /// Converts a wildcard to a regex.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to convert.</param>
        /// <returns>A regex equivalent of the given wildcard.</returns>
        public static bool IsWildcard(this string str, string pattern)
        {
            return Regex.IsMatch(str, "^" + Regex.Escape(pattern).
             Replace("\\*", ".*").
             Replace("\\?", ".") + "$");
        }

        /// <summary>
        /// 利用反射实现所有类型深拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [Obsolete("V2 中使用 MessagePack反序列化与序列化进行拷贝，同时仅扩展序列化接口，避免扩展大范围全局污染")]
        public static T Clone<T>(this T source)
        {
            //如果是字符串或值类型则直接返回
            if (source is string || source.GetType().IsValueType) return source;
            object retval = Activator.CreateInstance(source.GetType());
            FieldInfo[] fields = source.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, Clone(field.GetValue(source))); }
                catch { }
            }
            return (T)retval;
        }

        public static bool ContainsArg(this string[] args, string arg)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], arg, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsArg(this string[] args, string arg, out string value)
        {
            value = "";
            for (var i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], arg, StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length > i + 1)
                    {
                        value = args[i + 1];
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool ContainsArg(this string[] args, string arg, out int value)
        {
            value = 0;
            for (var i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], arg, StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length > i + 1 && int.TryParse(args[i + 1], out int val))
                    {
                        value = val;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
