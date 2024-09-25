using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Types
{
    public class ArrayType : TypeNode
    {
        private TypeNode node;

        public ArrayType(TypeNode node)
        {
            this.node = node;
        }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            this.node.GenerateCode(writer, depth);
            writer.Write($"{Symbols.PT_LEFTBRACKET.Name}{Symbols.PT_RIGHTBRACKET.Name}");
        }
    }
}