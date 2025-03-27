using AuroraScript.Compiler;


namespace AuroraScript.Ast.Expressions
{
    public class GroupExpression : OperatorExpression
    {
        internal GroupExpression(Operator @operator) : base(@operator)
        {
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitGroupingExpression(this);
        }

        public override string ToString()
        {
            return $"({ChildNodes[0]})";
        }
    }
}