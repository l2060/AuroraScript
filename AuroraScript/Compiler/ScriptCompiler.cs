using AuroraScript.Analyzer;
using AuroraScript.Ast;
using AuroraScript.Compiler.Emits;
using AuroraScript.Compiler.Exceptions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Compiler
{
    public class ModuleResolver
    {

    }

    public class ModuleSyntaxRef
    {
        public String ModulePath { get; set; }

        public ModuleDeclaration SyntaxTree { get; set; }
    }



    public class ScriptCompiler
    {
        private readonly String _baseDirectory;
        public string FileExtension { get; set; } = ".as";

        private ConcurrentDictionary<string, ModuleSyntaxRef> scriptModules = new ConcurrentDictionary<string, ModuleSyntaxRef>();

        private ByteCodeGenerator codeGenerator;
        public ScriptCompiler(String baseDirectory, ByteCodeGenerator codeGenerator)
        {

            _baseDirectory = Path.GetFullPath(baseDirectory);
            this.codeGenerator = codeGenerator;
        }

        public async Task Build()
        {
            var files = Directory.GetFiles(_baseDirectory, "*.as", SearchOption.AllDirectories);
            ModuleSyntaxRef[] syntaxRefs = new ModuleSyntaxRef[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                syntaxRefs[i] = await BuildSyntaxTreeAsync(files[i]);
            }
            codeGenerator.Visit(syntaxRefs);
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
                throw new AuroraCompilerException(fileFullPath, "Import file path not found ");
            }
            var module = scriptModules.GetOrAdd(fileFullPath, (key) =>
            {
                //var moduleName = key.Replace(_baseDirectory, "").Replace("\\", "/");
                return new ModuleSyntaxRef()
                {
                    ModulePath = fileFullPath
                };
            });
            if (module.SyntaxTree != null) return module;
            var lexer = new AuroraLexer(fileFullPath, Encoding.UTF8);
            var parser = new AuroraParser(this, lexer);
            module.SyntaxTree = parser.Parse();
            if (module.SyntaxTree.MetaInfos.TryGetValue("module", out var value))
            {
                module.SyntaxTree.ModuleName = value.ToString();
            }
            scriptModules.TryAdd(fileFullPath, module);
            // load dependencys 
            foreach (var dependency in module.SyntaxTree.Imports)
            {
                var moduleAst = await BuildSyntaxTreeAsync(dependency.FullPath);
                dependency.ModuleName = moduleAst.SyntaxTree.ModuleName;
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