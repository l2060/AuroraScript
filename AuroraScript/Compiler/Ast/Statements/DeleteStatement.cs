using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;


namespace AuroraScript.Compiler.Ast.Statements
{
    public class DeleteStatement : Statement
    {
        internal DeleteStatement(Expression expression)
        {
            this.Expression = expression;
            expression.Parent = this;
        }

        public Expression Expression { get; private set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitDeleteStatement(this);
        }

        public override string ToString()
        {
            return $"delete {Expression}";
        }
    }
}