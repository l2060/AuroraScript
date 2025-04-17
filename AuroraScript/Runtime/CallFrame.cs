using AuroraScript.Runtime.Base;
using System.Buffers;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 表示函数调用的栈帧，包含执行状态和环境
    /// </summary>
    public class CallFrame
    {
        /// <summary>
        /// 指令指针，指向当前执行的指令
        /// </summary>
        public Int32 Pointer;

        /// <summary>
        /// 局部变量数组
        /// </summary>
        public readonly ScriptObject[] Locals;

        /// <summary>
        /// 调用参数
        /// </summary>
        public readonly ScriptObject[] Arguments;

        /// <summary>
        /// 当前模块对象
        /// </summary>
        public readonly ScriptModule Module;

        /// <summary>
        /// 当前模块对象
        /// </summary>
        public readonly ScriptGlobal Global;

        /// <summary>
        /// 闭包捕获的环境
        /// </summary>
        public readonly CallFrame Environment;


        /// <summary>
        /// 创建一个新的调用帧
        /// </summary>
        /// <param name="bytecode">要执行的字节码</param>
        /// <param name="ip">初始指令指针位置</param>
        /// <param name="environment">执行环境</param>
        public CallFrame(CallFrame environment, ScriptGlobal global, ScriptModule thisModule, Int32 entryPointer, params ScriptObject[] arguments)
        {
            Global = global;
            Environment = environment;
            Pointer = entryPointer;
            Module = thisModule;
            Arguments = arguments;
            Locals = ArrayPool<ScriptObject>.Shared.Rent(16);
        }


        public Boolean TryGetArgument(Int32 index, out ScriptObject arg)
        {
            arg = ScriptObject.Null;
            if (index >= Arguments.Length) return false;
            arg = Arguments[index];
            return true;
        }

        public ScriptObject GetArgument(Int32 index)
        {
            if (index >= Arguments.Length) return ScriptObject.Null;
            return Arguments[index];
        }



        ~CallFrame()
        {
            ArrayPool<ScriptObject>.Shared.Return(Locals);
        }

    }



}
