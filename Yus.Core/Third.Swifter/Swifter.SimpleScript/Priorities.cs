namespace Swifter.SimpleScript
{
    /// <summary>优先级枚举</summary>
    public enum Priorities : int
    {
        /// <summary>常规</summary>
        Bracket = 100,

        /// <summary>常量</summary>
        Constant = 500,

        /// <summary>获取字段</summary>
        GetField = 600,

        /// <summary>设置字段</summary>
        SetField = 800,

        /// <summary>调用方法</summary>
        InvokeMethod = 1000,

        /// <summary>一元运算符</summary>
        UnaryOperator = 3000,

        /// <summary>二元运算符High</summary>
        BinaryOperatorHigh = 4000,

        /// <summary>二元运算符Medium</summary>
        BinaryOperatorMedium = 5000,

        /// <summary>二元运算符Low</summary>
        BinaryOperatorLow = 6000,

        /// <summary>对比操作符</summary>
        CompareOperator = 7000,

        /// <summary>比较操作符</summary>
        EqualsOperator = 8000,

        /// <summary>按位与操作符</summary>
        ByBitAndOperator = 9010,

        /// <summary>按位取反操作符</summary>
        ByBitNonOperator = 9020,

        /// <summary>按位或操作符</summary>
        ByBitOrOperator = 9030,

        /// <summary>与操作符</summary>
        AndOperator = 9110,

        /// <summary>或操作符</summary>
        OrOperator = 9120,

        /// <summary>三元运算符</summary>
        ThreeMeshOperator = 9200,

        /// <summary>分配值操作符</summary>
        AssignValueOperator = 10000,

        /// <summary>值定义</summary>
        DefindVar = 20000,

        /// <summary>参数分离</summary>
        ParameterSeparator = 500000,

        /// <summary>未设置</summary>
        None = 999999999
    }
}