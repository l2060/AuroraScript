﻿using System;

namespace AuroraScript.Runtime.Debugger
{
    /// <summary>
    /// 变量符号信息，用于调试时查询变量信息
    /// 包含变量名称、本地索引和作用域信息
    /// </summary>
    public class VariableSymbol
    {
        /// <summary>
        /// 变量名称
        /// </summary>
        public readonly String Name;

        /// <summary>
        /// 变量在局部变量表中的索引
        /// </summary>
        public readonly Int32 LocalIndex;


        /// <summary>
        /// 创建一个新的变量符号信息
        /// </summary>
        /// <param name="name">变量名称</param>
        /// <param name="localIndex">变量在局部变量表中的索引</param>
        public VariableSymbol(String name, Int32 localIndex)
        {
            Name = name;
            LocalIndex = localIndex;
        }

        /// <summary>
        /// 返回变量符号的字符串表示
        /// </summary>
        /// <returns>格式化的变量信息字符串</returns>
        public override string ToString()
        {
            return $"{Name} ({LocalIndex})";
        }
    }
}
