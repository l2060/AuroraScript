
using AuroraScript.Stream;

namespace AuroraScript.Ast.Statements
{
    public class ContinueStatement : Statement
    {
        internal ContinueStatement()
        {

        }


        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.KW_CONTINUE.Name);
            writer.Write(Symbols.PT_SEMICOLON.Name);
        }
    }
}
