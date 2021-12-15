using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    internal class NewExpression : Expression
    {

        public Token Identifier { get; set; }


        public List<AstNode> Arguments { get; set; }



    }
}
