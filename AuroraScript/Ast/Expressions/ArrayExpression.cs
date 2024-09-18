
namespace AuroraScript.Ast.Expressions
{
    public class ArrayExpression : OperatorExpression
    {
        internal ArrayExpression(Operator @operator) : base(@operator)
        {
            this.Elements = new List<Expression>();
        }

        public List<Expression> Elements { get; set; }



        public override String ToString()
        {
            var els = Elements.Select(el => el.ToString()).ToArray();
            return $"{Operator.Array.Symbol.Name}{String.Join(',', els)}{Operator.Array.SecondarySymbols.Name}";
        }

        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            var els = Elements.Select(el => el.ToString()).ToArray();
            writer.Write(Operator.Array.Symbol.Name);
            writeParameters(writer, Elements, ", ");
            writer.Write(Operator.Array.SecondarySymbols.Name);
        }
    }
}
