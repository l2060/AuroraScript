using AuroraScript.Compiler;
using AuroraScript.Compiler.Emits;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using System.Diagnostics;

namespace AuroraScript
{
    public class AuroraEngine
    {
        public readonly StringList _stringSet;
        private readonly InstructionBuilder _instructionBuilder;
        private readonly ScriptCompiler compiler;
        private readonly ByteCodeGenerator codeGenerator;
        private RuntimeVM runtimeVM;
        private readonly ScriptObject Global;




        public AuroraEngine(EngineOptions options)
        {
            Global = new ScriptObject();
            _stringSet = new StringList();
            _instructionBuilder = new InstructionBuilder(_stringSet);
            codeGenerator = new ByteCodeGenerator(_instructionBuilder, _stringSet);
            compiler = new ScriptCompiler(options.BaseDirectory, codeGenerator);
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
            CreateDomain();





            //return vm.Execute(null);
            return;
        }


        public static ScriptObject LOG(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            Console.WriteLine(String.Join(", ", args));
            return ScriptObject.Null;
        }



        public void CreateDomain()
        {


            var global = runtimeVM.Execute(new ExecuteContext());


            var console = new ScriptObject();

            global.SetPropertyValue("console", console);
            console.SetPropertyValue("log", new ClrFunction(LOG));

            var moduleTimer = global.GetPropertyValue("@TIMER");
            var createTimerMethod = moduleTimer.GetPropertyValue("createTimer");
            var testMethod = moduleTimer.GetPropertyValue("test");




            Stopwatch stopwatch = Stopwatch.StartNew();
            runtimeVM.Execute(testMethod as Closure);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);



            if (createTimerMethod is Closure closure)
            {
                var result = runtimeVM.Execute(closure, new StringValue("Hello"), new NumberValue(500));
                var resetFunc = result.GetPropertyValue("reset");
                var cancelFunc = result.GetPropertyValue("cancel");
                var result2 = runtimeVM.Execute(resetFunc as Closure);
                var result3 = runtimeVM.Execute(cancelFunc as Closure);
                Console.WriteLine(result);
            }



        }








    }
}