using System;

namespace Yus.Config
{
    /// <summary>基础配置工具，基于SharpConfig实现</summary>
    public abstract class YusBaseConfig
    {
        /// <summary>单例锁</summary>
        protected static object locker = new object();

        /// <summary>配置文件全路径</summary>
        public abstract string ConfigFileName { get; }

        /// <summary>设置配置文件初始化的内容</summary>
        protected abstract Action<SharpConfig.Configuration> SetConfig { get; }

        /// <summary>是否正在编辑配置文件</summary>
        public bool Editing { get; private set; } = false;

        /// <summary>配置文件控制器</summary>
        public SharpConfig.Configuration Cfg { get; set; }

        /// <summary>创建一个默认配置，如果配置文件不存在就生成一个</summary>
        public YusBaseConfig()
        {
            Reload();
        }

        /// <summary>开始设置配置文件值</summary>
        public void BeginSet()
        {
            Editing = true;
        }

        /// <summary>结束设置配置文件值</summary>
        public void EndSet()
        {
            Editing = false;
            Save();
        }

        /// <summary>重新加载配置文件，如果配置文件不存在就生成一个</summary>
        public void Reload()
        {
            if (!System.IO.File.Exists(ConfigFileName))
            {
                Cfg = new SharpConfig.Configuration();
                SetConfig?.Invoke(Cfg);
                Save();
                return;
            }
            Cfg = SharpConfig.Configuration.LoadFromFile(ConfigFileName);
        }

        /// <summary>保存已设置的配置值，如果正在<see cref="Editing"/>为true则不保存</summary>
        protected void ValueSave()
        {
            if (!Editing) Save();
        }

        /// <summary>保存已设置的配置值到配置文件中</summary>
        public virtual void Save()
        {
            Cfg.SaveToFile(ConfigFileName);
        }

        /// <summary>
        /// 获取设置，如果出现问题就重置默认值
        /// </summary>
        /// <param name="section">所属部分名</param>
        /// <param name="key">参数键</param>
        /// <returns></returns>
        protected SharpConfig.Setting GetSetting(string section, string key)
        {
            try
            {
                if (!Cfg.Contains(section) || !Cfg[section].Contains(key))
                {
                    SetConfig?.Invoke(Cfg);
                    Save();
                }
                return Cfg[section][key];
            }
            catch (SharpConfig.SettingValueCastException ex)
            {
                Console.WriteLine("字符类型无法转换: " + ex);
                SetConfig?.Invoke(Cfg);
                Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                SetConfig?.Invoke(Cfg);
                Save();
            }
            return Cfg[section][key];
        }
    }
}
