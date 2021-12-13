using AuroraScript.Analyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript
{
    public class AuroraEngine
    {




        public void build(string filepath)
        {
            var Lexer = new AuroraLexer(filepath, Encoding.UTF8);
            while (true)
            {
                var token = Lexer.Next();
                if (token == Token.EOF) return;






            }
        }


    }
}
