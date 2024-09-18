


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

        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            Expression.WriteCode(writer);
            writer.WriteLine(Symbols.PT_SEMICOLON.Name);
        }
    }
}
