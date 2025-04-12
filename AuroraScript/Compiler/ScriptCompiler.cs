using AuroraScript.Analyzer;
using AuroraScript.Ast;
using AuroraScript.Compiler.Emits;
using AuroraScript.Compiler.Exceptions;

using AuroraScript.Uilty;
using System.Collections.Concurrent;
using System.Text;

namespace AuroraScript.Compiler
{
    public class ModuleResolver
    {

    }

    public class ModuleSyntaxRef
    {
        public String ModuleName { get; set; }
        public String ModulePath { get; set; }

        public ModuleDeclaration SyntaxTree { get; set; }


    }



    public class ScriptCompiler
    {
        private readonly String _baseDirectory;
        public string FileExtension { get; set; } = ".as";
        private ByteCodeGenerator codeGenerator;
        private ConcurrentDictionary<string, ModuleSyntaxRef> scriptModules = new ConcurrentDictionary<string, ModuleSyntaxRef>();


        public ScriptCompiler(String baseDirectory)
        {
            codeGenerator = new ByteCodeGenerator();
            _baseDirectory = Path.GetFullPath(baseDirectory);
        }

        public async Task Build(string filepath)
        {
            var files = Directory.GetFiles(_baseDirectory, "*.as", SearchOption.AllDirectories);
            ModuleSyntaxRef[] syntaxRefs = new ModuleSyntaxRef[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                syntaxRefs[i] = await BuildSyntaxTreeAsync(files[i]);
            }

            //syntaxRefs = files.Select(item => BuildSyntaxTreeAsync(item).Result).ToArray();

            //using (var s = new WitchTimer("BUILD USE XX :"))
            //{
            //    syntaxRefs = await Task.WhenAll(files.Select(BuildSyntaxTreeAsync));
            //}
            foreach (var item in syntaxRefs)
            {
                item.SyntaxTree.Accept(codeGenerator);
            }
            codeGenerator.DumpCode();
            var bytes = codeGenerator.Build();
        }







        /// <summary>
        /// build Abstract syntax tree
        /// Increase the path cache to prevent the endless loop of circular references
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public async Task<ModuleSyntaxRef> BuildSyntaxTreeAsync(string fileFullPath)
        {
            if (!File.Exists(fileFullPath))
            {
                throw new CompilerException(fileFullPath, "Import file path not found ");
            }
            var module = scriptModules.GetOrAdd(fileFullPath, (key) =>
            {
                var moduleName = key.Replace(_baseDirectory, "").Replace("\\", "/");
                return new ModuleSyntaxRef()
                {
                    ModuleName = moduleName,
                    ModulePath = fileFullPath
                };
            });
            if (module.SyntaxTree != null) return module;
            var lexer = new AuroraLexer(fileFullPath, Encoding.UTF8);
            var parser = new AuroraParser(this, lexer);
            module.SyntaxTree = parser.Parse();
            scriptModules.TryAdd(fileFullPath, module);
            // load dependencys 
            foreach (var dependency in module.SyntaxTree.Imports)
            {
                var moduleAst = await BuildSyntaxTreeAsync(dependency.FullPath);
                module.SyntaxTree.Dependencys.Add(moduleAst);
            }
            return module;
        }


        /// <summary>
        /// optimize abstract syntax tree
        /// </summary>
        /// <param name="root"></param>
        public void opaimizeTree(AstNode parent)
        {
            //for (int i = parent.Length - 1; i >= 0; i--)
            //{
            //    var node = parent[i];
            //    if (node is AssignmentExpression assignmentExpression)
            //    {
            //        Console.WriteLine();
            //    }
            //    opaimizeTree(node);
            //}
        }


    }
}