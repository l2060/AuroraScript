using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime
{
    internal class ExecuteContext
    {
        // 操作数栈，用于存储执行过程中的临时值
        public readonly Stack<ScriptObject> _operandStack;

        // 调用栈，用于管理函数调用
        public readonly Stack<CallFrame> _callStack;

        public CallFrame CurrentFrame;

        public readonly ScriptGlobal Global;

        public ExecuteContext(ScriptGlobal global)
        {
            _operandStack = new Stack<ScriptObject>();
            _callStack = new Stack<CallFrame>();
            Global = global;
        }







        public void Reset()
        {
            _operandStack.Clear();
            _callStack.Clear();
        }
    }
}
