using System;

namespace Yus.Text
{
    /// <summary>Guid生成</summary>
    public static class YusGuid
    {
        /// <summary>
        /// 获取一个 Guid，默认大写，无短线连接，32个字符
        /// </summary>
        /// <param name="lowerCase">是否小写输出</param>
        /// <param name="line">是否包含短线</param>
        /// <param name="length">保留长度</param>
        /// <returns></returns>
        public static string GetGuid(bool lowerCase = false, bool line = false, int length = 32)
        {
            var guid = Guid.NewGuid().ToString();
            guid = line ? guid : guid.Replace("-", "");

            if (!line && length < 32 && length > 0) guid = guid.Substring(0, length);

            return lowerCase ? guid.ToLower() : guid.ToUpper();
        }

        /// <summary>
        /// 获取一个 CombGuid , 默认大写，无短线连接, 32个字符
        /// </summary>
        /// <param name="lowerCase">是否小写输出</param>
        /// <param name="line">是否包含短线</param>
        /// <returns></returns>
        public static string GetCombGuid(bool lowerCase = false, bool line = false)
        {
            var guid = YusCombGuid.NewComb(DateTime.Now).ToString(CombGuidFormatStringType.Comb);
            guid = line ? guid : guid.Replace("-", "");
            return lowerCase ? guid.ToLower() : guid.ToUpper();
        }
    }
}
