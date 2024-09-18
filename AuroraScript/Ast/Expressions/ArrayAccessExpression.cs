
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

        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            Target.WriteCode(writer);
            writer.Write(Operator.Array.Symbol.Name);
            Index.WriteCode(writer);
            writer.Write(Operator.Array.SecondarySymbols.Name);
        }



    }
}
