using AuroraScript.Compiler;
using AuroraScript.Compiler.Emits;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Debugger;
using AuroraScript.Runtime.Extensions;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System;
using System.Threading.Tasks;

namespace AuroraScript
{
    /// <summary>
    /// AuroraScript引擎类，作为脚本引擎的主入口点
    /// 负责初始化编译器、运行时环境，并提供脚本执行的接口
    /// </summary>
    public class AuroraEngine
    {

        /// <summary>
        /// 全局对象，存储全局变量和函数
        /// </summary>
        public readonly ScriptGlobal Global = new ScriptGlobal();

        /// <summary>
        /// 字符串常量池，存储脚本中使用的所有字符串常量
        /// </summary>
        public readonly StringList _stringSet = new StringList();

        /// <summary>
        /// CLR 类型注册表，由宿主负责向脚本环境暴露可访问的别名。
        /// </summary>
        public ClrTypeRegistry ClrRegistry { get; } = new ClrTypeRegistry();

        /// <summary>
        /// 运行时虚拟机，执行编译后的字节码
        /// </summary>
        private RuntimeVM runtimeVM;



        private readonly EngineOptions _options;

        /// <summary>
        /// 初始化AuroraScript引擎
        /// </summary>
        /// <param name="options">引擎配置选项，包含脚本基础目录等信息</param>
        public AuroraEngine(EngineOptions options)
        {
            Prototypes.Proload();
            _options = options;
            // 在全局对象中注册构造函数和全局变量
            Global.Define("console", new ConsoleEnvironment(), writeable: false, enumerable: false);
            Global.Define("Array", ArrayConstructor.INSTANCE, writeable: false, enumerable: false);
            Global.Define("String", StringConstructor.INSTANCE, writeable: false, enumerable: false);
            Global.Define("Boolean", BooleanConstructor.INSTANCE, writeable: false, enumerable: false);
            Global.Define("Object", ScriptObjectConstructor.INSTANCE, writeable: false, enumerable: false);
        }



        /// <summary>
        /// 获取全局对象
        /// </summary>
        /// <param name="name">全局对象名称</param>
        /// <returns>全局对象</returns>
        public ScriptObject GetGlobal(string name)
        {
            return Global.GetPropertyValue(name);
        }

        public ClrTypeDescriptor RegisterClrType(string alias, Type type, ClrTypeOptions options = null, bool overwrite = false)
        {
            return ClrRegistry.RegisterType(alias, type, options, overwrite);
        }

        /// <summary>
        /// 设置全局对象
        /// </summary>
        /// <param name="name">全局对象名称</param>
        /// <param name="value">全局对象值</param>
        public void SetGlobal(string name, ScriptObject value)
        {
            Global.SetPropertyValue(name, value);
        }

        /// <summary>
        /// 设置全局对象
        /// </summary>
        /// <param name="name">全局对象名称</param>
        /// <param name="value">全局对象值</param>
        /// <param name="writeable">是否可写</param>
        /// <param name="readable">是否可读</param>
        public void Define(string name, ScriptObject value, bool writeable = true, bool readable = true)
        {
            Global.Define(name, value, writeable, readable);
        }


        /// <summary>
        /// 编译并构建脚本
        /// </summary>
        /// <param name="filename">脚本文件名，可以是相对路径或绝对路径</param>
        /// <returns>异步任务</returns>
        public async Task BuildAsync(String filename)
        {
            
            var debugSymbols = new DebugSymbolInfo();
            // 初始化指令构建器
            var instructionBuilder = new InstructionBuilder(_stringSet);
            // 初始化字节码生成器
            var codeGenerator = new ByteCodeGenerator(instructionBuilder, _stringSet, debugSymbols);
            // 初始化脚本编译器，设置基础目录和字节码生成器
            var compiler = new ScriptCompiler(_options.BaseDirectory, codeGenerator);
            // 编译脚本文件
            await compiler.Build(filename);
            // 获取字符串常量池
            var stringConstants = _stringSet.List;
            // 生成字节码
            var bytes = codeGenerator.Build();
            // 创建运行时虚拟机
            runtimeVM = new RuntimeVM(bytes, stringConstants, debugSymbols, ClrRegistry);
            // 输出字节码（调试用）
            codeGenerator.DumpCode();
            return;
        }


        /// <summary>
        /// 创建Domain的Global环境（可选）
        /// </summary>
        /// <returns></returns>
        public ScriptGlobal NewEnvironment()
        {
            return new ScriptGlobal() { _prototype = Global };
        }



        /// <summary>
        /// 创建独立的脚本域环境
        /// 每个脚本域拥有独立的全局对象，但共享原型链
        /// </summary>
        /// <param name="globalEnv">指定域的Global环境</param>
        /// <returns>新创建的脚本域对象</returns>
        public ScriptDomain CreateDomain(ScriptGlobal domainGlobal = null)
        {
            // 创建域全局对象，继承自引擎全局对象
            if (domainGlobal == null)
            {
                domainGlobal = new ScriptGlobal() { _prototype = Global };
            }
            // 创建执行上下文
            ExecuteContext exeContext = ExecuteContextPool.Rent(domainGlobal, runtimeVM, new ExecuteOptions(10, 0, false));
            // 创建初始调用帧并压入调用栈
            exeContext._callStack.Push(CallFramePool.Rent(domainGlobal, null, 0, Array.Empty<ScriptObject>(), Array.Empty<ClosureUpvalue>()));
            // 执行初始化代码
            runtimeVM.Execute(exeContext);
            exeContext.Dispose();
            // 返回新创建的脚本域
            return new ScriptDomain(this, runtimeVM, domainGlobal);
        }


    }
}