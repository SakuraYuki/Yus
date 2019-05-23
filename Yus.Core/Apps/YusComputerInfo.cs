using Microsoft.Win32;
using System;
using System.Management;
using System.Net.NetworkInformation;

namespace Yus.Apps
{
    /// <summary>电脑信息获取（依赖系统服务，使用<see cref="ManagementClass"/>类）</summary>
    public class YusComputerInfo
    {
        /// <returns>获取电脑信息串联的字符串数据，分别连接CPU、主板、BIOS和MAC数据</returns>
        public static string GetComputerInfo()
        {
            var cpu = GetCpuInfo();
            var baseBoard = GetBaseBoardInfo();
            var bios = GetBiosInfo();
            var mac = GetMacInfo();
            var info = string.Concat(cpu, baseBoard, bios, mac);
            return info;
        }

        /// <summary>
        /// 获取CPU信息
        /// </summary>
        /// <returns></returns>
        public static string GetCpuInfo()
        {
            var info = GetHardWareInfo("Win32_Processor", "ProcessorId");
            return info;
        }

        /// <summary>
        /// 获取CPU信息
        /// </summary>
        /// <returns></returns>
        public static string GetBiosInfo()
        {
            var info = GetHardWareInfo("Win32_BIOS", "SerialNumber");
            return info;
        }

        /// <summary>
        /// 获取主板信息
        /// </summary>
        /// <returns></returns>
        public static string GetBaseBoardInfo()
        {
            var info = GetHardWareInfo("Win32_BaseBoard", "SerialNumber");
            return info;
        }

        /// <summary>
        /// 获取MAC地址信息
        /// </summary>
        /// <returns></returns>
        public static string GetMacInfo()
        {
            var info = string.Empty;
            info = GetHardWareInfo("Win32_BaseBoard", "SerialNumber");
            return info;
        }

        /// <summary>
        /// 获取网卡信息
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddressByNetworkInformation()
        {
            var key = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\";
            var macAddress = string.Empty;
            try
            {
                var nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var adapter in nics)
                {
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                        && adapter.GetPhysicalAddress().ToString().Length != 0)
                    {
                        var fRegistryKey = key + adapter.Id + "\\Connection";
                        var rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                        if (rk != null)
                        {
                            var fPnpInstanceId = rk.GetValue("PnpInstanceID", "").ToString();
                            var fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                            if (fPnpInstanceId.Length > 3 &&
                                fPnpInstanceId.Substring(0, 3) == "PCI")
                            {
                                macAddress = adapter.GetPhysicalAddress().ToString();
                                for (var i = 1; i < 6; i++)
                                {
                                    macAddress = macAddress.Insert(3 * i - 1, ":");
                                }
                                break;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //这里写异常的处理  
            }
            return macAddress;
        }
        
        /// <summary>
        /// 获取指定硬件信息
        /// </summary>
        /// <param name="typePath">类型</param>
        /// <param name="key">数据键值</param>
        /// <returns></returns>
        public static string GetHardWareInfo(string typePath, string key)
        {
            try
            {
                var managementClass = new ManagementClass(typePath);
                var mn = managementClass.GetInstances();
                var properties = managementClass.Properties;
                foreach (var property in properties)
                {
                    if (property.Name != key) continue;
                    foreach (var m in mn)
                    {
                        return m.Properties[property.Name].Value.ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                //这里写异常的处理
                Console.WriteLine(ex);
                throw ex;
            }
            return string.Empty;
        }
    }
}
