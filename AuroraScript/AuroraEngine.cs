using AuroraScript.Compiler;
using AuroraScript.Compiler.Emits;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;

namespace AuroraScript
{
    public class AuroraEngine
    {
        private readonly ScriptCompiler compiler;
        private ByteCodeGenerator codeGenerator;
        private RuntimeVM runtimeVM;


        public AuroraEngine(EngineOptions options)
        {
            codeGenerator = new ByteCodeGenerator();
            compiler = new ScriptCompiler(options.BaseDirectory, codeGenerator);
            runtimeVM = new RuntimeVM();
        }

        /// <summary>
        /// 获取全局对象
        /// </summary>
        /// <param name="name">全局对象名称</param>
        /// <returns>全局对象</returns>
        public ScriptObject GetGlobal(string name)
        {
            // 这里需要实现从运行时环境中获取全局对象的逻辑
            return null;
        }

        /// <summary>
        /// 设置全局对象
        /// </summary>
        /// <param name="name">全局对象名称</param>
        /// <param name="value">全局对象值</param>
        public void SetGlobal(string name, ScriptObject value)
        {
            // 这里需要实现设置全局对象的逻辑
        }





        /// <summary>
        /// 编译并构建脚本
        /// </summary>
        /// <param name="filename">脚本文件名</param>
        public async Task BuildAsync(String filename)
        {
            await compiler.Build(filename);

            codeGenerator.DumpCode();
            var bytes = codeGenerator.Build();
            var stringConstants = codeGenerator._stringSet.List;

            var vm = new RuntimeVM(bytes, stringConstants);
            return;
        }

        /// <summary>
        /// 编译并执行脚本
        /// </summary>
        /// <param name="filename">脚本文件名</param>
        /// <returns>执行结果</returns>
        public async Task<object> ExecuteAsync(String filename)
        {
            await compiler.Build(filename);

            var bytes = codeGenerator.Build();
            var stringConstants = codeGenerator._stringSet.List;

            var vm = new RuntimeVM(bytes, stringConstants);
            return vm.Execute();
        }






    }
}