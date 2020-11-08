using System.Runtime.CompilerServices;

namespace Swifter.SimpleScript
{
    /// <summary>值接口</summary>
    public interface IValue
    {
        /// <summary>值类型</summary>
        ValueTypes Type { get; }
        
        /// <summary>
        /// 字符串化的值
        /// </summary>
        /// <returns></returns>
        string Stringify();
        
        /// <summary>
        /// 值比较
        /// </summary>
        /// <param name="value">要比对的值</param>
        /// <returns></returns>
        bool Equal(IValue value);
    }
}
