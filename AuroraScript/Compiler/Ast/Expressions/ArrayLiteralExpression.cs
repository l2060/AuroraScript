
using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public class ArrayLiteralExpression : OperatorExpression
    {
        internal ArrayLiteralExpression() : base(Operator.ArrayLiteral)
        {
        }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Operator.ArrayLiteral.Symbol.Name);
            writeParameters(writer, ChildNodes, Symbols.PT_COMMA.Name + " ");
            writer.Write(Operator.ArrayLiteral.SecondarySymbols.Name);
        }
    }
}
