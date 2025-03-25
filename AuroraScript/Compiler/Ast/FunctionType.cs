using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast
{
    public class FunctionType : Expression
    {
        /// <summary>
        /// parameters
        /// </summary>
        public List<ParameterDeclaration> Parameters { get; set; }

        public override void GenerateCode(TextCodeWriter writer, int depth = 0)
        {
            writer.Write(Symbols.PT_LEFTPARENTHESIS.Name);
            if (Parameters != null) writeParameters(writer, Parameters, ", ");
            writer.Write(Symbols.PT_RIGHTPARENTHESIS.Name);
        }

        public void GenerateLambdaCode(TextCodeWriter writer, int depth = 0)
        {
            writer.Write(Symbols.PT_LEFTPARENTHESIS.Name);
            if (Parameters != null) writeParameters(writer, Parameters, ", ");
            writer.Write(Symbols.PT_RIGHTPARENTHESIS.Name);
        }
        public override void Accept(IAstVisitor visitor)
        {

        }
    }
}