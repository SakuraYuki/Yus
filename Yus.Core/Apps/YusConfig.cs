using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yus.Apps
{
    /// <summary>Yus框架相关配置</summary>
    public class YusConfig : Config.YusBaseConfig
    {
        private const string App = nameof(App), Run = nameof(Run);

        /// <summary>单例实例</summary>
        protected static YusConfig _Instance;
        /// <summary>获取一个配置文件实例</summary>
        public static YusConfig Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (locker)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new YusConfig();
                        }
                    }
                }
                return _Instance;
            }
        }

        /// <summary>配置文件全路径</summary>
        public override string ConfigFileName => Yus.Apps.YusApps.AppPath + "Yus.cfg";

        /// <summary>设置配置文件初始化的内容</summary>
        protected override Action<SharpConfig.Configuration> SetConfig => cfg =>
        {
            Cfg[App][nameof(LogDir)].StringValue = "Logs";
            Cfg[App][nameof(ExportDir)].StringValue = "Export";
            Cfg[App][nameof(TempDir)].StringValue = "Temp";
            Cfg[App][nameof(DataDir)].StringValue = "Data";
#if DEBUG
            Cfg[Run][nameof(Debug)].BoolValue = true;
#else
            Cfg[Run][nameof(Debug)].BoolValue = false;
#endif
        };

        /// <summary>配置构造器</summary>
        public YusConfig() : base() { }

        /// <summary>日志目录</summary>
        public string LogDir
        {
            get { return System.IO.Path.Combine(Yus.Apps.YusApps.AppPath, Cfg[App][nameof(LogDir)].StringValue); }
            set
            {
                Cfg[App][nameof(LogDir)].StringValue = value;
                ValueSave();
            }
        }

        /// <summary>导出目录</summary>
        public string ExportDir
        {
            get { return System.IO.Path.Combine(Yus.Apps.YusApps.AppPath, Cfg[App][nameof(ExportDir)].StringValue); }
            set
            {
                Cfg[App][nameof(ExportDir)].StringValue = value;
                ValueSave();
            }
        }

        /// <summary>临时文件目录</summary>
        public string TempDir
        {
            get { return System.IO.Path.Combine(Yus.Apps.YusApps.AppPath, Cfg[App][nameof(TempDir)].StringValue); }
            set
            {
                Cfg[App][nameof(TempDir)].StringValue = value;
                ValueSave();
            }
        }

        /// <summary>数据目录</summary>
        public string DataDir
        {
            get { return System.IO.Path.Combine(Yus.Apps.YusApps.AppPath, Cfg[App][nameof(DataDir)].StringValue); }
            set
            {
                Cfg[App][nameof(DataDir)].StringValue = value;
                ValueSave();
            }
        }

        /// <summary>是否进行Debug</summary>
        public bool Debug
        {
            get { return Cfg[Run][nameof(Debug)].BoolValue; }
            set
            {
                Cfg[Run][nameof(Debug)].BoolValue = value;
                ValueSave();
            }
        }
    }
}
