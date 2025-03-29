using AuroraScript.Compiler;

namespace AuroraScript
{
    public class AuroraEngine
    {
        private readonly ScriptCompiler compiler;
        public AuroraEngine(EngineOptions options)
        {
            compiler = new ScriptCompiler(options.BaseDirectory);


        }





        /// <summary>
        /// </summary>
        /// <param name="filename"></param>
        public async Task BuildAsync(String filename)
        {
            await compiler.Build(filename);
        }






    }
}