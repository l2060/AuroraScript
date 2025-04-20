using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime.Debugger
{
    /// <summary>
    /// 函数符号信息，用于调试时查询函数信息
    /// 包含函数名称、入口点、变量表等信息
    /// </summary>
    public class FunctionSymbol : DebugSymbol
    {
        /// <summary>
        /// 函数名称
        /// </summary>
        public readonly String Name;

        /// <summary>
        /// 函数入口点（字节码地址）
        /// </summary>
        public readonly Int32 EntryPoint;

        /// <summary>
        /// 返回函数符号的字符串表示
        /// </summary>
        /// <returns>格式化的函数信息字符串</returns>
        public override string ToString()
        {
            return $"{Name} (入口点: {EntryPoint}, 结束点: {EndPoint})";
        }
    }
}
