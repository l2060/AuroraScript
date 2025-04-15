using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 表示函数调用的栈帧，包含执行状态和环境
    /// </summary>
    internal class CallFrame
    {
        /// <summary>
        /// 指令指针，指向当前执行的指令
        /// </summary>
        public int Pointer { get; set; }

        /// <summary>
        /// 局部变量环境
        /// </summary>
        public Environment Environment { get; }

        /// <summary>
        /// 局部变量数组
        /// </summary>
        public ScriptObject[] Locals { get; }


        public ScriptObject[] Arguments;


        public ScriptObject ThisModule;
        /// <summary>
        /// 创建一个新的调用帧
        /// </summary>
        /// <param name="bytecode">要执行的字节码</param>
        /// <param name="ip">初始指令指针位置</param>
        /// <param name="environment">执行环境</param>
        /// <param name="localCount">局部变量数量</param>
        public CallFrame(int ip, Environment environment ,ScriptObject thisModule, int localCount = 160)
        {
            Pointer = ip;
            ThisModule = thisModule;
            Environment = environment;
            Locals = new ScriptObject[localCount];
        }




    }
}
