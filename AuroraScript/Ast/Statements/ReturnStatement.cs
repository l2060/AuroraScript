using AuroraScript.Ast.Expressions;

namespace AuroraScript.Ast.Statements
{
    internal class ReturnStatement : Statement
    {
        public ReturnStatement(Expression expression)
        {
            this.expression = expression;
        }

        public Expression expression;

    }
}
