using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 表示一个闭包，包含函数字节码和捕获的环境
    /// </summary>
    internal class Closure : Callable
    {
        /// <summary>
        /// 函数的字节码
        /// </summary>
        public Int32 EntryPointer { get; }

        /// <summary>
        /// 函数名称（可选）
        /// </summary>
        public string FuncName { get; }

        /// <summary>
        /// Module对象
        /// </summary>
        public ScriptObject ThisModule { get; set; }

        /// <summary>
        /// 闭包This环境
        /// </summary>
        public CallFrame Environment { get; set; }

        /// <summary>
        /// 创建一个新的闭包
        /// </summary>
        /// <param name="environment">闭包捕获的环境</param>
        /// <param name="thisModule">函数所处模块</param>
        /// <param name="entryPointer">函数代码指针</param>
        /// <param name="funcName">函数名称</param>
        internal Closure(CallFrame environment, ScriptObject thisModule, Int32 entryPointer, string funcName = null)
        {
            ThisModule = thisModule;
            EntryPointer = entryPointer;
            Environment = environment;
            FuncName = funcName;
        }

        /// <summary>
        /// 返回闭包的字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"<function {(string.IsNullOrEmpty(FuncName) ? "anonymous" : FuncName)}>";
        }

        public override ScriptObject Invoke(AuroraEngine engine, ScriptObject module, ScriptObject[] args)
        {
            return null;
        }

        public override BoundFunction Bind(ScriptObject target)
        {
            return null;
        }
    }
}
