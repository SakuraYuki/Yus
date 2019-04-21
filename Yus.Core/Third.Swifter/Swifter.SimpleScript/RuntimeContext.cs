using Swifter.SimpleScript.Value;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Swifter.SimpleScript
{
    /// <summary>运行时上下文</summary>
    public sealed class RuntimeContext
    {
        readonly Dictionary<string, IValue> fields = new Dictionary<string, IValue>();

        /// <summary>
        /// 设置上下文字段的值
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public IValue GetField(string name)
        {
            if (fields.TryGetValue(name, out var value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// 定义上下文字段
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        public void DefineField(string name, IValue value)
        {
            fields[name] = value;
        }
    }

    sealed class AddFunction : BaseFunction
    {
        public override IValue Invoke(IValue[] parameters)
        {
            return new DoubleConstant(((BaseNumber)parameters[0]).ReadFloat() + ((BaseNumber)parameters[1]).ReadFloat());
        }
    }
}
