using AuroraScript.Runtime.Base;
using System.Buffers;

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
        public Int32 Pointer { get; set; }

        /// <summary>
        /// 局部变量数组
        /// </summary>
        public ScriptObject[] Locals { get; }

        /// <summary>
        /// 调用参数
        /// </summary>
        public readonly ScriptObject[] Arguments;

        /// <summary>
        /// 当前模块对象
        /// </summary>
        public ScriptObject ThisModule;

        /// <summary>
        /// 闭包捕获的环境
        /// </summary>
        public CallFrame Environment { get; set; }


        /// <summary>
        /// 创建一个新的调用帧
        /// </summary>
        /// <param name="bytecode">要执行的字节码</param>
        /// <param name="ip">初始指令指针位置</param>
        /// <param name="environment">执行环境</param>
        public CallFrame(CallFrame environment, ScriptObject thisModule, Int32 entryPointer, params ScriptObject[] arguments)
        {
            Environment = environment;
            Pointer = entryPointer;
            ThisModule = thisModule;
            Arguments = arguments;
            Locals = ArrayPool<ScriptObject>.Shared.Rent(16);
        }


        ~CallFrame()
        {
            ArrayPool<ScriptObject>.Shared.Return(Locals);
        }

    }



}
