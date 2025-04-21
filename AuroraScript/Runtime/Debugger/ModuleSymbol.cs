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
        public String Name { get; set; }

        /// <summary>
        /// 模块文件路径
        /// </summary>
        public String FilePath { get; set; }


        public FunctionSymbol ResolveFunction(Int32 address)
        {
            foreach (var item in Childrens)
            {
                if (address >= item.StartPoint && address <= item.EndPoint) return item as FunctionSymbol;
            }
            return null;
        }




        /// <summary>
        /// 返回模块符号的字符串表示
        /// </summary>
        /// <returns>格式化的模块信息字符串</returns>
        public override string ToString()
        {
            return $"Module: {Name}@{FilePath} (Start: {StartPoint}, End: {EndPoint})";
        }
    }
}
