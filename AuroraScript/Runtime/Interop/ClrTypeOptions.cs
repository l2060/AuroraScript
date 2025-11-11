using System.Reflection;

namespace AuroraScript.Runtime.Interop
{
    /// <summary>
    /// 配置 CLR 类型在脚本中的曝光行为。
    /// 目前仅作为占位，后续可扩展成员过滤、别名策略等。
    /// </summary>
    public sealed class ClrTypeOptions
    {
        /// <summary>
        /// 默认配置：公开所有公共实例与静态成员。
        /// </summary>
        public static readonly ClrTypeOptions Default = new ClrTypeOptions();

        /// <summary>
        /// 成员可见性的绑定标志。默认同时包含公共实例与静态成员。
        /// </summary>
        public BindingFlags Binding { get; init; }

        /// <summary>
        /// 是否允许派生类覆盖时替换现有桩代码。
        /// </summary>
        public bool AllowOverride { get; init; }

        /// <summary>
        /// 构造函数曝光策略。当前简化为是否允许所有公共构造函数。
        /// </summary>
        public bool ExposeConstructors { get; init; }

        public ClrTypeOptions()
        {
            Binding = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            AllowOverride = false;
            ExposeConstructors = true;
        }
    }
}

