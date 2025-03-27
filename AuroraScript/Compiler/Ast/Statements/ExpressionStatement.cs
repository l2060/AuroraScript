using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;


namespace AuroraScript.Ast.Statements
{
    public class ExpressionStatement : Statement
    {
        public Expression Expression { get; private set; }

        internal ExpressionStatement(Expression expression)
        {
            this.Expression = expression;
        }

        public override void Accept(IAstVisitor visitor)
        {
            this.Expression.Accept(visitor);
        }
    }
}