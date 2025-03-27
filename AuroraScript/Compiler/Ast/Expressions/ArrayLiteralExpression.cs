using AuroraScript.Compiler;

namespace AuroraScript.Ast.Expressions
{
    public class ArrayLiteralExpression : OperatorExpression
    {
        internal ArrayLiteralExpression() : base(Operator.ArrayLiteral)
        {
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitArrayExpression(this);
        }

        public override string ToString()
        {
            return $"[{String.Join(", ", ChildNodes)}]";
        }


    }
}