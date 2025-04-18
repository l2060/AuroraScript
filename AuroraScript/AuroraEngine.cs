using AuroraScript.Compiler;
using AuroraScript.Compiler.Emits;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// 字符串常量池，存储脚本中使用的所有字符串常量
        /// </summary>
        public readonly StringList _stringSet;

        /// <summary>
        /// 指令构建器，用于生成字节码指令
        /// </summary>
        private readonly InstructionBuilder _instructionBuilder;

        /// <summary>
        /// 脚本编译器，负责解析脚本文件并生成语法树
        /// </summary>
        private readonly ScriptCompiler compiler;

        /// <summary>
        /// 字节码生成器，将语法树转换为字节码
        /// </summary>
        private readonly ByteCodeGenerator codeGenerator;

        /// <summary>
        /// 运行时虚拟机，执行编译后的字节码
        /// </summary>
        private RuntimeVM runtimeVM;

        /// <summary>
        /// 全局对象，存储全局变量和函数
        /// </summary>
        private readonly ScriptGlobal Global;




        /// <summary>
        /// 初始化AuroraScript引擎
        /// </summary>
        /// <param name="options">引擎配置选项，包含脚本基础目录等信息</param>
        public AuroraEngine(EngineOptions options)
        {
            // 初始化全局对象
            Global = new ScriptGlobal();
            // 初始化字符串常量池
            _stringSet = new StringList();
            // 初始化指令构建器
            _instructionBuilder = new InstructionBuilder(_stringSet);
            // 初始化字节码生成器
            codeGenerator = new ByteCodeGenerator(_instructionBuilder, _stringSet);
            // 初始化脚本编译器，设置基础目录和字节码生成器
            compiler = new ScriptCompiler(options.BaseDirectory, codeGenerator);

            // 创建控制台对象，用于日志输出
            var console = new ScriptObject();

            // 在全局对象中注册控制台和调试函数
            Global.SetPropertyValue("console", console);
            Global.SetPropertyValue("debug", new ClrFunction(LOG));

            Global.SetPropertyValue("Array", new ClrFunction(ScriptArray.CONSTRUCTOR));
            Global.SetPropertyValue("String", StringConstructor.INSTANCE);

            // 在控制台对象中注册日志、计时和计时结束函数
            console.SetPropertyValue("log", new ClrFunction(LOG));
            console.SetPropertyValue("time", new ClrFunction(TIME));
            console.SetPropertyValue("timeEnd", new ClrFunction(TIMEEND));
        }


        /// <summary>
        /// 日志输出函数，在脚本中通过console.log或debug调用
        /// </summary>
        /// <param name="domain">脚本域</param>
        /// <param name="thisObject">调用对象（this）</param>
        /// <param name="args">参数数组</param>
        /// <returns>空对象</returns>
        public static ScriptObject LOG(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            Console.WriteLine(String.Join(", ", args));
            return ScriptObject.Null;
        }

        /// <summary>
        /// 用于计时功能的秒表对象
        /// </summary>
        private static Stopwatch _stopwatch = Stopwatch.StartNew();

        /// <summary>
        /// 存储计时标记的字典，键为计时标记名称，值为开始时间
        /// </summary>
        private static Dictionary<String, Int64> _times = new();


        /// <summary>
        /// 开始计时函数，在脚本中通过console.time调用
        /// </summary>
        /// <param name="domain">脚本域</param>
        /// <param name="thisObject">调用对象（this）</param>
        /// <param name="args">参数数组，第一个参数为计时标记名称</param>
        /// <returns>空对象</returns>
        public static ScriptObject TIME(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length == 1)
            {
                // 记录当前时间作为计时开始点
                _times[args[0].ToString()] = _stopwatch.ElapsedMilliseconds;
            }
            return ScriptObject.Null;
        }

        /// <summary>
        /// 结束计时函数，在脚本中通过console.timeEnd调用
        /// 计算并输出从开始计时到结束计时的时间间隔
        /// </summary>
        /// <param name="domain">脚本域</param>
        /// <param name="thisObject">调用对象（this）</param>
        /// <param name="args">参数数组，第一个参数为计时标记名称</param>
        /// <returns>空对象</returns>
        public static ScriptObject TIMEEND(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length == 1 && _times.TryGetValue(args[0].ToString(), out var value))
            {
                // 计算时间间隔
                var time = _stopwatch.ElapsedMilliseconds - value;
                // 移除计时标记
                _times.Remove(args[0].ToString());
                // 输出计时结果

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{args[0]} Used {time}ms");
                Console.ResetColor();

            }
            return ScriptObject.Null;
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
        /// 编译并构建脚本
        /// </summary>
        /// <param name="filename">脚本文件名，可以是相对路径或绝对路径</param>
        /// <returns>异步任务</returns>
        public async Task BuildAsync(String filename)
        {
            // 编译脚本文件
            await compiler.Build(filename);
            // 获取字符串常量池
            var stringConstants = codeGenerator._stringSet.List;
            // 生成字节码
            var bytes = codeGenerator.Build();
            // 创建运行时虚拟机
            runtimeVM = new RuntimeVM(bytes, stringConstants);
            // 输出字节码（调试用）
            codeGenerator.DumpCode();
            return;
        }




        /// <summary>
        /// 创建独立的脚本域环境
        /// 每个脚本域拥有独立的全局对象，但共享原型链
        /// </summary>
        /// <returns>新创建的脚本域对象</returns>
        public ScriptDomain CreateDomain()
        {
            // 创建域全局对象，继承自引擎全局对象
            var domainGlobal = new ScriptGlobal() { _prototype = Global };
            // 创建执行上下文
            ExecuteContext exeContext = new ExecuteContext(domainGlobal, runtimeVM, new ExecuteOptions(10, 0, false));
            // 创建初始调用帧并压入调用栈
            exeContext._callStack.Push(new CallFrame(null, domainGlobal, null, 0));
            // 执行初始化代码
            runtimeVM.Execute(exeContext);
            // 返回新创建的脚本域
            return new ScriptDomain(this, runtimeVM, domainGlobal);
        }


    }
}