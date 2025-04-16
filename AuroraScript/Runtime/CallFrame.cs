using AuroraScript.Runtime.Base;

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
        /// 局部变量数组
        /// </summary>
        public ScriptObject[] Locals { get; }


        public readonly ScriptObject[] Arguments;


        public ScriptObject ThisModule;

        public Closure Closure;



        /// <summary>
        /// 创建一个新的调用帧
        /// </summary>
        /// <param name="bytecode">要执行的字节码</param>
        /// <param name="ip">初始指令指针位置</param>
        /// <param name="environment">执行环境</param>
        public CallFrame(Closure closure, params ScriptObject[] arguments)
        {
            Closure = closure;
            Pointer = closure.EntryPointer;
            ThisModule = closure.ThisModule;
            Arguments = arguments;
            Locals = new ScriptObject[256];
        }




    }
}
