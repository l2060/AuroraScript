
namespace AuroraScript.Ast.Expressions
{
    public class ArrayAccessExpression : OperatorExpression
    {
        internal ArrayAccessExpression(Operator @operator) : base(@operator)
        {
        }

        public Expression Index { get; set; }

    }
}
