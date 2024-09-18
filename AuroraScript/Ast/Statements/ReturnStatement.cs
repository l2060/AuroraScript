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


        public override String ToString()
        {
            return $"{Symbols.KW_RETURN.Name} {this.Expression}{Symbols.PT_SEMICOLON.Name}\r\n";
        }


        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.KW_RETURN.Name);
            writer.Write(" ");
            this.Expression.WriteCode(writer);
            writer.WriteLine(Symbols.PT_SEMICOLON.Name);
        }
    }
}
