using AuroraScript.Analyzer;
using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Exceptions;
using AuroraScript.Uilty;
using System.Collections.Concurrent;
using System.Text;

namespace AuroraScript
{
    public class AuroraCompiler
    {
        public String FileExtension { get; set; } = ".ts";

        private ConcurrentDictionary<String, AuroraParser> scriptParsers = new ConcurrentDictionary<String, AuroraParser>();


        /// <summary>
        /// Fill in the file extension 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private String fillExtension(String filename)
        {
            var extension = Path.GetExtension(filename);
            if (extension.ToLower() != this.FileExtension) filename = filename + this.FileExtension;
            return filename;
        }


        /// <summary>
        /// build Abstract syntax tree 
        /// Increase the path cache to prevent the endless loop of circular references 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public AstNode buildAst(string filepath, string relativePath = null)
        {
            AuroraLexer lexer;
            AstNode root;

            if (relativePath == null) relativePath = "";
            // get import fileName
            var filename = filepath.Replace("/", "\\");
            // full extension
            filename = this.fillExtension(filename);
            // get import file fullPath
            var fileFullPath = Path.GetFullPath(Path.Combine(relativePath, filename));
            if (!File.Exists(fileFullPath))
            {
                throw new CompilerException(fileFullPath, "Import file path not found ");
            }
            scriptParsers.TryGetValue(fileFullPath, out AuroraParser parser);
            if (parser != null)
            {
                return parser.root;
            }
            using (var time = new WitchTimer("lexer：" + fileFullPath))
            {
                lexer = new AuroraLexer(fileFullPath, Encoding.UTF8);
            }
            using (var time = new WitchTimer("parser：" + fileFullPath))
            {
                parser = new AuroraParser(this, lexer);
                this.scriptParsers.TryAdd(fileFullPath, parser);
                root = parser.Parse();
            }
            //
            this.PrintTreeCode(root);
            return root;
        }



        public void buildFile(string filepath)
        {
            AstNode root = this.buildAst(filepath);
            this.opaimizeTree(root);

        }



        private void PrintTreeCode(AstNode root)
        {
            foreach (var item in root.ChildNodes)
            {
                Console.WriteLine(item);
            }
        }



        /// <summary>
        /// optimize abstract syntax tree  
        /// </summary>
        /// <param name="root"></param>
        public void opaimizeTree(AstNode parent)
        {
            for (int i = parent.Length - 1; i >= 0; i--)
            {
                var node = parent[i];
                if (node is GroupExpression)
                {
                    node.Remove();
                    parent.AddNode(node);
                }
                opaimizeTree(node);
            }
        }



    }












}
