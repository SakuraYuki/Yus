using System;
using Yus.Apps;

namespace Yus.Diagnostics
{
    /// <summary>调试模块</summary>
    public class YusDebug
    {
        /// <summary>
        /// 将信息写入控制台输出（需将<see cref="YusConfig.Debug"/>设置为true）
        /// </summary>
        /// <param name="message">要输出的消息</param>
        /// <param name="args">格式化参数</param>
        public static void ConsoleLog(string message, params object[] args)
        {
            if (YusConfig.Instance.Debug) Console.WriteLine(message, args);
        }

        /// <summary>
        /// 将对象写入控制台输出（需将<see cref="YusConfig.Debug"/>设置为true）
        /// </summary>
        /// <param name="obj">要输出的对象</param>
        public static void ConsoleLog(object obj)
        {
            if (YusConfig.Instance.Debug) Console.WriteLine(obj);
        }

        /// <summary>
        /// 将异常信息写到控制台输出（需将<see cref="YusConfig.Debug"/>设置为true）
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="msg">异常消息说明</param>
        public static void ConsoleLog(Exception ex, string msg = "出现异常")
        {
            if (!YusConfig.Instance.Debug || ex == null) return;
            var innerEx = ex;
            // 获取最内部的异常
            while (innerEx.InnerException != null) innerEx = innerEx.InnerException;
            Console.WriteLine($"{msg}({innerEx.GetType().Name})\n{innerEx.Message}\n{innerEx.StackTrace}");
        }
    }
}
