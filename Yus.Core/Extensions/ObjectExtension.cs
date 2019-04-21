using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Yus.Diagnostics;
using Yus.Serialization;

namespace System
{

    /// <summary>
    /// 适用于所有对象的扩展方法
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 判定对象不为 null
        /// </summary>
        /// <returns></returns>
        public static bool YusNotNull(this object obj) { return obj != null; }

        /// <summary>
        /// 判定对象为 null
        /// </summary>
        /// <returns></returns>
        public static bool YusIsNull(this object obj) { return obj == null; }

        /// <summary>
        /// 转换为 JSON
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="indented">是否格式化</param>
        /// <param name="sets">序列化设置</param>
        /// <returns></returns>
        public static string YusToJson(this object obj, bool indented = false, Swifter.Json.JsonFormatterOptions sets = Swifter.Json.JsonFormatterOptions.Default)
        {
            return obj.YusIsNull() ? null : YusJson.ToJson(obj, indented: indented, sets: sets);
        }

        /// <summary>
        /// 强制转换为指定对象 , 简化写法, 出错不管
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="obj">要转换的对象</param>
        /// <returns></returns>
        public static T YusTo<T>(this object obj) { return (T)obj; }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="p">要获取的对象</param>
        /// <param name="key">属性名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T YusProp<T>(this object p, string key, T defaultValue = default(T))
        {
            if (p == null) return defaultValue;
            var type = p.GetType();

            var prop = type.GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
            if (prop == null) return defaultValue;

            var value = prop.GetValue(key, null);
            if (value == null) return defaultValue;

            try { return value.YusToJson().YusToObject<T>(); }
            catch (Exception e)
            {
                YusDebug.ConsoleLog(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取字符串属性
        /// </summary>
        /// <param name="p">要获取的对象</param>
        /// <param name="key">属性值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string YusPropString(this object p, string key, string defaultValue = null)
        {
            if (p == null) return defaultValue;
            var type = p.GetType();

            var prop = type.GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
            if (prop == null) return defaultValue;

            var value = prop.GetValue(key, null);
            if (value == null) return defaultValue;

            var valEncStr = value as string;
            return valEncStr ?? defaultValue;
        }

        /// <summary>
        /// 转换到 <see cref="byte"/> 值
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <returns></returns>
        public static byte YusToByte(this object obj)
        {
            return Convert.ToByte(obj);
        }

        /// <summary>
        /// 转换到 <see cref="int"/> 值
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <returns></returns>
        public static int YusToInt32(this object obj)
        {
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 转换到 <see cref="long"/> 值
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <returns></returns>
        public static long YusToInt64(this object obj)
        {
            return Convert.ToInt64(obj);
        }

        /// <summary>
        /// 转换到 <see cref="float"/> 值
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <returns></returns>
        public static float YusToSingle(this object obj)
        {
            return Convert.ToSingle(obj);
        }

        /// <summary>
        /// 转换到 <see cref="double"/> 值
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <returns></returns>
        public static double YusToDouble(this object obj)
        {
            return Convert.ToDouble(obj);
        }

        /// <summary>
        /// 转换到 <see cref="decimal"/> 值
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <returns></returns>
        public static decimal YusToDecimal(this object obj)
        {
            return Convert.ToDecimal(obj);
        }

        /// <summary>
        /// 转换到 <see cref="bool"/> 值
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <returns></returns>
        public static bool YusToBool(this object obj)
        {
            return Convert.ToBoolean(obj);
        }

        /// <summary>
        /// 将 <see cref="int"/> 转换到 <see cref="byte"/>
        /// </summary>
        /// <param name="value"><see cref="int"/> 值</param>
        /// <returns></returns>
        public static byte YusToByte(this int value)
        {
            return Convert.ToByte(value);
        }
    }
}
