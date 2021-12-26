

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// PrefixUnary Expression
    /// ++i
    /// --i
    /// </summary>
    public class PrefixUnaryExpression : OperatorExpression
    {
        internal PrefixUnaryExpression(Operator @operator) : base(@operator)
        {
        }



        public Exception Operand { get; set; }


        public Expression Right
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }

        public override String ToString()
        {
            return $"({this.Operator.Symbol.Name}{this.Right})";
        }
    }
}
