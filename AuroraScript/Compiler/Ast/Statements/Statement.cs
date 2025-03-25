using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Statements
{
    public abstract class Statement : AstNode
    {
        internal Statement()
        {
        }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.WriteLine("????????");
            writer.WriteLine(Symbols.PT_SEMICOLON.Name);
        }
    }
}