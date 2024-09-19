
using AuroraScript.Stream;

namespace AuroraScript.Ast.Statements
{
    public class Statement : AstNode
    {
        internal Statement()
        {

        }

        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            writer.WriteLine("????????");
            writer.WriteLine(Symbols.PT_SEMICOLON.Name);
        }


    }
}
