using System;
using System.Text;
using Yus.Apps;

namespace Yus.Security
{
    /// <summary>授权检查器</summary>
    public class YusLicenseChecker
    {
        /// <summary>默认过期时间戳</summary>
        public long DefaultExpireStamp => YusDate.DateTimeToUnix(DefaultExpireDateTime);

        /// <summary>默认过期时间</summary>
        public DateTime DefaultExpireDateTime { get; protected set; }

        /// <summary>字符串加密器</summary>
        public YusStringEncrypt StringEncrypter { get; protected set; }

        /// <summary>授权文件默认路径</summary>
        public string DefaultLicenseFilename { get; protected set; }

        private string _RegCode;
        /// <summary>注册码</summary>
        public string RegCode
        {
            get
            {
                if (_RegCode != null) return _RegCode;
                return _RegCode = ComputerRegCode();
            }
        }

        /// <summary>
        /// 初始化授权检查器
        /// </summary>
        /// <param name="encrypt">授权码加密器</param>
        /// <param name="licenseFilename">授权文件读取和保存的全路径</param>
        public YusLicenseChecker(YusStringEncrypt encrypt=null, string licenseFilename=null)
        {
            DefaultExpireDateTime = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);
            StringEncrypter = encrypt ?? new YusStringEncrypt(Encoding.ASCII.GetBytes("YusCoreFramework"), Encoding.ASCII.GetBytes("YUSCOREFRAMEWORK"));
            DefaultLicenseFilename = licenseFilename ?? AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + "/License";
        }

        /// <summary>
        /// 获取计算机特征信息作为注册码
        /// </summary>
        /// <returns></returns>
        public string ComputerRegCode()
        {
            try
            {
                return YusComputerInfo.GetComputerInfo().Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// 授权码是否已过期
        /// </summary>
        /// <param name="license">授权码</param>
        /// <param name="regCode">注册码</param>
        /// <returns></returns>
        public bool IsLicenseExpire(string license = null, string regCode = null)
        {
            try
            {
                regCode = (regCode ?? RegCode).Trim();
                license = license ?? ReadLicense(DefaultLicenseFilename);
                var licInfo = StringEncrypter.Decrypt(license);
                var licInfos = licInfo.Split('_');
                var expireDate = ExpireDateTime(license);
                return DateTime.Now <= expireDate && licInfos[1] == regCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取授权码的过期时间
        /// </summary>
        /// <param name="licence">授权码</param>
        /// <returns></returns>
        public DateTime ExpireDateTime(string licence)
        {
            try
            {
                var licInfo = StringEncrypter.Decrypt(licence);
                var licInfos = licInfo.Split('_');
                return YusDate.UnixToDate(Convert.ToInt64(licInfos[0]));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return default(DateTime);
            }
        }

        /// <summary>
        /// 获取授权码的过期时间戳
        /// </summary>
        /// <param name="licence">授权码</param>
        /// <returns></returns>
        public long ExpireStamp(string licence)
        {
            try
            {
                var licInfo = StringEncrypter.Decrypt(licence);
                var licInfos = licInfo.Split('_');
                return Convert.ToInt64(licInfos[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }

        /// <summary>
        /// 生成授权码
        /// </summary>
        /// <param name="regCode">注册码</param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        public string GenerateLicense(string regCode = null, DateTime expire = default(DateTime))
        {
            try
            {
                var expireDateTime = expire.ToBinary() > 0 ? expire : DefaultExpireDateTime;
                return StringEncrypter.Encrypt(YusDate.DateTimeToUnix(expireDateTime) + "_" + (regCode ?? RegCode).Trim());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// 尝试读取授权文件的授权码
        /// </summary>
        /// <param name="filename">授权文件路径</param>
        /// <returns></returns>
        public string ReadLicense(string filename = null)
        {
            filename = filename ?? DefaultLicenseFilename;
            if (!System.IO.File.Exists(filename)) return null;
            return System.IO.File.ReadAllText(filename, Encoding.UTF8);
        }

        /// <summary>
        /// 尝试保存授权信息到授权文件中
        /// </summary>
        /// <param name="regCode">注册码</param>
        /// <param name="expire">过期时间</param>
        /// <param name="filename">保存授权文件全路径</param>
        /// <returns></returns>
        public string SaveLicense(string regCode = null, DateTime expire = default(DateTime), string filename = null)
        {
            filename = filename ?? DefaultLicenseFilename;
            regCode = regCode ?? RegCode;
            System.IO.File.Delete(filename);
            var licence = GenerateLicense(regCode: regCode, expire: expire);
            System.IO.File.WriteAllText(filename, licence, Encoding.UTF8);
            return licence;
        }
    }
}
