using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yus.Apps
{
    /// <summary>程序相关信息</summary>
    public class YusApps
    {
        /// <summary>
        /// 程序全路径，例如C:/Software/Notepad
        /// </summary>
        public static string AppPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory.YusPathCase();
    }
}
