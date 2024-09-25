using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Statements
{
    public class ReturnStatement : Statement
    {
        internal ReturnStatement(Expression expression)
        {
            this.Expression = expression;
        }

        public Expression Expression { get; private set; }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.KW_RETURN.Name);
            writer.Write(" ");
            this.Expression.GenerateCode(writer);
            writer.WriteLine(Symbols.PT_SEMICOLON.Name);
        }
    }
}