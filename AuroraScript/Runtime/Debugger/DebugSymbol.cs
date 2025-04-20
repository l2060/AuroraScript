using System;
using System.Collections.Generic;


namespace AuroraScript.Runtime.Debugger
{
    public abstract class DebugSymbol
    {
        
        /// <summary>
        ///  开始指令地址
        /// </summary>        
        public Int32 StartPoint;

        /// <summary>
        /// 结束指令 地址
        /// </summary>
        public Int32 EndPoint;

        /// <summary>
        /// 源代码位置
        /// </summary>
        public readonly SourceLocation Location;


        /// <summary>
        /// 父级符号对象
        /// </summary>
        public DebugSymbol Parent;


        /// <summary>
        /// 子级符号 对象
        /// </summary>
        public List<DebugSymbol> Childrens;

    }
}
