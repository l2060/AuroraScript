﻿using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime.Debugger
{
    /// <summary>
    /// 模块符号信息，用于调试时查询模块信息
    /// 包含模块名称、路径和函数表等信息
    /// </summary>
    public class ModuleSymbol: DebugSymbol
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public readonly String Name;

        /// <summary>
        /// 模块文件路径
        /// </summary>
        public readonly String FilePath;


        /// <summary>
        /// 返回模块符号的字符串表示
        /// </summary>
        /// <returns>格式化的模块信息字符串</returns>
        public override string ToString()
        {
            return $"模块: {Name} ({FilePath})";
        }
    }
}
