using AuroraScript.Compiler;
using AuroraScript.Compiler.Emits;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AuroraScript
{
    public class AuroraEngine
    {
        public readonly StringList _stringSet;
        private readonly InstructionBuilder _instructionBuilder;
        private readonly ScriptCompiler compiler;
        private readonly ByteCodeGenerator codeGenerator;
        private RuntimeVM runtimeVM;
        private readonly ScriptGlobal Global;




        public AuroraEngine(EngineOptions options)
        {
            Global = new ScriptGlobal();
            _stringSet = new StringList();
            _instructionBuilder = new InstructionBuilder(_stringSet);
            codeGenerator = new ByteCodeGenerator(_instructionBuilder, _stringSet);
            compiler = new ScriptCompiler(options.BaseDirectory, codeGenerator);


            var console = new ScriptObject();

            Global.SetPropertyValue("console", console);
            Global.SetPropertyValue("debug", new ClrFunction(LOG));

            console.SetPropertyValue("log", new ClrFunction(LOG));
            console.SetPropertyValue("time", new ClrFunction(TIME));
            console.SetPropertyValue("timeEnd", new ClrFunction(TIMEEND));

        }


        public static ScriptObject LOG(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            Console.WriteLine(String.Join(", ", args));
            return ScriptObject.Null;
        }

        private static Stopwatch _stopwatch = Stopwatch.StartNew();
        private static Dictionary<String, Int64> _times = new();


        public static ScriptObject TIME(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length == 1)
            {
                _times[args[0].ToString()] = _stopwatch.ElapsedMilliseconds;
            }
            return ScriptObject.Null;
        }

        public static ScriptObject TIMEEND(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length == 1 && _times.TryGetValue(args[0].ToString(), out var value))
            {
                var time = _stopwatch.ElapsedMilliseconds - value;
                _times.Remove(args[0].ToString());
                Console.WriteLine($"Time {args[0]} {time}ms");
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
        /// <param name="filename">脚本文件名</param>
        public async Task BuildAsync(String filename)
        {
            await compiler.Build(filename);
            var stringConstants = codeGenerator._stringSet.List;
            var bytes = codeGenerator.Build();
            runtimeVM = new RuntimeVM(bytes, stringConstants);

            codeGenerator.DumpCode();
            return;
        }




        /// <summary>
        /// 创建独立的脚本域环境
        /// </summary>
        /// <returns></returns>
        public ScriptDomain CreateDomain()
        {
            var domainGlobal = new ScriptGlobal() { _prototype = Global };
            ExecuteContext exeContext = new ExecuteContext(domainGlobal, runtimeVM);
            exeContext._callStack.Push(new CallFrame(null, domainGlobal, null, 0));
            runtimeVM.Execute(exeContext);
            return new ScriptDomain(this, runtimeVM, domainGlobal);
        }


    }
}