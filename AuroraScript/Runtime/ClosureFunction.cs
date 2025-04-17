using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 表示一个闭包，包含函数字节码和捕获的环境
    /// </summary>
    public class ClosureFunction : Callable
    {
        /// <summary>
        /// 闭包指向的方法的字节码地址
        /// </summary>
        public readonly Int32 EntryPointer;

        /// <summary>
        /// 函数名称（可选）
        /// </summary>
        public readonly string FuncName;

        /// <summary>
        /// Module Object
        /// </summary>
        public readonly ScriptModule Module;

        /// <summary>
        /// Domain Global
        /// </summary>
        public readonly ScriptGlobal Global;

        /// <summary>
        /// 闭包This环境
        /// </summary>
        public readonly CallFrame Environment;

        /// <summary>
        /// 创建一个新的闭包
        /// </summary>
        /// <param name="environment">闭包捕获的环境</param>
        /// <param name="global">闭包所在Domain的Global</param>
        /// <param name="thisModule">闭包所处模块</param>
        /// <param name="entryPointer">闭包方法指针</param>
        /// <param name="funcName">闭包方法名称</param>
        internal ClosureFunction(CallFrame environment, ScriptGlobal global, ScriptModule thisModule, Int32 entryPointer, string funcName = null)
        {
            Module = thisModule;
            Global = global;
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
