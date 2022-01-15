
namespace AuroraScript.Ast.Statements
{
    public class ContinueStatement : Statement
    {
        internal ContinueStatement()
        {

        }
        public override String ToString()
        {
            return $"{Symbols.KW_CONTINUE.Name}{Symbols.PT_SEMICOLON.Name}\r\n";
        }
    }
}
