using System;

namespace AuroraScript.Runtime
{
    [Flags]
    public enum ValueKind : Int16
    {
        Null = 0,
        Boolean = 1 << 0,
        Number = 1 << 1,
        String = 1 << 2,
        Object = 1 << 3,

        Array = Object | (1 << 4),

        Date = Object | (1 << 5),

        Regex = Object | (1 << 6),
        /// <summary>
        /// 脚本原生方法
        /// </summary>
        Function = Object | (1 << 7),

        /// <summary>
        /// Clr原生类型
        /// </summary>
        ClrType = Object | (1 << 8),

        /// <summary>
        /// Clr原生方法
        /// </summary>
        ClrFunction = Object | (1 << 9),

        /// <summary>
        /// 原型对象的Clr粘合函数
        /// </summary>
        ClrBonding = Object | (1 << 10)
    }
}

