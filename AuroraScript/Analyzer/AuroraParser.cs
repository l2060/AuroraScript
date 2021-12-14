using AuroraScript.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Analyzer
{
    internal class AuroraParser
    {
        private AuroraLexer lexer;
        public AuroraParser(AuroraLexer lexer)
        {
            this.lexer = lexer;
        }



        public AstNode Parse()
        {
            AstNode result = new BlockStatement();
            while (true)
            {
                var token = this.lexer.Next();
                Console.WriteLine(token);

                if (token == Token.EOF) break;






            }


            return result;
        }









    }
}
