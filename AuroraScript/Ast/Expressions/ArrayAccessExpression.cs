
using AuroraScript.Stream;

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


        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            Target.GenerateCode(writer);
            writer.Write(Operator.Array.Symbol.Name);
            Index.GenerateCode(writer);
            writer.Write(Operator.Array.SecondarySymbols.Name);
        }



    }
}
