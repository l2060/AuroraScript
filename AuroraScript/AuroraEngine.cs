using AuroraScript.Compiler;
using AuroraScript.Compiler.Emits;
using AuroraScript.Runtime;

namespace AuroraScript
{
    public class AuroraEngine
    {
        private readonly ScriptCompiler compiler;
        private ByteCodeGenerator codeGenerator;


        public AuroraEngine(EngineOptions options)
        {
            codeGenerator = new ByteCodeGenerator();
            compiler = new ScriptCompiler(options.BaseDirectory, codeGenerator);


        }





        /// <summary>
        /// </summary>
        /// <param name="filename"></param>
        public async Task BuildAsync(String filename)
        {
            await compiler.Build(filename);

            codeGenerator.DumpCode();
            var bytes = codeGenerator.Build();

            
            var vm = new RuntimeVM();



        }






    }
}