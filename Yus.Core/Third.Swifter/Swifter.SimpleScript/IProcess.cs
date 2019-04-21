using Swifter.SimpleScript.Value;
using System.Runtime.CompilerServices;

namespace Swifter.SimpleScript
{
    /// <summary>处理接口</summary>
    public interface IProcess
    {
        /// <summary>
        /// 执行处理
        /// </summary>
        /// <param name="runtime">运行时上下文</param>
        /// <returns></returns>
        [MethodImpl(VersionDifferences.AggressiveInlining)]
        IValue Execute(RuntimeContext runtime);
    }
}
