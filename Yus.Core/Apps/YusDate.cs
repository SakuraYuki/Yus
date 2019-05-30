using System;

// ReSharper disable InconsistentNaming

namespace Yus.Apps
{
    /// <summary>时间日期模块</summary>
    public class YusDate
    {
        /// <summary>时间格式，2008-01-01</summary>
        public const string Date = "yyyy-MM-dd";
        /// <summary>日期格式，12:30:50</summary>
        public const string Time = "HH:mm:ss";
        /// <summary>时间格式（中文），2008年01月01日</summary>
        public const string DateCN = "yyyy年MM月dd日";
        /// <summary>日期格式（中文），12时30分50秒</summary>
        public const string TimeCN = "HH时mm分ss秒";
        /// <summary>时间日期格式（中文），2008年01月01日 12时30分50秒</summary>
        public const string DateTime = "yyyy-MM-dd HH:mm:ss";
        /// <summary>时间日期格式（中文），2008年01月01日 12时30分50秒</summary>
        public const string DateTimeCN = "yyyy年MM月dd日 HH时mm分ss秒";
        /// <summary>时间日期格式，包含毫秒，2008-01-01 12:30:50 356</summary>
        public const string LongTime = "yyyy-MM-dd HH:mm:ss fff";
        /// <summary>时间日期格式，包含毫秒（中文），2008年01月01日 12时30分50秒 356毫秒</summary>
        public const string LongTimeCN = "yyyy年MM月dd日 HH时mm分ss秒 fff毫秒";
        /// <summary>日期格式，下划线连接，2008_01_01</summary>
        public const string DateUnderline = "yyyy_MM_dd";
        /// <summary>日期格式，下划线连接，12_30_50</summary>
        public const string TimeUnderline = "HH_mm_ss";
        /// <summary>时间日期格式，下划线连接，2008_01_01_12_30_50</summary>
        public const string DateTimeUnderline = "yyyy_MM_dd_HH_mm_ss";
        /// <summary>时间日期格式，包含毫秒，下划线连接，2008_01_01_12_30_50_356</summary>
        public const string LongTimeUnderline = "yyyy_MM_dd_HH_mm_ss_fff";
        /// <summary>日期格式，斜杠连接，2008/01/01</summary>
        public const string DateSlash = "yyyy/MM/dd";

        /// <summary>起始时间</summary>
        public static readonly DateTime OriginDate = new DateTime(1970, 1, 1);

        /// <summary>获取 UTC+8 时区当前的时间</summary>
        public static DateTime NowP8 => System.DateTime.UtcNow.AddHours(8);

        /// <summary>默认时间，2000-01-01 12:00:00</summary>
        public static DateTime DefaultTime => System.DateTime.Parse("2000-01-01 12:00:00 AM");

        /// <summary>
        /// Unix时间戳转换为C# DateTime
        /// </summary>
        /// <param name="timestamp">Unix时间戳</param>
        /// <returns></returns>
        public static DateTime UnixToDate(long timestamp)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(OriginDate); // 当地时区
            return startTime.AddSeconds(timestamp);
        }

        /// <summary>
        /// JavaScript 时间戳转换为 C# DateTime
        /// </summary>
        /// <param name="timestamp">JavaScript时间戳</param>
        /// <returns></returns>
        public static DateTime JsToDateTime(long timestamp)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(OriginDate); // 当地时区
            return startTime.AddMilliseconds(timestamp);
        }

        /// <summary>
        /// C# DateTime 转换为 Unix 时间戳(10位,秒数)
        /// </summary>
        /// <param name="date">要转换的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static long DateTimeToUnix(DateTime date = default(DateTime))
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(OriginDate); // 当地时区
            return (long)((date.ToBinary() == 0 ? System.DateTime.Now : date) - startTime).TotalSeconds; // 相差秒数
        }

        /// <summary>
        /// C# DateTime 转换为 JavaScript 时间戳(13位,毫秒数)
        /// </summary>
        /// <param name="date">要转换的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static long DateTimeToJs(DateTime date = default(DateTime))
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(OriginDate); // 当地时区
            return (long)((date.ToBinary() == 0 ? System.DateTime.Now : date) - startTime).TotalMilliseconds; // 相差毫秒数
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="Date"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringDate(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, Date);
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="Time"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringTime(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, Time);
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="DateCN"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringDateCN(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, DateCN);
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="TimeCN"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringTimeCN(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, TimeCN);
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="DateTime"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringDateTime(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, DateTime);
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="DateTimeCN"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringDateTimeCN(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, DateTimeCN);
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="LongTime"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringLongTime(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, LongTime);
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="LongTimeCN"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringLongTimeCN(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, LongTimeCN);
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="DateUnderline"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringDateUnderline(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, DateUnderline);
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="TimeUnderline"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringTimeUnderline(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, TimeUnderline);
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="DateTimeUnderline"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringDateTimeUnderline(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, DateTimeUnderline);
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="LongTimeUnderline"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringLongTimeUnderline(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, LongTimeUnderline);
        }

        /// <summary>
        /// 格式化字符串，使用<see cref="DateSlash"/>的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间，不传入则为当前时间</param>
        /// <returns></returns>
        public static string StringDateSlash(DateTime date = default(DateTime))
        {
            return InnerStringProcess(date, DateSlash);
        }

        /// <summary>
        /// 格式化字符串，使用指定的格式来格式化输出
        /// </summary>
        /// <param name="date">要格式化的时间</param>
        /// <param name="format">格式化规则</param>
        /// <returns></returns>
        private static string InnerStringProcess(DateTime date, string format)
        {
            return (date.ToBinary() == 0 ? System.DateTime.Now : date).ToString((format));
        }
    }
}
