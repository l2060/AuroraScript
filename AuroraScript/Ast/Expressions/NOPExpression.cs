using AuroraScript.Stream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    public class NOPExpression : Expression
    {

        public Token Identifier { get; set; }


        public List<AstNode> Arguments { get; set; }





        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {

        }


    }
}
