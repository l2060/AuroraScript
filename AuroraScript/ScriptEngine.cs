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
            //最终由link-module 链接起来
            // 这个地方不应该由这里加载引入模块，而是由其他线程加载。
            var root = buildModule(filename);
            var codeGenerator = new ByteCodeGenerator();
            root.Accept(codeGenerator);
        }


        private ModuleDeclaration buildModule(String filename, string relativePath = null)
        {
            var compiler = new ScriptCompiler();
            ModuleDeclaration root = compiler.buildAst(filename, relativePath);
            foreach (var dependency in root.Imports)
            {
                var moduleAst = buildModule(dependency.File.Value, root.Directory);
                root.Dependencys.Add(moduleAst);
            }
            return root;
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