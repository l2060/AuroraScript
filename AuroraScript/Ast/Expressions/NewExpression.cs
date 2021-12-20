

namespace AuroraScript.Ast.Expressions
{
    internal class NewExpression : Expression
    {

        public Token Identifier { get; set; }


        public List<AstNode> Arguments { get; set; }



    }
}
