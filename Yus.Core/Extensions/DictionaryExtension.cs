using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yus.Diagnostics;

namespace System
{
    /// <summary>字典扩展方法</summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 获取字典中指定键的字符串值，如果键不存在则返回指定的默认值
        /// </summary>
        /// <param name="dict">要获取值的字典</param>
        /// <param name="key">指定键</param>
        /// <param name="defaultString">键不存在时返回的默认值</param>
        /// <returns>指定键的值或者默认值</returns>
        public static string YusDictString(this Dictionary<string, object> dict, string key, string defaultString = null)
        {
            if (dict == null) return defaultString;
            if (!dict.ContainsKey(key)) return defaultString;
            var value = dict[key];
            var valString = value?.ToString();
            return valString == null || string.IsNullOrEmpty(valString) ? defaultString : valString;
        }

        /// <summary>
        /// 获取字典中指定键的值，并将其转化为指定类型，如果键不存在则返回指定的默认值
        /// </summary>
        /// <typeparam name="T">要指定的返回值类型</typeparam>
        /// <param name="dict">要获取值的字典</param>
        /// <param name="key">指定键</param>
        /// <param name="defaultValue">键不存在时返回的默认值</param>
        /// <returns>指定键的值或者默认值</returns>
        public static T YusDictValue<T>(this Dictionary<string, object> dict, string key, T defaultValue = default(T))
        {
            if (dict == null) return defaultValue;
            if (!dict.ContainsKey(key)) return defaultValue;
            var value = dict[key];

            try
            {
                return value.YusToJson().YusToObject<T>();
            }
            catch (Exception e)
            {
                YusDebug.ConsoleLog(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取字典中指定键的值（该值必须是要转化对象的 JSON 字符串），并将其序列化为指定对象，如果键不存在或 JSON 不正确则返回指定的默认值
        /// </summary>
        /// <typeparam name="T">要指定的返回值类型</typeparam>
        /// <param name="dict">要获取值的字典</param>
        /// <param name="key">指定键</param>
        /// <param name="defaultValue">键不存在或 JSON 不正确时返回的默认值</param>
        /// <returns>指定键的值或者默认值</returns>
        public static T YusDictObject<T>(this Dictionary<string, object> dict, string key, T defaultValue = default(T))
        {
            var str = dict.YusDictString(key, null);

            try
            {
                return str.YusToObject<T>();
            }
            catch (Exception e)
            {
                YusDebug.ConsoleLog(e);
                return defaultValue;
            }
        }
    }
}
