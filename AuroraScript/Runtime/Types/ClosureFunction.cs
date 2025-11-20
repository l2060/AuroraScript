using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using System;

namespace AuroraScript.Runtime.Types
{
    /// <summary>
    /// 表示一个闭包函数，包含函数字节码和捕获的环境
    /// 闭包函数可以访问其定义时的环境变量，即使这些变量在定义范围外也可访问
    /// 是脚本中函数的运行时表示，用于实现函数的调用和闭包特性
    /// </summary>
    public class ClosureFunction : ScriptObject
    {
        private readonly ScriptDomain Domain;

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

        private readonly ClosureUpvalue[] _capturedUpvalues;


        /// <summary>
        /// 创建一个新的闭包函数
        /// </summary>
        /// <param name="thisModule">闭包所属的模块对象</param>
        /// <param name="entryPointer">函数入口点的字节码地址</param>
        /// <param name="capturedUpvalues">捕获的上值集合</param>
        /// <param name="funcName">函数名称，可选，匿名函数为null</param>
        internal ClosureFunction(ScriptDomain domain, ScriptModule thisModule, int entryPointer, ClosureUpvalue[] capturedUpvalues, string funcName = null)
        {
            Domain = domain;
            // 设置模块对象
            Module = thisModule;
            // 设置函数入口点
            EntryPointer = entryPointer;
            // 捕获的上值集合
            _capturedUpvalues = capturedUpvalues ?? System.Array.Empty<ClosureUpvalue>();
            // 设置函数名称
            FuncName = funcName;
        }

        public ExecuteContext Invoke(ExecuteOptions options, params ScriptObject[] args)
        {
            return Domain.Execute(this, options, args);
        }

        public ExecuteContext InvokeFromClr(ExecuteOptions options, params object[] args)
        {
            ScriptObject[] scriptArgs = Array.Empty<ScriptObject>();
            if (args != null && args.Length > 0)
            {
                var registry = Domain?.Engine?.ClrRegistry;
                scriptArgs = ClrValueConverter.ToScriptObjectArray(args, registry);
            }
            return Invoke(options, scriptArgs);
        }

        public ExecuteContext InvokeFromClr(params object[] args)
        {
            return InvokeFromClr(null, args);
        }

        internal ClosureUpvalue[] CapturedUpvalues => _capturedUpvalues;

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
    }
}
