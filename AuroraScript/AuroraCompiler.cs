using AuroraScript.Analyzer;
using AuroraScript.Ast;
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
        /// build Abstract syntax tree 
        /// Increase the path cache to prevent the endless loop of circular references 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public AstNode buildAst(string filepath)
        {
            var fullPath = Path.GetFullPath(filepath);
            scriptParsers.TryGetValue(fullPath, out AuroraParser parser);
            if (parser != null)
            {
                return parser.root;
            }
            AuroraLexer lexer;
            //AuroraParser parser;
            AstNode root;
            using (var time = new WitchTimer("lexer：" + fullPath))
            {
                lexer = new AuroraLexer(fullPath, Encoding.UTF8);
            }
            using (var time = new WitchTimer("parser：" + fullPath))
            {
                parser = new AuroraParser(this, lexer);
                this.scriptParsers.TryAdd(fullPath, parser);
                root = parser.Parse();
            }
            return root;
        }



        public void buildFile(string filepath)
        {
            AstNode root = this.buildAst(filepath);
            //
            var printer = new AstPrinter(root);
            printer.print();
        }
    }









}
