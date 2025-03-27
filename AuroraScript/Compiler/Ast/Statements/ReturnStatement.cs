using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;


namespace AuroraScript.Ast.Statements
{
    public class ReturnStatement : Statement
    {
        internal ReturnStatement(Expression expression)
        {
            this.Expression = expression;
        }

        public Expression Expression { get; private set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitReturnStatement(this);
        }

        public override string ToString()
        {
            return $"return {Expression}";
        }
    }
}