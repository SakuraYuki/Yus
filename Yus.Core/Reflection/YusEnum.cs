using System;
using System.Collections.Generic;

namespace Yus.Reflection
{
    /// <summary>枚举处理模块</summary>
    public class YusEnum
    {
        /// <summary>
        /// 获取枚举的名字和值的对应字典
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<string, int> GetEnumKeyValueDict(Type enumType)
        {
            var kvs = new Dictionary<string, int>();
            var enums = Enum.GetValues(enumType);
            foreach (var e in enums) kvs.Add(e.ToString(), e.YusToInt32());
            return kvs;
        }

        /// <summary>
        /// 获取枚举的名字和值的对应键值对集合
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static List<KeyValuePair<string, int>> GetEnumKeyValueList(Type enumType)
        {
            var kvs = new List<KeyValuePair<string, int>>();
            var enums = Enum.GetValues(enumType);
            foreach (var e in enums) kvs.Add(new KeyValuePair<string, int>(e.ToString(), e.YusToInt32()));
            return kvs;
        }

        /// <summary>
        /// 获取枚举的名字和备注的对应字典
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<int, string> GetEnumDesc(Type enumType)
        {
            var kvs = new Dictionary<int, string>();
            var enums = Enum.GetValues(enumType).GetEnumerator();
            var fields = enumType.GetFields();
            for (int i = 0; enums.MoveNext(); i++)
            {
                var curr = enums.Current;
                kvs.Add(curr.YusToInt32(), fields[i + 1].GetDescription());
            }

            return kvs;
        }
    }
}
