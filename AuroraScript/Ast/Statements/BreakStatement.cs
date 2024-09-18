
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


        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.KW_BREAK.Name);
            writer.WriteLine(Symbols.PT_SEMICOLON.Name);
        }
    }
}
