
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

    }
}
