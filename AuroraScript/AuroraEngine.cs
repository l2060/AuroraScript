using AuroraScript.Analyzer;
using AuroraScript.Ast.Statements;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuroraScript
{
    public class AuroraEngine
    {
        public void build(string filepath)
        {
            var Lexer = new AuroraLexer(filepath, Encoding.UTF8);
            var parser = new AuroraParser(Lexer);
            var node = parser.Parse();


            Console.WriteLine(node);


            string str = JsonConvert.SerializeObject(node);


            Console.WriteLine(str);






        }


    }
}
