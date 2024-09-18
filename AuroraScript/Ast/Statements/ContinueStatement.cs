
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
        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.KW_CONTINUE.Name);
            writer.Write(Symbols.PT_SEMICOLON.Name);
        }
    }
}
