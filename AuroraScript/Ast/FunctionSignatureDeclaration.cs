using AuroraScript.Ast.Statements;
using AuroraScript.Ast.Types;
using AuroraScript.Stream;

namespace AuroraScript.Ast
{
    public class FunctionSignatureDeclaration: Statement
    {
        /// <summary>
        /// parameters
        /// </summary>
        public List<ParameterDeclaration> Parameters { get; set; }

        /// <summary>
        /// function result types
        /// </summary>
        public List<ObjectType> Typeds { get; set; }



        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            writer.Write($"{Symbols.PT_LEFTPARENTHESIS.Name}");
            this.writeParameters(writer, Parameters, ", ");
            writer.Write("{0} {1} {2} =>", Symbols.PT_RIGHTPARENTHESIS.Name, Symbols.PT_LAMBDA.Name, Symbols.PT_COLON.Name);
            if (Typeds.Count > 0)
            {
                if (Typeds.Count == 1)
                {
                    this.writeParameters(writer, Typeds, Symbols.PT_COMMA.Name + " ");
                }
                else
                {
                    writer.Write(Symbols.PT_LEFTBRACKET.Name);
                    this.writeParameters(writer, Typeds, Symbols.PT_COMMA.Name + " ");
                    writer.Write(Symbols.PT_RIGHTBRACKET.Name);
                }
            }
        }

    }
}
