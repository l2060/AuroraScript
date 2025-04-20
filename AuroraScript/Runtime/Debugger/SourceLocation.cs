﻿using System;

namespace AuroraScript.Runtime.Debugger
{
    /// <summary>
    /// 源代码位置信息，用于调试时定位代码位置
    /// 包含文件路径、行号和列号信息
    /// </summary>
    public struct SourceLocation
    {
        /// <summary>
        /// 源文件的完整路径
        /// </summary>
        public readonly String FilePath;

        /// <summary>
        /// 模块名称
        /// </summary>
        public readonly String ModuleName;

        /// <summary>
        /// 源代码行号（从1开始）
        /// </summary>
        public readonly Int32 LineNumber;

        /// <summary>
        /// 创建一个新的源代码位置信息
        /// </summary>
        /// <param name="filePath">源文件的完整路径</param>
        /// <param name="moduleName">模块名称</param>
        /// <param name="lineNumber">源代码行号</param>
        /// <param name="columnNumber">源代码列号</param>
        public SourceLocation(String filePath, String moduleName, Int32 lineNumber)
        {
            FilePath = filePath;
            ModuleName = moduleName;
            LineNumber = lineNumber;
        }

        /// <summary>
        /// 返回源代码位置的字符串表示
        /// </summary>
        /// <returns>格式化的位置信息字符串</returns>
        public override string ToString()
        {
            return $"{ModuleName} ({FilePath}:{LineNumber})";
        }
    }
}
