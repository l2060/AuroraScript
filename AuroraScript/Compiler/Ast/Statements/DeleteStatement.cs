using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;


namespace AuroraScript.Compiler.Ast.Statements
{
    internal class DeleteStatement : Statement
    {
        internal DeleteStatement(Expression expression)
        {
            this.Expression = expression;
            expression.Parent = this;
        }

        public Expression Expression { get; private set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptDeleteStatement(this);
        }

        public override string ToString()
        {
            return $"delete {Expression}";
        }
    }
}