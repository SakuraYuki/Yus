using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Yus.Text;

namespace System
{
    /// <summary>
    /// 字符串相关扩展
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 将 C:\WorkSpace\Project 转换成 C:/WorkSpace/Project 的形式
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>替换后的字符串</returns>
        public static string YusPathCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            return str.Replace("\\", "/");
        }

        /// <summary>
        /// 比较字符串是否相等
        /// </summary>
        /// <param name="str">当前字符串</param>
        /// <param name="str1">要比较的字符串</param>
        /// <param name="sc">比较规则</param>
        /// <returns>是否相等</returns>
        public static bool YusEq(this string str, string str1, StringComparison sc = StringComparison.CurrentCulture)
        {
            if (str == null) return false;
            return str.Equals(str1, sc);
        }

        /// <summary>
        /// 如果字符串是空字符串（null，Empty，WhiteSpace），就替换为默认值
        /// </summary>
        /// <param name="str">当前字符串</param>
        /// <param name="val">默认值</param>
        /// <returns>原值或者默认值</returns>
        public static string NullReplace(this string str, string val)
        {
            return string.IsNullOrWhiteSpace(str) ? val : str;
        }

        /// <summary>
        /// 获取字符串的 MD5 值
        /// </summary>
        /// <param name="text">要获取的字符串</param>
        /// <param name="lowerCase">是否小写输出</param>
        /// <param name="line">是否包含短线</param>
        /// <param name="encoding">编码规则</param>
        /// <returns>MD5值</returns>
        public static string YusMd5(this string text, bool lowerCase = false, bool line = false, Encoding encoding = null)
        {
            return Yus.Text.YusMd5.GetMd5(text, lowerCase: lowerCase, line: line, encoding: encoding);
        }

        /// <summary>
        /// Base64 加密
        /// </summary>
        /// <param name="text">要加密的字符串</param>
        /// <exception cref="ArgumentException">null不可以参与加密</exception>
        /// <returns>Base64加密后的字符串</returns>
        public static string YusBase64Encode(this string text)
        {
            if (text == null) throw new ArgumentException("null 无法进行 Base64 编码");
            return Yus.Security.YusBase64.Base64Encode(text);
        }

        /// <summary>
        /// Base64 解密
        /// </summary>
        /// <param name="base64">要解密的 Base64 字符串</param>
        /// <exception cref="ArgumentException">null不可以参与解密</exception>
        /// <returns>Base64加密后的字符串</returns>
        public static string YusBase64Decode(this string base64)
        {
            if (base64 == null) throw new ArgumentException("null 无法进行 Base64 编码");
            return Yus.Security.YusBase64.Base64Decode(base64);
        }

        /// <summary>
        /// 将 <see cref="string"/>(JSON) 转换到指定对象
        /// </summary>
        /// <param name="value"><see cref="string"/>(JSON) 值</param>
        /// <param name="sets">反序列化设置</param>
        /// <returns></returns>
        public static T YusToObject<T>(this string value, Swifter.Json.JsonFormatterOptions sets = Swifter.Json.JsonFormatterOptions.Default)
        {
            return Yus.Serialization.YusJson.ToObject<T>(value, sets: sets);
        }

        /// <summary>
        /// 字符串是空或者null
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <returns></returns>
        public static bool YusNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 字符串不是空或者null
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <returns></returns>
        public static bool YusNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 字符串是空或者null或空格字符
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <returns></returns>
        public static bool YusNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 字符串不是空或者null或空格字符
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <returns></returns>
        public static bool YusNotNullOrWhiteSpace(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        /// <summary>忽略大小写的字符串相等比较，判断是否以任意一个待比较字符串相等</summary>
        /// <param name="value">数值</param>
        /// <param name="strs">待比较字符串数组</param>
        /// <returns></returns>
        public static bool YusEqualIgnoreCase(this string value, params string[] strs)
        {
            foreach (var item in strs)
            {
                if (string.Equals(value, item, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        /// <summary>
        /// 字符串是否匹配指定正则
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <param name="regex">正则表达式</param>
        /// <param name="opts">匹配选项</param>
        /// <returns></returns>
        public static bool YusIsMatch(this string str, string regex, RegexOptions opts = RegexOptions.None)
        {
            return Regex.IsMatch(str, regex, opts);
        }

        /// <summary>
        /// 字符串是否不匹配指定正则
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <param name="regex">正则表达式</param>
        /// <param name="opts">匹配选项</param>
        /// <returns></returns>
        public static bool YusNotMatch(this string str, string regex, RegexOptions opts = RegexOptions.None)
        {
            return !Regex.IsMatch(str, regex, opts);
        }

        /// <summary>
        /// 字符串是否是MD5值
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <param name="lenght">长度</param>
        /// <param name="upper">MD5是否是大写</param>
        /// <returns></returns>
        public static bool YusIsMatchMd5(this string str, int lenght = 32, bool upper = true)
        {
            return upper ? str.YusIsMatch("^[A-Z0-9]{" + lenght + "}$") : str.YusIsMatch("^[a-zA-Z0-9]{" + lenght + "}$");
        }
    }
}
