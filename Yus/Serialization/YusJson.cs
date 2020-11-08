using Swifter.Json;
using System;

namespace Yus.Serialization
{
    /// <summary>JSON 相关序列化/反序列化工具</summary>
    public static class YusJson
    {
        /// <summary>
        /// 序列化对象到 JSON 字符串
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="sets">序列化设置</param>
        /// <returns></returns>
        public static string ToJson(object obj, JsonFormatterOptions sets = JsonFormatterOptions.Default)
        {
            if (obj == null) return null;
            return JsonFormatter.SerializeObject(obj, sets);
        }

        /// <summary>
        /// 反序列化 JSON 到对象
        /// </summary>
        /// <typeparam name="T">要序列化成哪种对象</typeparam>
        /// <param name="json">JSON 字符</param>
        /// <param name="sets">反序列化设置</param>
        /// <returns></returns>
        public static T ToObject<T>(string json, JsonFormatterOptions sets = JsonFormatterOptions.Default)
        {
            try
            {
                return JsonFormatter.DeserializeObject<T>(json, sets);
            }
            catch (Exception e)
            {
                Yus.Diagnostics.YusDebug.ConsoleLog(e, nameof(ToObject));
            }
            return default;
        }
    }
}
