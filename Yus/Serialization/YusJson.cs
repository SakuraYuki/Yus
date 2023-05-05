using Newtonsoft.Json;
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
        /// <param name="formatting">JSON格式，对齐还是压缩</param>
        /// <param name="settings">序列化设置</param>
        /// <returns></returns>
        public static string ToJson(object obj, Formatting formatting = Formatting.None, JsonSerializerSettings settings = null)
        {
            if (obj == null) return null;
            return JsonConvert.SerializeObject(obj, formatting: formatting, settings: settings);
        }

        /// <summary>
        /// 反序列化 JSON 到对象
        /// </summary>
        /// <typeparam name="T">要序列化成哪种对象</typeparam>
        /// <param name="json">JSON 字符</param>
        /// <param name="settings">反序列化设置</param>
        /// <returns></returns>
        public static T ToObject<T>(string json, JsonSerializerSettings settings = null)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (Exception e)
            {
                Yus.Diagnostics.YusDebug.ConsoleLog(e, nameof(ToObject));
            }
            return default;
        }
    }
}
