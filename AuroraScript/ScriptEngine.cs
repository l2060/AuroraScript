using AuroraScript.Ast;
using AuroraScript.Compiler;
using AuroraScript.Core;

namespace AuroraScript
{
    public class ScriptEngine
    {
        /// <summary>
        /// </summary>
        /// <param name="filename"></param>
        public void build(String filename)
        {
            var compiler = new ScriptCompiler();
            ModuleDeclaration root = compiler.buildAst(filename);
            var codeGenerator = new ByteCodeGenerator();
            root.Accept(codeGenerator);

            compiler.PrintGenerateCode(root);
        }



        private unsafe void test()
        {
            NumberUnion nu = new NumberUnion(Int16.MaxValue, 0, 0, 0);
            var bytes = new Byte[] { 255, 127, 0, 0 };
            fixed (Byte* s = bytes)
            {
                NumberUnion* union = (NumberUnion*)s;
                Console.WriteLine(*union);
            }
        }





    }
}