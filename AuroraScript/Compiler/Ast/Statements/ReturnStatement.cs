using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;


namespace AuroraScript.Ast.Statements
{
    internal class ReturnStatement : Statement
    {
        internal ReturnStatement(Expression expression)
        {
            this.Expression = expression;
            expression.Parent = this;
        }

        public Expression Expression { get; private set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptReturnStatement(this);
        }

        public override string ToString()
        {
            return $"return {Expression}";
        }
    }
}