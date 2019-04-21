using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yus.Core.Apps
{
    /// <summary>时间日期模块</summary>
    public class YusDate
    {
        /// <summary>
        /// 时间格式，2008-01-01
        /// </summary>
        public const string Date = "yyyy-MM-dd";
        /// <summary>
        /// 日期格式，12:30:50
        /// </summary>
        public const string Time = "hh:mm:ss";
        /// <summary>
        /// 时间格式（中文），2008年01月01日
        /// </summary>
        public const string DateCN = "yyyy年MM月dd日";
        /// <summary>
        /// 日期格式（中文），12时30分50秒
        /// </summary>
        public const string TimeCN = "hh时mm分ss秒";
        /// <summary>
        /// 时间日期格式（中文），2008年01月01日 12时30分50秒
        /// </summary>
        public const string DateTimeCN = "yyyy年MM月dd日 hh:mm:ss";
        /// <summary>
        /// 时间日期格式，包含毫秒，2008-01-01 12:30:50 356
        /// </summary>
        public const string LongTime = "yyyy年MM月dd日 hh:mm:ss ffff";
        /// <summary>
        /// 时间日期格式，包含毫秒（中文），2008年01月01日 12时30分50秒 356毫秒
        /// </summary>
        public const string LongTimeCN = "yyyy年MM月dd日 hh:mm:ss ffff";
        /// <summary>
        /// 日期格式，下划线连接，2008_01_01
        /// </summary>
        public const string DateUnderline = "yyyy_MM_dd";
        /// <summary>
        /// 日期格式，下划线连接，12_30_50
        /// </summary>
        public const string TimeUnderline = "hh_mm_ss";
        /// <summary>
        /// 时间日期格式，下划线连接，2008_01_01_12_30_50
        /// </summary>
        public const string DateTimeUnderline = "yyyy_MM_dd_hh_mm_ss";
        /// <summary>
        /// 时间日期格式，包含毫秒，下划线连接，2008_01_01_12_30_50_356
        /// </summary>
        public const string LongTimeUnderline = "yyyy_MM_dd_hh_mm_ss_ffff";
        /// <summary>
        /// 日期格式，斜杠连接，2008/01/01
        /// </summary>
        public const string DateSlash = "yyyy/MM/dd";

        /// <summary>
        /// 获取 UTC+8 时区当前的时间
        /// </summary>
        public static DateTime NowP8 => DateTime.UtcNow.AddHours(8);

        /// <summary>
        /// 默认时间，2000-01-01 12:00:00
        /// </summary>
        public static DateTime DefaultTime => DateTime.Parse("2000-01-01 12:00:00 AM");

        /// <summary>
        /// Unix时间戳转换为C# DateTime
        /// </summary>
        /// <param name="timestamp">Unix时间戳</param>
        /// <returns></returns>
        public static DateTime UnixToDate(long timestamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            return startTime.AddSeconds(timestamp);
        }

        /// <summary>
        /// JavaScript 时间戳转换为 C# DateTime
        /// </summary>
        /// <param name="timestamp">JavaScript时间戳</param>
        /// <returns></returns>
        public static DateTime JsToDate(long timestamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            return startTime.AddMilliseconds(timestamp);
        }

        /// <summary>
        /// C# DateTime 转换为 Unix 时间戳(10位,秒数)
        /// </summary>
        /// <returns></returns>
        public static long DateTimeToUnix(DateTime date = default(DateTime))
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            return (long)((date.ToBinary() == 0 ? DateTime.Now : date) - startTime).TotalSeconds; // 相差秒数
        }

        /// <summary>
        /// C# DateTime 转换为 JavaScript 时间戳(13位,毫秒数)
        /// </summary>
        /// <returns></returns>
        public static long DateTimeToJs()
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            return (long)(DateTime.Now - startTime).TotalMilliseconds; // 相差毫秒数
        }

        /// <summary>
        /// C# DateTime 转换为 JavaScript 时间戳(13位,毫秒数)
        /// </summary>
        /// <param name="date">指定时间</param>
        /// <returns></returns>
        public static long DateTimeToJs(DateTime date)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            return (long)(date - startTime).TotalMilliseconds; // 相差毫秒数
        }
    }
}
