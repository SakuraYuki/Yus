using System.Collections.Specialized;
using System.Linq;
using Yus.Diagnostics;

namespace System
{
    #region NameValueCollection

    /// <summary>
    /// 键值集合扩展方法
    /// </summary>
    public static class CollectionExtension
    {
        /// <summary>
        /// 获取指定键的字符串值，指定键不存在时使用默认值
        /// </summary>
        /// <param name="nvc">当前集合</param>
        /// <param name="key">要获取的键</param>
        /// <param name="defaultString">默认值</param>
        /// <returns></returns>
        public static string YusGetString(this NameValueCollection nvc, string key, string defaultString = null)
        {
            if (nvc == null) return defaultString;
            if (!nvc.AllKeys.Contains(key)) return defaultString;
            var value = nvc[key];
            var valString = value?.ToString();
            return string.IsNullOrEmpty(valString) ? defaultString : valString;
        }

        /// <summary>
        /// 获取指定键的值，指定键不存在时返回默认值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="nvc">当前集合</param>
        /// <param name="key">要获取的键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T YusGetValue<T>(this NameValueCollection nvc, string key, T defaultValue = default(T))
        {
            if (nvc == null) return defaultValue;
            if (!nvc.AllKeys.Contains(key)) return defaultValue;
            var value = nvc[key];

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
        /// 获取指定键的值，并将其反序列化为对象，指定键不存在时返回默认值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="nvc">当前集合</param>
        /// <param name="key">要获取的键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T YusGetObject<T>(this NameValueCollection nvc, string key, T defaultValue = default(T))
        {
            var str = nvc.YusGetString(key, null);

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

    #endregion
}
