using AuroraScript.Compiler;

namespace AuroraScript.Ast.Expressions
{
    public class MapExpression : OperatorExpression
    {
        internal MapExpression(Operator @operator) : base(@operator)
        {
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitMapExpression(this);
        }


        public override string ToString()
        {
            return $"{{ {String.Join(", ", ChildNodes)} }}";
        }
    }
}