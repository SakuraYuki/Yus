using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Yus.Text
{
    /// <summary>MD5获取</summary>
    public class YusMd5
    {
        /// <summary>
        /// 获取字符串的 MD5 值
        /// </summary>
        /// <param name="text">要获取的字符串</param>
        /// <param name="lowerCase">是否小写输出</param>
        /// <param name="line">是否包含短线</param>
        /// <param name="encoding">编码规则</param>
        /// <returns></returns>
        public static string GetMd5(string text, bool lowerCase = false, bool line = false, Encoding encoding = null)
        {
            if (text == null) throw new ArgumentNullException("text");
            encoding = encoding ?? Encoding.ASCII;

            var hash = BitConverter.ToString(MD5.Create().ComputeHash(encoding.GetBytes(text)));
            hash = line ? hash : hash.Replace("-", "");
            return lowerCase ? hash.ToLower() : hash.ToUpper();
        }
    }
}
