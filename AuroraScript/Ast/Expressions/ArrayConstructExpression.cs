
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public class ArrayConstructExpression : OperatorExpression
    {
        internal ArrayConstructExpression(Operator @operator) : base(@operator)
        {
            this.Elements = new List<Expression>();
        }

        public List<Expression> Elements { get; set; }


        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            var els = Elements.Select(el => el.ToString()).ToArray();
            writer.Write(Operator.Array.Symbol.Name);
            writeParameters(writer, Elements, Symbols.PT_COMMA.Name + " ");
            writer.Write(Operator.Array.SecondarySymbols.Name);
        }
    }
}
