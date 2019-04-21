﻿using System.Runtime.CompilerServices;

namespace Swifter.Tools
{
    /// <summary>
    /// 储存一个字符串的数字信息
    /// </summary>
    public unsafe sealed class NumberInfo
    {
        /// <summary>
        /// 
        /// </summary>
        internal NumberInfo()
        {
            integerBegin = -1;
            fractionalBegin = -1;
            exponentBegin = -1;
        }

        internal char* chars;
        internal bool isFloat;
        internal bool haveFractional;
        internal bool isNegative;
        internal bool exponentIsNegative;
        internal int integerBegin;
        internal int integerCount;
        internal int fractionalBegin;
        internal int fractionalCount;
        internal int exponentBegin;
        internal int exponentCount;
        internal byte radix;
        internal int end;

        /// <summary>
        /// 获取该数字是否为浮点数。
        /// </summary>
        public bool IsFloat => isFloat;

        /// <summary>
        /// 获取该数字是否为负数。
        /// </summary>
        public bool IsNegative => isNegative;

        /// <summary>
        /// 获取该数字的指数是否为负数。
        /// </summary>
        public bool ExponentIsNegative => exponentIsNegative;

        /// <summary>
        /// 获取该数字的整数部分的长度。
        /// </summary>
        public int IntegerCount => integerCount;

        /// <summary>
        /// 获取该数字的小数部分的长度。
        /// </summary>
        public int FractionalCount => fractionalCount;

        /// <summary>
        /// 获取该数字的指数的长度。
        /// </summary>
        public int ExponentCount => exponentCount;

        /// <summary>
        /// 获取该字符串是否是一个数字。
        /// </summary>
        public bool IsNumber
        {
            [MethodImpl(VersionDifferences.AggressiveInlining)]
            get
            {
                return integerCount > 0 && (!haveFractional || fractionalCount > 0) && (exponentBegin == -1 || exponentCount != 0);
            }
        }

        /// <summary>
        /// 获取是否存在指数。
        /// 此值不考虑是否为数字。
        /// </summary>
        public bool HaveExponent
        {
            [MethodImpl(VersionDifferences.AggressiveInlining)]
            get
            {
                return exponentBegin != -1 && exponentCount != 0;
            }
        }

        /// <summary>
        /// 获取是否为十进制数字。
        /// </summary>
        public bool IsDecimal => IsNumber && radix == 10;

        /// <summary>
        /// 获取此数字在字符串中的结束位置，数字内容不包含此位置。
        /// </summary>
        public int End
        {
            [MethodImpl(VersionDifferences.AggressiveInlining)]
            get
            {
                return end;
            }
        }

        /// <summary>
        /// 获取此数字的进制数。
        /// </summary>
        public byte Radix => radix;

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        private int ToString(char* chars)
        {
            if (IsNumber)
            {
                int index = 0;

                if (isNegative)
                {
                    chars[0] = NumberHelper.NegativeSign;

                    ++index;
                }

                Copy(this.chars, integerBegin, integerCount, chars, index);

                index += integerCount;

                if (haveFractional && fractionalCount != 0)
                {
                    chars[index] = NumberHelper.DotSign;

                    ++index;

                    Copy(this.chars, fractionalBegin, fractionalCount, chars, index);

                    index += fractionalCount;
                }

                if (exponentBegin != -1 && exponentCount != 0)
                {
                    chars[index] = NumberHelper.ExponentSign;

                    ++index;

                    if (exponentIsNegative)
                    {
                        chars[index] = NumberHelper.NegativeSign;

                        ++index;
                    }

                    Copy(this.chars, exponentBegin, exponentCount, chars, index);

                    index += exponentCount;
                }

                return index;
            }
            else
            {
                Copy(NumberHelper.NaNSign, chars, 0);

                return NumberHelper.NaNSign.Length;
            }
        }
        
        private int StringLength
        {
            [MethodImpl(VersionDifferences.AggressiveInlining)]
            get
            {
                if (IsNumber)
                {
                    return (isNegative ? 1 : 0) + integerCount + ((haveFractional && fractionalBegin != -1 && fractionalCount != 0) ? 1 + fractionalCount : 0) + ((exponentBegin != -1 && exponentCount != 0) ? (exponentIsNegative ? 1 : 0) + 1 + exponentCount : 0);
                }

                return NumberHelper.NaNSign.Length;
            }
        }

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        private static void Copy(char* source, int begin, int count, char* destination, int index)
        {
            int end = begin + count;

            for (; begin < end; ++begin, ++index)
            {
                destination[index] = source[begin];
            }
        }

        [MethodImpl(VersionDifferences.AggressiveInlining)]
        private static void Copy(string source, char* destination, int index)
        {
            for (int i = 0; i < source.Length; ++i, ++index)
            {
                destination[index] = source[i];
            }
        }

        /// <summary>
        /// 获取此数字的信息的字符串表现形式。
        /// </summary>
        /// <returns>返回一个 string 值</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public override string ToString()
        {
            if (IsNumber)
            {
                var result = new string('\0', StringLength);

                fixed(char* chars = result)
                {
                    ToString(chars);
                }

                return result;
            }
            else
            {
                return "NaN";
            }
        }

        /// <summary>
        /// 转换为 Double。失败将引发异常。
        /// </summary>
        /// <returns>返回一个 Double</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public double ToDouble()
        {
            return NumberHelper.InstanceByRadix(radix).ToDouble(this);
        }

        /// <summary>
        /// 转换为 Double。失败将引发异常。
        /// </summary>
        /// <returns>返回一个 Double</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public decimal ToDecimal()
        {
            return NumberHelper.ToDecimal(this);
        }

        /// <summary>
        /// 转换为 UInt64。失败将引发异常。
        /// </summary>
        /// <returns>返回一个 Double</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public ulong ToUInt64()
        {
            return NumberHelper.InstanceByRadix(radix).ToUInt64(this);
        }

        /// <summary>
        /// 转换为 Int64。失败将引发异常。
        /// </summary>
        /// <returns>返回一个 Double</returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public long ToInt64()
        {
            return NumberHelper.InstanceByRadix(radix).ToInt64(this);
        }
    }
}