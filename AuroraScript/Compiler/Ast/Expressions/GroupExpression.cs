using AuroraScript.Compiler;


namespace AuroraScript.Ast.Expressions
{
    internal class GroupExpression : OperatorExpression
    {
        internal GroupExpression(Operator @operator) : base(@operator)
        {
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptGroupingExpression(this);
        }

        public override string ToString()
        {
            return $"({ChildNodes[0]})";
        }
    }
}