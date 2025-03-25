using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Statements
{
    public class BreakStatement : Statement
    {
        internal BreakStatement()
        {
        }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.KW_BREAK.Name);
            writer.WriteLine(Symbols.PT_SEMICOLON.Name);
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitBreakExpression(this);
        }
    }
}