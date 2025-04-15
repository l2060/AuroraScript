using AuroraScript.Compiler;
using AuroraScript.Compiler.Emits;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;

namespace AuroraScript
{
    public class AuroraEngine
    {
        public readonly StringList _stringSet;
        private readonly InstructionBuilder _instructionBuilder;
        private readonly ScriptCompiler compiler;
        private readonly ByteCodeGenerator codeGenerator;
        private RuntimeVM runtimeVM;
        private readonly Runtime.Environment Global;
 



        public AuroraEngine(EngineOptions options)
        {
            Global = new Runtime.Environment(null);
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




        public void CreateDomain()
        {

            runtimeVM.Execute( new ExecuteContext());   

        }








    }
}