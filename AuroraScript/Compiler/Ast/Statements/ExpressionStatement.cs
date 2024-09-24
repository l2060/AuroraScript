


using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Statements
{
    public class ExpressionStatement : Statement
    {
        public Expression Expression { get; private set; }


        internal ExpressionStatement(Expression expression)
        {
            this.Expression = expression;
        }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            Expression.GenerateCode(writer);
            writer.WriteLine(Symbols.PT_SEMICOLON.Name);
        }
    }
}
