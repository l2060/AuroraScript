
namespace AuroraScript.Ast.Expressions
{
    public class ArrayExpression : Expression
    {

        internal ArrayExpression()
        {

        }

        public List<Expression> Elements { get; set; }

    }
}
