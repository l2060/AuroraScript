
namespace AuroraScript.Ast.Expressions
{
    public class ArrayAccessExpression : OperatorExpression
    {
        internal ArrayAccessExpression(Operator @operator) : base(@operator)
        {
        }

        public Expression Index { get; set; }



        public Expression Target
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }


        public override String ToString()
        {
            return $" {Target}{Operator.Array.Symbol.Name}{Index}{Operator.Array.SecondarySymbols.Name} ";
        }
    }
}
