using System;
using System.Collections.Generic;


namespace AuroraScript.Runtime.Debugger
{
    public abstract class DebugSymbol
    {

        /// <summary>
        ///  开始指令地址
        /// </summary>        
        public Int32 StartPoint { get; set; }

        /// <summary>
        /// 结束指令 地址
        /// </summary>
        public Int32 EndPoint { get; set; }

        /// <summary>
        /// 源代码位置
        /// </summary>
        public Int32 LineNumber;

        /// <summary>
        /// 父级符号对象
        /// </summary>
        public DebugSymbol Parent;


        /// <summary>
        /// 子级符号 对象
        /// </summary>
        public List<DebugSymbol> Childrens;


        internal void Add(DebugSymbol symbol)
        {
            if (Childrens == null) Childrens = new List<DebugSymbol>();
            Childrens.Add(symbol);
            symbol.Parent = this;
        }


        public DebugSymbol Resolve(Int32 address)
        {
            if(Childrens != null)
            {
                foreach (var item in Childrens)
                {
                    if (address >= item.StartPoint && address <= item.EndPoint)
                    {
                        return item.Resolve(address);
                    }
                }
            }
            return this;
        }


        public T ResolveParent<T>() where T: DebugSymbol
        {
            if (this is T that) return that;
            if (this.Parent != null)
            {
                return this.Parent.ResolveParent<T>();
            }
            return null;
        }


    }
}
