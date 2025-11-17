namespace AuroraScript.Core
{
    public enum ValueKind : byte
    {
        Null = 0,
        Boolean = 1,
        Number = 2,
        String = 3,
        Object = 4,
        Array = 5,

        /// <summary>
        /// 脚本原生方法
        /// </summary>
        Function = 6,

        /// <summary>
        /// Clr原生方法
        /// </summary>
        ClrFunction = 7,

        /// <summary>
        /// Wrap
        /// </summary>
        WrapFunction = 7



    }
}

