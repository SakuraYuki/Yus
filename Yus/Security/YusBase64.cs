using System;
using System.Text;

namespace Yus.Security
{
    /// <summary>Base64相关模块</summary>
    public static class YusBase64
    {
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="Message">要加密的内容</param>
        /// <returns></returns>
        public static string Base64Encode(string Message)
        {
            byte[] bytes = Encoding.Default.GetBytes(Message);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="Message">要解密的内容</param>
        /// <returns></returns>
        public static string Base64Decode(string Message)
        {
            byte[] bytes = Convert.FromBase64String(Message);
            return Encoding.Default.GetString(bytes);
        }
    }
}
