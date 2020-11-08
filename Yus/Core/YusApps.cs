using System;

namespace Yus
{
    /// <summary>程序相关信息</summary>
    public static class YusApps
    {
        /// <summary>
        /// 程序全路径，例如C:/Software/Notepad
        /// </summary>
        public static string AppPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory.YusPathCase();
    }
}
