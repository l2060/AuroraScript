
namespace AuroraScript.Ast.Statements
{
    public class BreakStatement : Statement
    {
        internal BreakStatement()
        {

        }
        public override String ToString()
        {
            return $"{Symbols.KW_BREAK.Name}{Symbols.PT_SEMICOLON.Name}\r\n";
        }
    }
}
