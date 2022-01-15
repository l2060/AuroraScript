


using AuroraScript.Ast.Expressions;

namespace AuroraScript.Ast.Statements
{
    public class ExpressionStatement : Statement
    {
        public Expression Expression { get; private set; }


        internal ExpressionStatement(Expression expression)
        {
            this.Expression = expression;
        }

        public override String ToString()
        {
            return $"{Expression}{Symbols.PT_SEMICOLON.Name}";
        }


    }
}
