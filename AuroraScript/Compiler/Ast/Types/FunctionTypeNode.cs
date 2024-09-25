using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Types
{
    public class FunctionTypeNode : TypeNode
    {
        public AstNode Name { get; set; }
        public List<ParameterDeclaration> Parameters { get; set; }

        public List<TypeNode> Typeds;

        public override void GenerateCode(TextCodeWriter writer, int depth = 0)
        {
            writer.Write($"{Symbols.PT_LEFTPARENTHESIS.Name}");
            if (Parameters != null) writeParameters(writer, Parameters, ", ");
            writer.Write("{0} {1} ", Symbols.PT_RIGHTPARENTHESIS.Name, Symbols.PT_LAMBDA.Name);
            if (Typeds != null && Typeds.Count > 0)
            {
                if (Typeds.Count == 1)
                {
                    writeParameters(writer, Typeds, Symbols.PT_COMMA.Name + " ");
                }
                else
                {
                    writer.Write(Symbols.PT_LEFTBRACKET.Name);
                    writeParameters(writer, Typeds, Symbols.PT_COMMA.Name + " ");
                    writer.Write(Symbols.PT_RIGHTBRACKET.Name);
                }
            }
        }
    }
}