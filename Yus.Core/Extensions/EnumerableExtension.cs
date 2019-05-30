using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 迭代器扩展方法
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        /// 使用指定字符串连接元素
        /// </summary>
        /// <typeparam name="T">元素类型, 如果元素不是字符串, 将使用 <see cref="Convert.ToString(object)"/> 方法转换后连接</typeparam>
        /// <param name="list">集合</param>
        /// <param name="split">连接符</param>
        /// <returns></returns>
        public static string YusJoinString<T>(this IEnumerable<T> list, string split)
        {
            if (list == null) return null;
            var count = list.Count();
            if (count == 0) return "";
            if (count == 1) return Convert.ToString(list.First());

            split = split == null ? "" : split;

            var sb = new StringBuilder(Convert.ToString(list.First()));
            foreach (var item in list.Skip(1)) sb.Append(split).Append(Convert.ToString(item));

            return sb.ToString();
        }

        /// <summary>
        /// 使用指定动作遍历集合
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="action">对每个元素执行的动作</param>
        public static void YusForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (list == null) return;
            foreach (T item in list) action?.Invoke(item);
        }

        /// <summary>
        /// 判定是 null 或 空
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">集合</param>
        public static bool YusNullOrZero<T>(this IEnumerable<T> list) { return list == null || list.Count() == 0; }

        /// <summary>
        /// 判定不是 null 或 空
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">集合</param>
        public static bool YusNotNullOrZero<T>(this IEnumerable<T> list) { return list != null && list.Count() > 0; }
    }
}
