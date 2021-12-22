
namespace AuroraScript.Ast.Expressions
{
    public class ArrayExpression : OperatorExpression
    {
        internal ArrayExpression(Operator @operator) : base(@operator)
        {
            this.Elements = new List<Expression>();
        }

        public List<Expression> Elements { get; set; }

    }
}
