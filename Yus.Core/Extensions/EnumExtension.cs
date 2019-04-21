using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>枚举扩展</summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 将枚举转换成字典
        /// </summary>
        /// <param name="en">枚举</param>
        /// <returns></returns>
        public static Dictionary<string, int> YusToDict(this Enum en)
        {
            var dict = new Dictionary<string, int>();
            var enType = en.GetType();
            var values = Enum.GetValues(enType);
            for (int i = 0; i < values.Length; i++)
            {
                var v = values.GetValue(i);
                dict.Add(v.ToString(), (int)v);
            }

            return dict;
        }
    }
}
