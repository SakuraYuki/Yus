using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    /// <summary>反射相关扩展</summary>
    public static class ReflectionExtension
    {
        #region Assembly

        private static ConcurrentDictionary<string, object> _asmCache = new ConcurrentDictionary<string, object>();

        /// <summary>获取自定义属性，带有缓存功能，避免因.Net内部GetCustomAttributes没有缓存而带来的损耗</summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static TAttribute[] GetCustomAttributes<TAttribute>(this Assembly assembly)
        {
            if (assembly == null) return new TAttribute[0];

            var key = string.Format("{0}_{1}", assembly.FullName, typeof(TAttribute).FullName);

            return (TAttribute[])_asmCache.GetOrAdd(key, k =>
            {
                var atts = assembly.GetCustomAttributes(typeof(TAttribute), true) as TAttribute[];
                return atts ?? (new TAttribute[0]);
            });
        }

        /// <summary>获取自定义属性的值。可用于ReflectionOnly加载的程序集</summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult GetCustomAttributeValue<TAttribute, TResult>(this Assembly target) where TAttribute : Attribute
        {
            if (target == null) return default(TResult);

            // CustomAttributeData可能会导致只反射加载，需要屏蔽内部异常
            try
            {
                var list = CustomAttributeData.GetCustomAttributes(target);
                if (list == null || list.Count < 1) return default(TResult);

                foreach (var item in list)
                {
                    if (typeof(TAttribute) != item.Constructor.DeclaringType) continue;

                    var args = item.ConstructorArguments;
                    if (args != null && args.Count > 0) return (TResult)args[0].Value;
                }
            }
            catch { }

            return default(TResult);
        }

        #endregion

        #region Type

        /// <summary>判断某个类型是否可空类型</summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition &&
                Object.ReferenceEquals(type.GetGenericTypeDefinition(), typeof(Nullable<>))) return true;

            return false;
        }

        /// <summary>是否整数</summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInt(this Type type)
        {
            return type == typeof(int)
                || type == typeof(long)
                || type == typeof(short)
                || type == typeof(uint)
                || type == typeof(ulong)
                || type == typeof(ushort)
                || type == typeof(byte)
                || type == typeof(sbyte)
                ;
        }

        /// <summary>获取类型代码</summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeCode GetTypeCode(this Type type) => Type.GetTypeCode(type);

        /// <summary>
        /// 获取枚举的名字和值的对应字典
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<string, int> YusGetEnumKeyValueDict(this Type enumType)
        {
            if (enumType.YusIsNull()) return null;
            return Yus.Reflection.YusEnum.GetEnumKeyValueDict(enumType);
        }

        /// <summary>
        /// 获取枚举的名字和值的对应键值对集合
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static List<KeyValuePair<string, int>> YusGetEnumKeyValueList(this Type enumType)
        {
            if (enumType.YusIsNull()) return null;
            return Yus.Reflection.YusEnum.GetEnumKeyValueList(enumType);
        }

        /// <summary>
        /// 获取枚举的名字和备注的对应字典
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<int, string> YusGetEnumDesc(this Type enumType)
        {
            if (enumType.YusIsNull()) return null;
            return Yus.Reflection.YusEnum.GetEnumDesc(enumType);
        }

        /// <summary>获取类型的友好名称</summary>
        /// <param name="type">指定类型</param>
        /// <param name="isfull">是否全名，包含命名空间</param>
        /// <returns></returns>
        public static string GetName(this Type type, bool isfull) => isfull ? type.FullName : type.Name;

        #endregion

        #region MemberInfo

        /// <summary>
        /// 获取目标对象的成员值
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public static object GetValue(this object target, MemberInfo member)
        {
            // 有可能跟普通的 PropertyInfo.GetValue(Object target) 搞混了
            if (member == null)
            {
                member = target as MemberInfo;
                target = null;
            }

            if (member is PropertyInfo)
                return target.GetValue(member as PropertyInfo);
            else if (member is FieldInfo)
                return target.GetValue(member as FieldInfo);
            else
                throw new ArgumentOutOfRangeException("member");
        }

        /// <summary>
        /// 获取指定类型的自定义属性
        /// </summary>
        /// <param name="element">成员信息</param>
        /// <param name="attrType">要获取的属性类型</param>
        /// <param name="inherit">是否从继承对象中搜索</param>
        /// <returns></returns>
        public static Attribute YusGetCustomAttribute(this MemberInfo element, Type attrType, bool inherit)
        {
            return Attribute.GetCustomAttribute(element, attrType, inherit);
        }

        /// <summary>
        /// 获取指定泛型类型的自定义属性
        /// </summary>
        /// <typeparam name="T">要获取的属性类型</typeparam>
        /// <param name="element">成员信息</param>
        /// <param name="inherit">是否从继承对象中搜索</param>
        /// <returns></returns>
        public static T YusGetCustomAttribute<T>(this MemberInfo element, bool inherit) where T : Attribute
        {
            return (T)element.YusGetCustomAttribute(typeof(T), inherit);
        }

        /// <summary>获取成员绑定的显示名，优先DisplayName，然后Description</summary>
        /// <param name="member">成员对象</param>
        /// <param name="inherit">是否从继承对象中搜索</param>
        /// <returns></returns>
        public static string YusGetDesc(this MemberInfo member, bool inherit = true)
        {
            DescriptionAttribute customAttribute = member.YusGetCustomAttribute<DescriptionAttribute>(inherit);
            if (customAttribute != null && !customAttribute.Description.YusNullOrWhiteSpace())
                return customAttribute.Description;
            return (string)null;
        }

        /// <summary>把一个方法转为泛型委托，便于快速反射调用</summary>
        /// <typeparam name="TFunc"></typeparam>
        /// <param name="method"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TFunc As<TFunc>(this MethodInfo method, object target = null)
        {
            if (method == null) return default(TFunc);

            if (target == null)
                return (TFunc)(object)Delegate.CreateDelegate(typeof(TFunc), method, true);
            else
                return (TFunc)(object)Delegate.CreateDelegate(typeof(TFunc), target, method, true);
        }

        /// <summary>获取成员的类型，字段和属性是它们的类型，方法是返回类型，类型是自身</summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static Type GetMemberType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Constructor:
                    return (member as ConstructorInfo).DeclaringType;
                case MemberTypes.Field:
                    return (member as FieldInfo).FieldType;
                case MemberTypes.Method:
                    return (member as MethodInfo).ReturnType;
                case MemberTypes.Property:
                    return (member as PropertyInfo).PropertyType;
                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                    return member as Type;
                default:
                    return null;
            }
        }

        /// <summary>获取成员绑定的显示名，优先DisplayName，然后Description</summary>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static string GetDisplayName(this MemberInfo member, bool inherit = true)
        {
            var att = member.YusGetCustomAttribute<DisplayNameAttribute>(inherit);
            if (att != null && !att.DisplayName.YusNullOrWhiteSpace()) return att.DisplayName;

            return null;
        }

        /// <summary>获取成员绑定的显示名，优先DisplayName，然后Description</summary>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static string GetDescription(this MemberInfo member, bool inherit = true)
        {
            var att2 = member.YusGetCustomAttribute<DescriptionAttribute>(inherit);
            if (att2 != null && !att2.Description.YusNullOrWhiteSpace()) return att2.Description;

            return null;
        }

        /// <summary>获取自定义属性的值。可用于ReflectionOnly加载的程序集</summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="target">目标对象</param>
        /// <param name="inherit">是否递归</param>
        /// <returns></returns>
        public static TResult GetCustomAttributeValue<TAttribute, TResult>(this MemberInfo target, bool inherit = true) where TAttribute : Attribute
        {
            if (target == null) return default(TResult);

            try
            {
                var list = CustomAttributeData.GetCustomAttributes(target);
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (typeof(TAttribute).FullName != item.Constructor.DeclaringType.FullName) continue;

                        var args = item.ConstructorArguments;
                        if (args != null && args.Count > 0) return (TResult)args[0].Value;
                    }
                }
                if (inherit && target is Type)
                {
                    target = (target as Type).BaseType;
                    if (target != null && target != typeof(object))
                        return GetCustomAttributeValue<TAttribute, TResult>(target, inherit);
                }
            }
            catch
            {
                // 出错以后，如果不是仅反射加载，可以考虑正面来一次
                if (!target.Module.Assembly.ReflectionOnly)
                {
                    //var att = GetCustomAttribute<TAttribute>(target, inherit);
                    var att = target.YusGetCustomAttribute<TAttribute>(inherit);
                    if (att != null)
                    {
                        var pi = typeof(TAttribute).GetProperties().FirstOrDefault(p => p.PropertyType == typeof(TResult));
                        if (pi != null) return (TResult)att.GetValue(pi);
                    }
                }
            }

            return default(TResult);
        }

        #endregion

        #region PropertyInfo

        /// <summary>获取目标对象的属性值</summary>
        /// <param name="target">目标对象</param>
        /// <param name="property">属性</param>
        /// <returns></returns>
        public static object GetValue(this object target, PropertyInfo property) => property.GetValue(target, null);

        #endregion

        #region FieldInfo

        /// <summary>获取目标对象的字段值</summary>
        /// <param name="target">目标对象</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public static object GetValue(this object target, FieldInfo field) => field.GetValue(target);

        #endregion
    }
}
