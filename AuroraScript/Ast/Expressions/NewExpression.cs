

namespace AuroraScript.Ast.Expressions
{
    public class NewExpression : Expression
    {

        public Token Identifier { get; set; }


        public List<AstNode> Arguments { get; set; }



    }
}
