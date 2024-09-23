using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Ast.Types;
using AuroraScript.Stream;

namespace AuroraScript.Ast
{
    public class FunctionType : Expression
    {
        /// <summary>
        /// parameters
        /// </summary>
        public List<ParameterDeclaration> Parameters { get; set; }

        /// <summary>
        /// function result types
        /// </summary>
        public List<TypeNode> Typeds { get; set; }



        public override void GenerateCode(CodeWriter writer, int depth = 0)
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

        public void GenerateLambdaCode(CodeWriter writer, int depth = 0)
        {
            writer.Write($"{Symbols.PT_LEFTPARENTHESIS.Name}");
            if (Parameters != null) writeParameters(writer, Parameters, ", ");
            writer.Write("{0}{1} ", Symbols.PT_RIGHTPARENTHESIS.Name, Symbols.PT_COLON.Name);
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
