
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public class ArrayConstructExpression : OperatorExpression
    {
        internal ArrayConstructExpression() : base(Operator.Array)
        {
        }

        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Operator.Array.Symbol.Name);
            writeParameters(writer, ChildNodes, Symbols.PT_COMMA.Name + " ");
            writer.Write(Operator.Array.SecondarySymbols.Name);
        }
    }
}
