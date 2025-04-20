﻿using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime.Debugger
{
    /// <summary>
    /// 调试符号信息，用于存储和查询脚本的调试信息
    /// 包含模块、函数、变量和指令映射等信息
    /// </summary>
    public class DebugSymbolInfo
    {
        /// <summary>
        /// 模块符号表，键为模块名称，值为模块符号信息
        /// </summary>
        private readonly Dictionary<String, ModuleSymbol> _modules = new Dictionary<String, ModuleSymbol>();

        /// <summary>
        /// 根据指令地址获取模块符号信息
        /// </summary>
        /// <param name="address">指令地址</param>
        /// <returns>模块符号信息，如果未找到则返回null</returns>
        public ModuleSymbol GetModule(Int32 address)
        {
            return null;
        }

        /// <summary>
        /// 根据模块名称获取模块符号信息
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>模块符号信息，如果未找到则返回null</returns>
        public ModuleSymbol GetModule(String moduleName)
        {
            return null;
        }

        /// <summary>
        /// 获取所有模块符号信息
        /// </summary>
        /// <returns>模块符号信息的集合</returns>
        public IEnumerable<ModuleSymbol> GetModules()
        {
            return _modules.Values;
        }
    }
}
