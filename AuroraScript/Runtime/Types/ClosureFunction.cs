using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{
    /// <summary>
    /// 表示一个闭包函数，包含函数字节码和捕获的环境
    /// 闭包函数可以访问其定义时的环境变量，即使这些变量在定义范围外也可访问
    /// 是脚本中函数的运行时表示，用于实现函数的调用和闭包特性
    /// </summary>
    public class ClosureFunction : Callable
    {
        /// <summary>
        /// 闭包指向的方法的字节码地址
        /// 这是函数代码在字节码中的起始位置，执行时会从这里开始读取指令
        /// </summary>
        public readonly int EntryPointer;

        /// <summary>
        /// 函数名称（可选）
        /// 用于调试和错误信息显示，匿名函数为空
        /// </summary>
        public readonly string FuncName;

        /// <summary>
        /// 闭包所属的模块对象
        /// 存储模块级别的变量和函数，作为闭包的上下文环境
        /// </summary>
        public readonly ScriptModule Module;

        /// <summary>
        /// 闭包捕获的环境
        /// 存储闭包创建时的调用帧，用于访问外部变量
        /// 这是实现闭包特性的关键，允许函数访问其定义时的变量
        /// </summary>
        public readonly CallFrame Environment;


        /// <summary>
        /// 创建一个新的闭包函数
        /// </summary>
        /// <param name="environment">闭包捕获的环境，用于访问外部变量</param>
        /// <param name="thisModule">闭包所属的模块对象</param>
        /// <param name="entryPointer">函数入口点的字节码地址</param>
        /// <param name="funcName">函数名称，可选，匿名函数为null</param>
        internal ClosureFunction(CallFrame environment, ScriptModule thisModule, int entryPointer, string funcName = null)
        {
            // 设置模块对象
            Module = thisModule;
            // 设置函数入口点
            EntryPointer = entryPointer;
            // 设置捕获的环境
            Environment = environment;
            // 设置函数名称
            FuncName = funcName;
        }

        /// <summary>
        /// 返回闭包的字符串表示
        /// 用于调试和错误信息显示
        /// </summary>
        /// <returns>闭包函数的字符串表示，包含函数名称或“匿名”</returns>
        public override string ToString()
        {
            // 如果函数名为空，显示为匿名函数，否则显示函数名
            return $"<function {(string.IsNullOrEmpty(FuncName) ? "anonymous" : FuncName)}>";
        }

        /// <summary>
        /// 调用闭包函数
        /// 注意：当前实现中不支持直接调用，需要通过ScriptDomain.Execute方法执行
        /// </summary>
        /// <param name="domain">脚本域</param>
        /// <param name="module">模块对象</param>
        /// <param name="args">函数参数</param>
        /// <returns>始终返回null，因为当前实现不支持直接调用</returns>
        public override ScriptObject Invoke(ScriptDomain domain, ScriptObject module, ScriptObject[] args)
        {
            // 当前实现不支持直接调用，需要通过ScriptDomain.Execute方法执行
            return null;
        }

        /// <summary>
        /// 将闭包函数绑定到目标对象
        /// 注意：当前实现中不支持绑定
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <returns>始终返回null，因为当前实现不支持绑定</returns>
        public override BoundFunction Bind(ScriptObject target)
        {
            // 当前实现不支持绑定
            return null;
        }
    }
}
