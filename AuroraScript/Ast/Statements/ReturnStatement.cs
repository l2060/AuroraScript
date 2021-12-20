using AuroraScript.Ast.Expressions;

namespace AuroraScript.Ast.Statements
{
    public class ReturnStatement : Statement
    {
        internal ReturnStatement(Expression expression)
        {
            this.Expression = expression;
        }

        public Expression Expression { get; private set; }

    }
}
