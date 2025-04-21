﻿using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime.Debugger
{
    /// <summary>
    /// 调试符号信息，用于存储和查询脚本的调试信息
    /// 包含模块、函数、变量和指令映射等信息
    /// </summary>
    public class DebugSymbolInfo: DebugSymbol
    {
        /// <summary>
        /// 根据指令地址获取模块符号信息
        /// </summary>
        /// <param name="address">指令地址</param>
        /// <returns>模块符号信息，如果未找到则返回null</returns>
        public ModuleSymbol ResolveModule(Int32 address)
        {
            foreach (var item in Childrens)
            {
                if (address >= item.StartPoint && address <= item.EndPoint) return item as ModuleSymbol;
            }
            return null;
        }






    }
}
