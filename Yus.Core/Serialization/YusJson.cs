using Swifter.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yus.Serialization
{
    /// <summary>JSON 相关序列化/反序列化工具</summary>
    public class YusJson
    {
        private static JsonFormatterOptions _settings;
        /// <summary>
        /// 默认序列化设置
        /// </summary>
        public static JsonFormatterOptions Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        /// <summary>
        /// 序列化对象到 JSON 字符串
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="indented">是否格式化</param>
        /// <param name="sets">序列化设置</param>
        /// <returns></returns>
        public static string ToJson(object obj, bool indented = false, JsonFormatterOptions sets = JsonFormatterOptions.Default)
        {
            if (obj == null) return null;
            return JsonFormatter.SerializeObject(obj, sets);
        }

        /// <summary>
        /// 反序列化 JSON 到对象
        /// </summary>
        /// <typeparam name="T">要序列化成哪种对象</typeparam>
        /// <param name="json">JSON 字符</param>
        /// <returns></returns>
        public static T ToObject<T>(string json)
        {
            try
            {
                return JsonFormatter.DeserializeObject<T>(json, Settings);
            }
            catch (Exception e)
            {
                Yus.Diagnostics.YusDebug.ConsoleLog(e, nameof(ToObject));
            }
            return default(T);
        }
    }
}
