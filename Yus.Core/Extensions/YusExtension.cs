using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Yus.Diagnostics;

namespace System
{
    /// <summary>
    /// Yus通用扩展
    /// </summary>
    public static class YusExtension
    {
        /// <summary>
        /// 截断指定位数小数
        /// </summary>
        /// <param name="value">要截断的数</param>
        /// <param name="number">要截断的小数位</param>
        /// <returns></returns>
        public static string YusCut(this float value, int number = 2)
        {
            var str = value.ToString();
            var index = str.IndexOf('.');
            if (index == -1 || index + number > str.Length - 1) return str;
            return str.Substring(0, index + number + 1);
        }

        /// <summary>
        /// 截断指定位数小数
        /// </summary>
        /// <param name="value">要截断的数</param>
        /// <param name="number">要截断的小数位</param>
        /// <returns></returns>
        public static string YusCut(this double value, int number = 2)
        {
            var str = value.ToString();
            var index = str.IndexOf('.');
            if (index == -1 || index + number > str.Length - 1) return str;
            return str.Substring(0, index + number + 1);
        }
    }
}
